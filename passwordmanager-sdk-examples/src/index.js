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

// The log functions are mostly irrelevant, just fancy `console.log()`'s.
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

function logNeutral(output) {
  logBase("", "", output);
}

//////////////////// Main ////////////////////

async function main() {
  // Create a PasswordManagerApi instance.
  const pma = new PasswordManagerApi("http://localhost:5086");

  logInfo("Creating a new user.");
  try {
    await pma.userRegister(USERNAME, OLD_PASSWORD);
    logSuccess("Created a new user.");
  } catch (error) {
    if (error instanceof ApiError) {
      logError(error.message);
    } else {
      throw error;
    }
  }

  logInfo("Logging the user in.");
  try {
    await pma.userLogin(USERNAME, OLD_PASSWORD);
    logSuccess("Logged the user in.");
  } catch (error) {
    if (error instanceof ApiError) {
      logError(error.message);
    } else {
      throw error;
    }
  }

  logInfo("Creating a full password entry.");
  try {
    await pma.vaultCreatePassword({
      name: "Netflix",
      link: "www.netflix.com",
      username: "joe",
      password: "mama",
    });
    logSuccess("Created a full password entry.");
  } catch (error) {
    if (error instanceof ApiError) {
      logError(error.message);
    } else {
      throw error;
    }
  }

  logInfo("Creating a password entry with missing fields.");
  try {
    await pma.vaultCreatePassword({
      link: "www.cornhub.com",
      password: "iLikeBeans",
    });
    logSuccess("Created a password entry with missing fields.");
  } catch (error) {
    if (error instanceof ApiError) {
      logError(error.message);
    } else {
      throw error;
    }
  }

  logInfo("Fetching all password entrys from the user.");
  const passwordEntrys = await pma.vaultListPasswordEntrys();
  const passwordUuidToUpdate = passwordEntrys[1].uuid; // "Remember" for the next step to update and the delete it.
  logSuccess(passwordEntrys);

  logInfo("Change the password entry from before.");
  try {
    await pma.vaultUpdatePasswordEntry(passwordUuidToUpdate, {
      link: "www.github.com",
      password: "iLikeBeans",
    });
    logSuccess("Changed the password entry.");
    logNeutral(await pma.vaultListPasswordEntrys());
  } catch (error) {
    if (error instanceof ApiError) {
      logError(error.message);
    } else {
      throw error;
    }
  }

  logInfo("Delete the password we just changed.");
  try {
    await pma.vaultDeletePasswordEntry(passwordUuidToUpdate);
    logSuccess("Deleted the password entry.");
    logNeutral(await pma.vaultListPasswordEntrys());
  } catch (error) {
    if (error instanceof ApiError) {
      logError(error.message);
    } else {
      throw error;
    }
  }

  logInfo("Changing the users password.");
  try {
    await pma.userChangePassword(OLD_PASSWORD, NEW_PASSWORD);
    logSuccess(
      "Changed the users password, and you can still access the passwords."
    );
    logNeutral(await pma.vaultListPasswordEntrys());
  } catch (error) {
    if (error instanceof ApiError) {
      logError(error.message);
    } else {
      throw error;
    }
  }

  logInfo("Deleting the user and all their passwords.");
  try {
    await pma.userDelete(NEW_PASSWORD);
    logSuccess("Deleted the user and all their passwords.");
  } catch (error) {
    if (error instanceof ApiError) {
      logError(error.message);
    } else {
      throw error;
    }
  }
}

main();
