import fetch from "node-fetch";
import crypto from "crypto";


//////////////////// Settings ////////////////////


const backendDomain = "http://localhost:5086"


//////////////////// Main ////////////////////


const username = "Max";
const password = "hunter2";

await userRegister(username, password);
const token = await userLogin(username, password);
const userPasswords = await listPasswordEntrys(token);
console.log("User passwords:")
console.log(userPasswords);


//////////////////// Functions ////////////////////


function sha512(data) {
    return crypto.createHash("sha512").update(data, "binary").digest("hex");
}

async function userRegister(username, password) {
    const prehashedPassword = sha512(password + username);
    const response = await fetch(
        `${backendDomain}/api/User/Register`,
        {
            method: "POST",
            body: JSON.stringify(
                {
                    username: username,
                    prehashedPassword: prehashedPassword
                }
            ),
            headers: {
                "Content-type": "application/json",
                "accept": "application/json"
            }
        }
    );
    const json = await response.json();
    console.log(json.message);
}

async function userLogin(username, password) {
    const prehashedPassword = sha512(password + username);
    const response = await fetch(
        `${backendDomain}/api/User/Login`,
        {
            method: "POST",
            body: JSON.stringify(
                {
                    username: username,
                    prehashedPassword: prehashedPassword
                }
            ),
            headers: {
                "Content-type": "application/json",
                "accept": "application/json"
            }
        }
    );
    const json = await response.json();
    console.log(json.message);
    return json.token;
}

async function listPasswordEntrys(token) {
    const response = await fetch(
        `${backendDomain}/api/Vault/ListPasswordEntrys`,
        {
            method: "GET",
            headers: {
                "Content-type": "application/json",
                "accept": "application/json",
                "Authorization": token
            }
        }
    );
    const json = await response.json();
    return json;
}

