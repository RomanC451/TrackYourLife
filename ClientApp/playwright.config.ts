import path from "path";
import { fileURLToPath } from "url";
import { defineConfig, devices } from "@playwright/test";

import {
  apiHost,
  apiUrl,
  baseURL,
  e2eEmail,
  e2eFirstName,
  e2eLastName,
  e2ePassword,
  viteHost,
} from "./e2e/env";

const clientAppRoot = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(clientAppRoot, "..");

const webServer = [];

if (process.env.E2E_START_API !== "false") {
  webServer.push({
    command:
      "dotnet run --project App/TrackYourLife.App.csproj --launch-profile testReact",
    url: `${apiUrl}/swagger/index.html`,
    reuseExistingServer: !process.env.CI,
    cwd: repoRoot,
    timeout: 180_000,
    env: {
      FeatureFlags__SkipEmailVerification: "true",
      RefreshTokenCookie__Domain: apiHost,
      RefreshTokenCookie__Secure: "false",
      RefreshTokenCookie__SameSite: "Lax",
    },
  });
}

if (process.env.E2E_START_FRONTEND !== "false") {
  webServer.push({
    command: "npm run dev",
    url: baseURL,
    reuseExistingServer: !process.env.CI,
    env: {
      VITE_HOST: viteHost,
      VITE_API_PATH: apiUrl,
      VITE_HIDE_TOOLS: "true",
      ...(e2eEmail ? { VITE_DEV_EMAIL: e2eEmail } : {}),
      ...(e2ePassword ? { VITE_DEV_PASSWORD: e2ePassword } : {}),
      ...(e2eFirstName ? { VITE_DEV_FIRST_NAME: e2eFirstName } : {}),
      ...(e2eLastName ? { VITE_DEV_LAST_NAME: e2eLastName } : {}),
    },
    timeout: 90_000,
  });
}

const desktopChrome = devices["Desktop Chrome"];

export default defineConfig({
  testDir: "./e2e",
  globalSetup: "./e2e/global-setup.ts",
  timeout: 30_000,
  // Each spec file has its own user; keep tests in a file serial on that account.
  fullyParallel: false,
  workers: process.env.CI ? 4 : undefined,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  reporter: process.env.CI
    ? [["html", { open: "never" }], ["github"]]
    : [["list"], ["html", { open: "never" }]],
  use: {
    baseURL,
    trace: "on-first-retry",
  },
  projects: [
    {
      name: "app",
      use: { ...desktopChrome },
      testIgnore: /auth\.spec\.ts/,
    },
    {
      name: "auth",
      use: {
        ...desktopChrome,
        storageState: { cookies: [], origins: [] },
      },
      testMatch: /auth\.spec\.ts/,
    },
  ],
  webServer,
});
