import {
  expect,
  Page,
  request,
  type APIRequestContext,
  type BrowserContext,
} from "@playwright/test";
import { randomUUID } from "crypto";

import {
  apiHost,
  apiUrl,
  assertE2eCredentials,
  baseURL,
  e2eEmail,
  e2eFirstName,
  e2eLastName,
  e2ePassword,
  type E2eUserCredentials,
  validSignUpPassword,
} from "../env";

export {
  assertE2eCredentials,
  credentialsForSpecFile,
  e2eEmail,
  e2ePassword,
  validSignUpPassword,
  type E2eUserCredentials,
} from "../env";

export const E2E_USER_STORAGE_KEY = "e2eUserEmail";

const provisionedUsers = new Map<string, Promise<void>>();

function parseRefreshTokenValue(setCookieHeaders: string[]) {
  for (const header of setCookieHeaders) {
    const match = header.match(/^refreshToken=([^;]+)/);
    if (match) {
      return decodeURIComponent(match[1]);
    }
  }

  return null;
}

function parseCookieExpiry(setCookieHeaders: string[]) {
  for (const header of setCookieHeaders) {
    const match = header.match(/expires=([^;]+)/i);
    if (match) {
      return Math.floor(new Date(match[1]).getTime() / 1000);
    }
  }

  return Math.floor(Date.now() / 1000) + 7 * 24 * 60 * 60;
}

function getSetCookieHeaders(response: {
  headersArray: () => Array<{ name: string; value: string }>;
}) {
  return response
    .headersArray()
    .filter((header) => header.name.toLowerCase() === "set-cookie")
    .map((header) => header.value);
}

function shouldSkipAuthInjection(url: string) {
  return (
    url.includes("/api/auth/login") ||
    url.includes("/api/auth/register") ||
    url.includes("/api/auth/refresh-token")
  );
}

async function installApiAuthRoute(context: BrowserContext, accessToken: string) {
  await context.route(`${apiUrl}/api/**`, async (route) => {
    const url = route.request().url();

    if (shouldSkipAuthInjection(url)) {
      await route.continue();
      return;
    }

    const headers = { ...route.request().headers() };
    if (!headers.authorization && !headers.Authorization) {
      headers.authorization = `Bearer ${accessToken}`;
    }

    await route.continue({ headers });
  });
}

type AuthenticatedApiSession = {
  apiContext: APIRequestContext;
  accessToken: string;
  deviceId: string;
  refreshCookie: string;
  cookieExpires: number;
};

export async function ensureTestUser(credentials: E2eUserCredentials) {
  const existing = provisionedUsers.get(credentials.email);
  if (existing) {
    return existing;
  }

  const promise = provisionTestUser(credentials);
  provisionedUsers.set(credentials.email, promise);
  return promise;
}

async function provisionTestUser(credentials: E2eUserCredentials) {
  const apiContext = await request.newContext({ baseURL: apiUrl });

  try {
    let loginResponse = await apiContext.post("/api/auth/login", {
      data: {
        email: credentials.email,
        password: credentials.password,
        deviceId: randomUUID(),
      },
    });

    if (loginResponse.ok()) {
      return;
    }

    const registerResponse = await apiContext.post("/api/auth/register", {
      data: {
        email: credentials.email,
        password: credentials.password,
        firstName: credentials.firstName,
        lastName: credentials.lastName,
      },
    });

    if (!registerResponse.ok() && registerResponse.status() !== 400) {
      const body = await registerResponse.text();
      throw new Error(
        `Failed to register e2e user ${credentials.email} (${registerResponse.status()}): ${body}`,
      );
    }

    loginResponse = await apiContext.post("/api/auth/login", {
      data: {
        email: credentials.email,
        password: credentials.password,
        deviceId: randomUUID(),
      },
    });

    if (!loginResponse.ok()) {
      const body = await loginResponse.text();
      throw new Error(
        `Failed to login e2e user ${credentials.email} (${loginResponse.status()}): ${body}`,
      );
    }
  } finally {
    await apiContext.dispose();
  }
}

async function createAuthenticatedApiSession(
  credentials: E2eUserCredentials,
): Promise<AuthenticatedApiSession> {
  await ensureTestUser(credentials);

  const deviceId = randomUUID();
  let lastError: Error | undefined;

  for (let attempt = 0; attempt < 5; attempt++) {
    try {
      const apiContext = await request.newContext({ baseURL: apiUrl });
      const loginResponse = await apiContext.post("/api/auth/login", {
        data: {
          email: credentials.email,
          password: credentials.password,
          deviceId,
        },
      });

      if (!loginResponse.ok()) {
        const body = await loginResponse.text();
        throw new Error(
          `Authenticated context setup failed with status ${loginResponse.status()}: ${body}`,
        );
      }

      const loginSetCookieHeaders = getSetCookieHeaders(loginResponse);

      const refreshTokenValue = parseRefreshTokenValue(loginSetCookieHeaders);
      if (!refreshTokenValue) {
        throw new Error(
          "Authenticated context setup succeeded but no refreshToken cookie was returned",
        );
      }

      const refreshResponse = await apiContext.post("/api/auth/refresh-token", {
        data: { deviceId },
        headers: {
          Cookie: `refreshToken=${refreshTokenValue}`,
        },
      });

      if (!refreshResponse.ok()) {
        const body = await refreshResponse.text();
        throw new Error(
          `Authenticated context refresh failed with status ${refreshResponse.status()}: ${body}`,
        );
      }

      const accessToken = (await refreshResponse.json()).tokenValue as string;
      const refreshSetCookieHeaders = getSetCookieHeaders(refreshResponse);
      const refreshCookie =
        parseRefreshTokenValue(refreshSetCookieHeaders) ?? refreshTokenValue;

      return {
        apiContext,
        accessToken,
        deviceId,
        refreshCookie,
        cookieExpires: parseCookieExpiry(
          refreshSetCookieHeaders.length > 0
            ? refreshSetCookieHeaders
            : loginSetCookieHeaders,
        ),
      };
    } catch (error) {
      lastError = error as Error;
      await new Promise((resolve) => setTimeout(resolve, 1000 * (attempt + 1)));
    }
  }

  throw lastError ?? new Error("Failed to authenticate e2e context");
}

export async function getPageE2eCredentials(
  page: Page,
): Promise<E2eUserCredentials> {
  assertE2eCredentials();

  if (!page.url().startsWith(baseURL)) {
    await page.goto("/home");
    await page.waitForLoadState("networkidle");
  }

  const email = await page.evaluate((storageKey) => {
    return localStorage.getItem(storageKey);
  }, E2E_USER_STORAGE_KEY);

  if (!email) {
    throw new Error(
      "Missing per-spec e2e user on page. Use authenticatedTest fixture.",
    );
  }

  return {
    email,
    password: e2ePassword!,
    firstName: e2eFirstName,
    lastName: e2eLastName,
  };
}

export async function createAuthenticatedApiContext(
  credentials: E2eUserCredentials,
) {
  const session = await createAuthenticatedApiSession(credentials);

  return {
    apiContext: session.apiContext,
    accessToken: session.accessToken,
    authHeaders: {
      Authorization: `Bearer ${session.accessToken}`,
    },
    async dispose() {
      await session.apiContext.dispose();
    },
  };
}

export async function setupAuthenticatedContext(
  context: BrowserContext,
  credentials: E2eUserCredentials,
) {
  const session = await createAuthenticatedApiSession(credentials);

  await session.apiContext.dispose();

  await context.addCookies([
    {
      name: "refreshToken",
      value: session.refreshCookie,
      domain: apiHost,
      path: "/",
      expires: session.cookieExpires,
      httpOnly: true,
      secure: false,
      sameSite: "Lax",
    },
  ]);

  await context.addInitScript(
    ({ storedDeviceId, storedEmail, storageKey }) => {
      localStorage.setItem("deviceId", storedDeviceId);
      localStorage.setItem(storageKey, storedEmail);
    },
    {
      storedDeviceId: session.deviceId,
      storedEmail: credentials.email,
      storageKey: E2E_USER_STORAGE_KEY,
    },
  );

  await installApiAuthRoute(context, session.accessToken);
}
export async function gotoLoginPage(page: Page) {
  await page.goto("/auth?authMode=logIn");
}

export async function gotoSignUpPage(page: Page) {
  await page.goto("/auth?authMode=singUp");
}

export async function fillLoginForm(
  page: Page,
  email: string,
  password: string,
) {
  await page.locator("#log-in-email").fill(email);
  await page.locator("#log-in-password").fill(password);
}

export async function submitLogin(page: Page) {
  const loginResponse = page.waitForResponse(
    (response) =>
      response.url().includes("/api/auth/login") &&
      response.request().method() === "POST",
  );

  await page.getByRole("button", { name: "Log In", exact: true }).click();

  return loginResponse;
}

export async function login(page: Page, email = e2eEmail!, password = e2ePassword!) {
  assertE2eCredentials();
  await gotoLoginPage(page);
  await fillLoginForm(page, email, password);

  const response = await submitLogin(page);
  expect(response.ok(), `Login failed with status ${response.status()}`).toBe(
    true,
  );

  await expect(page).toHaveURL(/\/home/);
}

export async function logout(page: Page) {
  assertE2eCredentials();

  await page.getByRole("button", { name: e2eEmail! }).click();
  await page.getByRole("menuitem", { name: "Log out" }).click();

  await expect(page).toHaveURL(/\/auth/);
  await expect(page).toHaveURL(/authMode=logIn/);
}

type SignUpDetails = {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
};

export async function fillSignUpForm(page: Page, details: SignUpDetails) {
  await page.locator("#email").fill(details.email);
  await page.locator("#password").fill(details.password);

  await page.getByRole("button", { name: "Next slide" }).click();

  await page.locator("#confirmPassword").fill(details.confirmPassword);
  await page.locator("#firstName").fill(details.firstName);

  await page.getByRole("button", { name: "Next slide" }).click();

  await page.locator("#lastName").fill(details.lastName);
}

export async function submitSignUp(page: Page) {
  const registerResponse = page.waitForResponse(
    (response) =>
      response.url().includes("/api/auth/register") &&
      response.request().method() === "POST",
  );

  await page.getByRole("button", { name: "Sign Up", exact: true }).click();

  return registerResponse;
}

export function defaultSignUpDetails(email: string): SignUpDetails {
  return {
    email,
    password: validSignUpPassword,
    confirmPassword: validSignUpPassword,
    firstName: "E2E",
    lastName: "Signup",
  };
}
