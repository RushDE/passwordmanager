import fetch from "node-fetch";
import forge from "node-forge";
import crypto from "crypto";

export class PasswordManagerApi {
  //////////////////// "Settings" ////////////////////

  #PBKDF2_ITERATIONS;
  #PREHASH_ITERATIONS;

  //////////////////// Private fields ////////////////////

  #backendDomain;
  #username;
  #token;
  #key;

  //////////////////// Initialisation ////////////////////

  constructor(backendDomain) {
    this.#PBKDF2_ITERATIONS = 5_000_000;
    this.#PREHASH_ITERATIONS = 15;
    this.#backendDomain = backendDomain;
    this.#username = undefined;
    this.#token = undefined;
    this.#key = undefined;
  }

  //////////////////// /api/User/* Endpoints ////////////////////

  async userRegister(username, password) {
    const prehashedPassword = await this.#sha512(
      password + username,
      this.#PREHASH_ITERATIONS
    );
    const response = await fetch(`${this.#backendDomain}/api/User/Register`, {
      method: "POST",
      body: JSON.stringify({
        username: username,
        prehashedPassword: prehashedPassword,
      }),
      headers: this.#getApiHeaders(),
    });
    if (!response.ok) {
      throw new ApiError((await response.json()).message);
    }
  }

  async userLogin(username, password) {
    const prehashedPassword = await this.#sha512(
      password + username,
      this.#PREHASH_ITERATIONS
    );
    const response = await fetch(`${this.#backendDomain}/api/User/Login`, {
      method: "POST",
      body: JSON.stringify({
        username: username,
        prehashedPassword: prehashedPassword,
      }),
      headers: this.#getApiHeaders(),
    });
    const json = await response.json();
    if (response.ok) {
      this.#username = username;
      this.#token = json.token;
      this.#key = this.#deriveKeyFromPassword(password);
    } else {
      throw new ApiError(json.message);
    }
  }

  async userChangePassword(oldPassword, newPassword) {
    if (this.#username === undefined || this.#token === undefined) {
      throw new ApiError("You have to login first.");
    }
    const prehashedOldPassword = await this.#sha512(
      oldPassword + this.#username,
      this.#PREHASH_ITERATIONS
    );
    const prehashedNewPassword = await this.#sha512(
      newPassword + this.#username,
      this.#PREHASH_ITERATIONS
    );

    // Reencrypt all passwords.
    const passwordEntrys = await this.vaultListPasswordEntrys();
    this.#key = this.#deriveKeyFromPassword(newPassword);
    let reencryptedPasswordEntrys = [];
    for (const passwordEntry of passwordEntrys) {
      let reencryptedPasswordEntry = this.#encryptPasswordEntry({
        name: passwordEntry.name,
        link: passwordEntry.link,
        username: passwordEntry.username,
        password: passwordEntry.password,
      });
      reencryptedPasswordEntry.uuid = passwordEntry.uuid;
      reencryptedPasswordEntrys.push(reencryptedPasswordEntry);
    }

    const response = await fetch(
      `${this.#backendDomain}/api/User/ChangePassword`,
      {
        method: "PATCH",
        body: JSON.stringify({
          prehashedOldPassword: prehashedOldPassword,
          prehashedNewPassword: prehashedNewPassword,
          reencryptedPasswords: reencryptedPasswordEntrys,
        }),
        headers: this.#getApiHeaders(),
      }
    );
    if (!response.ok) {
      throw new ApiError((await response.json()).message);
    }
  }

  async userDelete(password) {
    if (this.#username === undefined || this.#token === undefined) {
      throw new ApiError({ message: "You have to login first." });
    }
    const prehashedPassword = await this.#sha512(
      password + this.#username,
      this.#PREHASH_ITERATIONS
    );
    const response = await fetch(`${this.#backendDomain}/api/User/DeleteUser`, {
      method: "DELETE",
      body: JSON.stringify({
        prehashedPassword: prehashedPassword,
      }),
      headers: this.#getApiHeaders(),
    });
    if (!response.ok) {
      throw new ApiError((await response.json()).message);
    }
  }

  async userLogout() {
    this.#username = undefined;
    this.#token = undefined;
    this.#key = undefined;
  }

  //////////////////// /api/Vault/* Endpoints ////////////////////

  async vaultCreatePassword({
    name = null,
    link = null,
    username = null,
    password = null,
  }) {
    const passwordEntry = this.#encryptPasswordEntry({
      name: name,
      link: link,
      username: username,
      password: password,
    });
    const response = await fetch(
      `${this.#backendDomain}/api/Vault/CreatePasswordEntry`,
      {
        method: "POST",
        body: JSON.stringify(passwordEntry),
        headers: this.#getApiHeaders(),
      }
    );
    if (!response.ok) {
      throw new ApiError((await response.json()).message);
    }
  }

  async vaultUpdatePasswordEntry(
    passwordUuid,
    { name = null, link = null, username = null, password = null }
  ) {
    let passwordEntry = this.#encryptPasswordEntry({
      name: name,
      link: link,
      username: username,
      password: password,
    });
    passwordEntry.uuid = passwordUuid;
    const response = await fetch(
      `${this.#backendDomain}/api/Vault/UpdatePasswordEntry`,
      {
        method: "PATCH",
        body: JSON.stringify(passwordEntry),
        headers: this.#getApiHeaders(),
      }
    );
    if (!response.ok) {
      throw new ApiError((await response.json()).message);
    }
  }

  async vaultDeletePasswordEntry(passwordUuid) {
    const response = await fetch(
      `${this.#backendDomain}/api/Vault/DeletePasswordEntry/${passwordUuid}`,
      {
        method: "DELETE",
        headers: this.#getApiHeaders(),
      }
    );
    if (!response.ok) {
      throw new ApiError((await response.json()).message);
    }
  }

  async vaultListPasswordEntrys() {
    // Fetch the passwords from the server.
    const response = await fetch(
      `${this.#backendDomain}/api/Vault/ListPasswordEntrys`,
      {
        method: "GET",
        headers: this.#getApiHeaders(),
      }
    );
    if (!response.ok) {
      throw new ApiError((await response.json()).message);
    }
    const json = await response.json();

    // Decrypt all the password entrys.
    let passwordEntrys = [];
    for (const encryptedPasswordEntry of json) {
      const passwordEntry = {
        uuid: encryptedPasswordEntry.uuid,
        name: this.#decryptAes256(encryptedPasswordEntry.encryptedName),
        link: this.#decryptAes256(encryptedPasswordEntry.encryptedLink),
        username: this.#decryptAes256(encryptedPasswordEntry.encryptedUsername),
        password: this.#decryptAes256(encryptedPasswordEntry.encryptedPassword),
      };
      passwordEntrys.push(passwordEntry);
    }

    // Return the entrys, because nothing can really error out.
    return passwordEntrys;
  }

  //////////////////// Helper functions ////////////////////

  async #sha512(data, iterations) {
    for (let i = 0; i < iterations; i++) {
      data = crypto.createHash("sha512").update(data, "binary").digest("hex");
    }
    return data;
  }

  #getApiHeaders() {
    let headers = {
      "Content-type": "application/json",
      accept: "application/json",
    };
    if (this.#token !== undefined) {
      headers.Authorization = this.#token;
    }
    return headers;
  }

  #deriveKeyFromPassword(password) {
    const passwordBytes = forge.util.encodeUtf8(password);
    const saltBytes = new Uint8Array(16); // Should be all zeros, because the key is prvate.
    const derivedKeyBytes = forge.pkcs5.pbkdf2(
      passwordBytes,
      saltBytes,
      this.#PBKDF2_ITERATIONS,
      32,
      "sha256"
    );
    return Buffer.from(derivedKeyBytes, "binary");
  }

  #encryptAes256(input) {
    if (!input) {
      return null;
    }
    const iv = crypto.randomBytes(16); // Generate a random initialization vector (IV).
    const cipher = crypto.createCipheriv("aes-256-cbc", this.#key, iv);
    let encrypted = cipher.update(input, "utf8", "hex");
    encrypted += cipher.final("hex");
    return iv.toString("hex") + encrypted;
  }

  #decryptAes256(encrypted) {
    if (!encrypted) {
      return null;
    }
    const iv = Buffer.from(encrypted.slice(0, 32), "hex"); // Extract the IV from the encrypted text.
    encrypted = encrypted.slice(32); // Remove the IV from the encrypted text.
    const decipher = crypto.createDecipheriv("aes-256-cbc", this.#key, iv);
    let decrypted = decipher.update(encrypted, "hex", "utf8");
    decrypted += decipher.final("utf8");
    return decrypted;
  }

  #encryptPasswordEntry({
    name = null,
    link = null,
    username = null,
    password = null,
  }) {
    // Initialize the new password entry.
    let passwordEntry = {
      encryptedName: null,
      encryptedLink: null,
      encryptedUsername: null,
      encryptedPassword: null,
    };

    // Encrypt the fields if they aren't empty.
    passwordEntry.encryptedName = this.#encryptAes256(name);
    passwordEntry.encryptedLink = this.#encryptAes256(link);
    passwordEntry.encryptedUsername = this.#encryptAes256(username);
    passwordEntry.encryptedPassword = this.#encryptAes256(password);

    // Return them.
    return passwordEntry;
  }
}

export class ApiError extends Error {
  constructor(message) {
    message = JSON.stringify(message, null, 4);
    super(message);
    this.name = this.constructor.name;
  }
}
