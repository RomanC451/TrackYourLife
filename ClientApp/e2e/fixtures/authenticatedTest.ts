import { test as base, expect } from "@playwright/test";

import { credentialsForSpecFile, type E2eUserCredentials } from "../env";
import { waitForApi } from "./api";
import { ensureTestUser, setupAuthenticatedContext } from "./auth";

export const test = base.extend<{ e2eUser: E2eUserCredentials }>({
  e2eUser: async ({}, use, testInfo) => {
    const credentials = credentialsForSpecFile(testInfo.file);
    await ensureTestUser(credentials);
    await use(credentials);
  },
  context: async ({ context, e2eUser }, use) => {
    await waitForApi();
    await setupAuthenticatedContext(context, e2eUser);
    await use(context);
  },
});

export { expect };
