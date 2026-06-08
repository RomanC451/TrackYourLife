import {
  assertE2eCredentials,
  e2eEmail,
  e2eFirstName,
  e2eLastName,
  e2ePassword,
} from "./env";
import { waitForApi } from "./fixtures/api";
import { ensureTestUser } from "./fixtures/auth";

async function globalSetup() {
  await waitForApi();

  assertE2eCredentials();
  await ensureTestUser({
    email: e2eEmail!,
    password: e2ePassword!,
    firstName: e2eFirstName,
    lastName: e2eLastName,
  });
}

export default globalSetup;
