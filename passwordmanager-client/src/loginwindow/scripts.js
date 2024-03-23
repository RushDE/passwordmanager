const { ipcRenderer } = require("electron");


// if the user specified password is 123 then it will be sent to the main process on submit button press

function login() {

  const passwordForm = document.getElementById("master-login-form");

  passwordForm.addEventListener("submit", (e) => {
    const password = document.getElementById("master-password").value;
    const master = document.getElementById("master-name").value;
   // send the password to the main process
    ipcRenderer.send("send-master-login", master, password);
  });
}

login();
