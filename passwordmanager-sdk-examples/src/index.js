import PasswordManagerApi from "passwordmanager-sdk";

//////////////////// "Settings" ////////////////////

const USERNAME = "joe";
const PASSWORD = "hunter2";

//////////////////// Constants ////////////////////

// Source: https://stackoverflow.com/a/41407246/19860801
const COLORS = Object.freeze({
  FG_RED: "\x1b[31m",
  FG_GREEN: "\x1b[32m",
  CMD_RESET: "\x1b[0m",
});

//////////////////// Main ////////////////////

async function main() {
  // Create a PasswordManagerApi instance.
  const pma = new PasswordManagerApi("http://localhost:5086");

  // Create a new user.
  try {
    console.log(COLORS.FG_GREEN, await pma.userRegister(USERNAME, PASSWORD));
  } catch (error) {
    console.log(COLORS.FG_RED, error);
  }

  // Cleanup.
  console.log(COLORS.CMD_RESET);
}

main();
