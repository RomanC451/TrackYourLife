import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";
import dotenv from "dotenv";

const clientAppRoot = path.resolve(
  path.dirname(fileURLToPath(import.meta.url)),
  "..",
);

function stripQuotes(value: string | undefined): string | undefined {
  return value?.trim().replace(/^["']|["']$/g, "");
}

function loadEnvFile(filename: string) {
  const filePath = path.join(clientAppRoot, filename);
  if (fs.existsSync(filePath)) {
    dotenv.config({ path: filePath });
  }
}

loadEnvFile(".env");
loadEnvFile(".env.e2e");

export const e2eEmail =
  stripQuotes(process.env.E2E_EMAIL) ??
  stripQuotes(process.env.VITE_DEV_EMAIL);

export const e2ePassword =
  stripQuotes(process.env.E2E_PASSWORD) ??
  stripQuotes(process.env.VITE_DEV_PASSWORD);

export const e2eFirstName =
  stripQuotes(process.env.E2E_FIRST_NAME) ??
  stripQuotes(process.env.VITE_DEV_FIRST_NAME) ??
  "E2E";

export const e2eLastName =
  stripQuotes(process.env.E2E_LAST_NAME) ??
  stripQuotes(process.env.VITE_DEV_LAST_NAME) ??
  "User";

export const viteHost = stripQuotes(process.env.E2E_VITE_HOST) ?? "127.0.0.1";

export const apiUrl =
  stripQuotes(process.env.E2E_API_URL) ?? "http://127.0.0.1:5244";

export const apiHost = new URL(apiUrl).hostname;

export const baseURL =
  stripQuotes(process.env.E2E_BASE_URL) ?? `http://${viteHost}:5173`;

/** Meets signUpSchema password rules */
export const validSignUpPassword = "TestPass123!";

export function assertE2eCredentials() {
  if (!e2eEmail || !e2ePassword) {
    throw new Error(
      "Missing e2e credentials. Set E2E_EMAIL/E2E_PASSWORD in ClientApp/.env.e2e " +
        "or VITE_DEV_EMAIL/VITE_DEV_PASSWORD in ClientApp/.env",
    );
  }
}

export function uniqueTestEmail() {
  return `e2e-${Date.now()}-${Math.random().toString(36).slice(2, 8)}@example.com`;
}

export type E2eUserCredentials = {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
};

/** One isolated user per spec file so parallel workers do not share app state. */
export function credentialsForSpecFile(filePath: string): E2eUserCredentials {
  assertE2eCredentials();

  const e2eDir = path.join(clientAppRoot, "e2e");
  const slug = path
    .relative(e2eDir, filePath)
    .replace(/\\/g, "/")
    .replace(/\.spec\.ts$/, "")
    .replace(/[^a-zA-Z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .toLowerCase();

  const domain = e2eEmail!.split("@")[1];

  return {
    email: `e2e-${slug}@${domain}`,
    password: e2ePassword!,
    firstName: e2eFirstName,
    lastName: e2eLastName,
  };
}
