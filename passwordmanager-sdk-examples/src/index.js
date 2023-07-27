import PasswordManagerApi from "passwordmanager-sdk";

//////////////////// "Settings" ////////////////////

const USERNAME = "joe";
const PASSWORD = "hunter2";

//////////////////// Constants ////////////////////

// Source: https://stackoverflow.com/a/41407246/19860801
const COLORS = Object.freeze({
  FG_CYAN: "\x1b[36m",
  FG_GREEN: "\x1b[32m",
  FG_RED: "\x1b[31m",
  CMD_RESET: "\x1b[0m",
});

//////////////////// Functions ////////////////////

function logInfo(output) {
  console.log(`${COLORS.FG_CYAN}INFO:${COLORS.CMD_RESET} ${output}`);
}

function logSuccess(output) {
  console.log(
    `${COLORS.FG_GREEN}SUCCESS:${COLORS.CMD_RESET}\n${JSON.stringify(
      output,
      null,
      4
    )}`
  );
}

function logError(output) {
  console.log(
    `${COLORS.FG_RED}ERROR:${COLORS.CMD_RESET}\n${JSON.stringify(
      output,
      null,
      4
    )}`
  );
}

//////////////////// Main ////////////////////

async function main() {
  // Create a PasswordManagerApi instance.
  const pma = new PasswordManagerApi("http://localhost:5086");

  logInfo("Creating a new user.");
  try {
    logSuccess(await pma.userRegister(USERNAME, PASSWORD));
  } catch (error) {
    logError(error);
  }

  logInfo("Logging the user in.");
  try {
    logSuccess(await pma.userLogin(USERNAME, PASSWORD));
  } catch (error) {
    logError(error);
  }
}

main();
