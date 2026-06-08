import { expect, test } from "@playwright/test";

import { uniqueTestEmail } from "./env";
import {
  assertE2eCredentials,
  defaultSignUpDetails,
  e2eEmail,
  e2ePassword,
  fillLoginForm,
  fillSignUpForm,
  gotoLoginPage,
  gotoSignUpPage,
  login,
  logout,
  submitLogin,
  submitSignUp,
  validSignUpPassword,
} from "./fixtures/auth";

test.describe("authentication", () => {
  test.describe("login", () => {
    test.beforeEach(assertE2eCredentials);

    test("logs in with valid credentials and redirects to home", async ({
      page,
    }) => {
      await gotoLoginPage(page);
      await fillLoginForm(page, e2eEmail!, e2ePassword!);

      const response = await submitLogin(page);
      expect(
        response.ok(),
        `Login failed with status ${response.status()}`,
      ).toBe(true);

      await expect(page).toHaveURL(/\/home/);
    });

    test("shows validation errors when required fields are empty", async ({
      page,
    }) => {
      await gotoLoginPage(page);

      await page.locator("#log-in-email").clear();
      await page.locator("#log-in-password").clear();
      await page.getByRole("button", { name: "Log In", exact: true }).click();

      await expect(page.getByText("Email required.")).toBeVisible();
      await expect(page.getByText("Password required.")).toBeVisible();
    });

    test("shows an error for invalid credentials", async ({ page }) => {
      await gotoLoginPage(page);
      await fillLoginForm(page, e2eEmail!, "WrongPassword123!");

      const response = await submitLogin(page);
      expect(response.status()).toBe(400);

      await expect(page.getByText("Invalid credentials")).toBeVisible();
      await expect(page).toHaveURL(/\/auth/);
    });

    test("redirects to the requested page after login", async ({ page }) => {
      await page.goto(
        "/auth?authMode=logIn&redirect=%2Fnutrition%2Foverview",
      );

      await fillLoginForm(page, e2eEmail!, e2ePassword!);

      const response = await submitLogin(page);
      expect(response.ok()).toBe(true);

      await expect(page).toHaveURL(/\/nutrition\/overview/);
    });
  });

  test.describe("route protection", () => {
    test("redirects unauthenticated users to login with a return URL", async ({
      page,
    }) => {
      await page.goto("/home");

      await expect(page).toHaveURL(/\/auth/);
      await expect(page).toHaveURL(/authMode=logIn/);
      await expect(page).toHaveURL(/redirect=/);
    });
  });

  test.describe("logout", () => {
    test.beforeEach(assertE2eCredentials);

    test("logs out and returns to the login page", async ({ page }) => {
      await login(page);
      await logout(page);

      await expect(page.locator("#log-in-email")).toBeVisible();
    });

    test("blocks access to protected routes after logout", async ({ page }) => {
      await login(page);
      await logout(page);

      await page.goto("/home");

      await expect(page).toHaveURL(/\/auth/);
    });
  });

  test.describe("auth mode switching", () => {
    test("switches between login and sign up modes", async ({ page }) => {
      await gotoLoginPage(page);

      await page
        .getByRole("button", { name: "I don't have an account." })
        .click();
      await expect(page).toHaveURL(/authMode=singUp/);
      await expect(page.locator("#email")).toBeVisible();

      await page
        .getByRole("button", { name: "I already have an account." })
        .click();
      await expect(page).toHaveURL(/authMode=logIn/);
      await expect(page.locator("#log-in-email")).toBeVisible();
    });
  });

  test.describe("sign up", () => {
    test("shows client-side validation for a weak password", async ({
      page,
    }) => {
      await gotoSignUpPage(page);

      await page.locator("#email").fill(uniqueTestEmail());
      await page.locator("#password").fill("short");
      await page.getByRole("button", { name: "Sign Up", exact: true }).click();

      await expect(
        page.getByText("Password must be at least 10 characters"),
      ).toBeVisible();
    });

    test("shows an error when passwords do not match", async ({ page }) => {
      await gotoSignUpPage(page);

      await page.locator("#email").fill(uniqueTestEmail());
      await page.locator("#password").fill(validSignUpPassword);
      await page.getByRole("button", { name: "Next slide" }).click();

      await page.locator("#confirmPassword").fill("DifferentPass123!");
      await page.locator("#firstName").fill("E2E");
      await page.getByRole("button", { name: "Next slide" }).click();
      await page.locator("#lastName").fill("User");
      await page.getByRole("button", { name: "Sign Up", exact: true }).click();

      await expect(page.getByText("Passwords do not match.")).toBeVisible();
    });

    test("registers a new account and returns to login", async ({ page }) => {
      const email = uniqueTestEmail();

      await gotoSignUpPage(page);
      await fillSignUpForm(page, defaultSignUpDetails(email));

      const response = await submitSignUp(page);
      expect(
        response.status(),
        `Registration failed with status ${response.status()}`,
      ).toBe(201);

      await expect(page.getByText("Account created successfully.")).toBeVisible();
      await expect(page).toHaveURL(/authMode=logIn/);
    });

    test("shows an error when registering with an existing email", async ({
      page,
    }) => {
      assertE2eCredentials();

      await gotoSignUpPage(page);
      await fillSignUpForm(page, defaultSignUpDetails(e2eEmail!));

      const response = await submitSignUp(page);
      expect(response.status()).toBe(400);

      await expect(page).toHaveURL(/authMode=singUp/);
    });
  });

  test.describe("email verification", () => {
    test("shows an error page for an invalid verification token", async ({
      page,
    }) => {
      await page.goto("/email-verification?token=invalid-e2e-token");

      await expect(page).toHaveURL(/\/error/);
      await expect(
        page.getByText("Ooops... something went wrong"),
      ).toBeVisible();
    });
  });
});
