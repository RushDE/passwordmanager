import { PasswordManagerApi, ApiError } from "passwordmanager-sdk";

//////////////////// "Settings" ////////////////////

const USERNAME = "max";
const OLD_PASSWORD = "hunter2";
const NEW_PASSWORD = "2retnuh"; // Much safer.

//////////////////// Constants ////////////////////

// Source: https://stackoverflow.com/a/41407246/19860801
const COLORS = Object.freeze({
  FG_CYAN: "\x1b[36m",
  FG_GREEN: "\x1b[32m",
  FG_RED: "\x1b[31m",
  CMD_RESET: "\x1b[0m",
});

//////////////////// Functions ////////////////////

function logBase(color, prefix, output) {
  if (typeof output !== "string") {
    output = JSON.stringify(output, null, 4);
  }
  console.log(`${color}${prefix}${COLORS.CMD_RESET}${output}`);
}

function logInfo(output) {
  logBase(COLORS.FG_CYAN, "INFO: ", output);
}

function logSuccess(output) {
  logBase(COLORS.FG_GREEN, "SUCCESS:\n", output);
}

function logError(output) {
  logBase(COLORS.FG_RED, "ERROR:\n", output);
}

//////////////////// Main ////////////////////

async function main() {
  // Create a PasswordManagerApi instance.
  const pma = new PasswordManagerApi("http://localhost:5086");

  logInfo("Creating a new user.");
  try {
    logSuccess(await pma.userRegister(USERNAME, OLD_PASSWORD));
  } catch (error) {
    if (error instanceof ApiError) {
      logError(error.message);
    } else {
      throw error;
    }
  }

  logInfo("Logging the user in.");
  try {
    logSuccess(await pma.userLogin(USERNAME, OLD_PASSWORD));
  } catch (error) {
    if (error instanceof ApiError) {
      logError(error.message);
    } else {
      throw error;
    }
  }

  logInfo("Changing the users password.");
  try {
    logSuccess(await pma.userChangePassword(OLD_PASSWORD, NEW_PASSWORD));
  } catch (error) {
    if (error instanceof ApiError) {
      logError(error.message);
    } else {
      throw error;
    }
  }
}

main();
