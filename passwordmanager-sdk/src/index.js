import fetch from "node-fetch";
import crypto from "crypto";

class PasswordManagerApi {
  constructor(backendDomain) {
    this.API_HEADERS = {
      "Content-type": "application/json",
      accept: "application/json",
    };

    this.backendDomain = backendDomain;
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
      headers: this.API_HEADERS,
    });
    const json = await response.json();
    if (response.ok) {
      return json;
    } else {
      throw json;
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
      headers: this.API_HEADERS,
    });
    const json = await response.json();
    if (response.ok) {
      this.token = json.token;
      return json;
    } else {
      throw json;
    }
  }

  async #sha512(data) {
    return crypto.createHash("sha512").update(data, "binary").digest("hex");
  }
}

export default PasswordManagerApi;
