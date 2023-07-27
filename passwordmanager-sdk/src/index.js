import fetch from "node-fetch";
import crypto from "crypto";

export class PasswordManagerApi {
  constructor(backendDomain) {
    this.backendDomain = backendDomain;
    this.username = undefined;
    this.token = undefined;
  }

  async userRegister(username, password) {
    const prehashedPassword = await this.#sha512(password + username);
    const response = await fetch(`${this.backendDomain}/api/User/Register`, {
      method: "POST",
      body: JSON.stringify({
        username: username,
        prehashedPassword: prehashedPassword,
      }),
      headers: this.#getApiHeaders(),
    });
    const json = await response.json();
    if (response.ok) {
      return json;
    } else {
      throw new ApiError(json);
    }
  }

  async userLogin(username, password) {
    const prehashedPassword = await this.#sha512(password + username);
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
      return json;
    } else {
      throw new ApiError(json);
    }
  }

  async userChangePassword(oldPassword, newPassword) {
    if (this.username === undefined || this.token === undefined) {
      throw new ApiError({ message: "You have to login first." });
    }
    const prehashedOldPassword = await this.#sha512(
      oldPassword + this.username
    );
    const prehashedNewPassword = await this.#sha512(
      newPassword + this.username
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
    const json = await response.json();
    if (response.ok) {
      return json;
    } else {
      throw new ApiError(json);
    }
  }

  async #sha512(data) {
    return crypto.createHash("sha512").update(data, "binary").digest("hex");
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
