import fetch from "node-fetch";
import crypto from "crypto";

export class PasswordManagerApi {
  #HASH_ITAERATIONS;
  #PREHASH_ITERATIONS;

  constructor(backendDomain) {
    this.#HASH_ITAERATIONS = 5;
    this.#PREHASH_ITERATIONS = 15;
    this.backendDomain = backendDomain;
    this.username = undefined;
    this.token = undefined;
  }

  async userRegister(username, password) {
    const prehashedPassword = await this.#sha512(
      password + username,
      this.#PREHASH_ITERATIONS
    );
    const response = await fetch(`${this.backendDomain}/api/User/Register`, {
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
    const response = await fetch(`${this.backendDomain}/api/User/Login`, {
      method: "POST",
      body: JSON.stringify({
        username: username,
        prehashedPassword: prehashedPassword,
      }),
      headers: this.#getApiHeaders(),
    });
    const json = await response.json();
    if (response.ok) {
      this.username = username;
      this.token = json.token;
    } else {
      throw new ApiError(json.message);
    }
  }

  async userChangePassword(oldPassword, newPassword) {
    // TODO: Would *probably* be good to reencrypt the passwords with the new password.
    if (this.username === undefined || this.token === undefined) {
      throw new ApiError("You have to login first.");
    }
    const prehashedOldPassword = await this.#sha512(
      oldPassword + this.username,
      this.#PREHASH_ITERATIONS
    );
    const prehashedNewPassword = await this.#sha512(
      newPassword + this.username,
      this.#PREHASH_ITERATIONS
    );
    const response = await fetch(
      `${this.backendDomain}/api/User/ChangePassword`,
      {
        method: "PATCH",
        body: JSON.stringify({
          prehashedOldPassword: prehashedOldPassword,
          prehashedNewPassword: prehashedNewPassword,
        }),
        headers: this.#getApiHeaders(),
      }
    );
    if (!response.ok) {
      throw new ApiError((await response.json()).message);
    }
  }

  async userDelete(password) {
    if (this.username === undefined || this.token === undefined) {
      throw new ApiError({ message: "You have to login first." });
    }
    const prehashedPassword = await this.#sha512(
      password + this.username,
      this.#PREHASH_ITERATIONS
    );
    const response = await fetch(`${this.backendDomain}/api/User/DeleteUser`, {
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
    if (this.token !== undefined) {
      headers.Authorization = this.token;
    }
    return headers;
  }
}

export class ApiError extends Error {
  constructor(message) {
    message = JSON.stringify(message, null, 4);
    super(message);
    this.name = this.constructor.name;
  }
}
