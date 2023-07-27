import fetch from "node-fetch";
import crypto from "crypto";

class PasswordManagerApi {
  constructor(backendDomain) {
    this.backendDomain = backendDomain;
    this.apiHeaders = {
      "Content-type": "application/json",
      accept: "application/json",
    };
  }

  async userRegister(username, password) {
    const prehashedPassword = await this.#sha512(password + username);
    const response = await fetch(`${this.backendDomain}/api/User/Register`, {
      method: "POST",
      body: JSON.stringify({
        username: username,
        prehashedPassword: prehashedPassword,
      }),
      headers: this.apiHeaders,
    });

    const message = (await response.json()).message;
    if (response.ok) {
      return message;
    } else {
      throw message;
    }
  }

  async #sha512(data) {
    return crypto.createHash("sha512").update(data, "binary").digest("hex");
  }
}

export default PasswordManagerApi;
