import { expect, test } from "../fixtures/authenticatedTest";

import {
  e2eEmail,
  e2eFirstName,
  e2eLastName,
  e2ePassword,
  type E2eUserCredentials,
} from "../env";
import { ensureTestUser } from "../fixtures/auth";
import { expectPageTitle } from "../fixtures/navigation";

const sharedBillingUser: E2eUserCredentials = {
  email: e2eEmail!,
  password: e2ePassword!,
  firstName: e2eFirstName,
  lastName: e2eLastName,
};

test.describe("billing smoke", () => {
  test.use({
    e2eUser: async ({}, use) => {
      await ensureTestUser(sharedBillingUser);
      await use(sharedBillingUser);
    },
  });

  test("billing page tabs switch", async ({ page }) => {
    await page.goto("/billing");

    await expectPageTitle(page, "Billing");
    await page.getByRole("tab", { name: "Invoices" }).click();
    await page.getByRole("tab", { name: "Usage" }).click();
    await page.getByRole("tab", { name: "Overview" }).click();
  });

  test("upgrade page loads", async ({ page }) => {
    await page.goto("/upgrade");
    await expect(page.getByRole("heading").first()).toBeVisible();
  });

  test("billing overview shows current plan details", async ({ page }) => {
    await page.goto("/billing");

    await expect(page.getByText("Current Plan")).toBeVisible();
    await expect(page.getByText("Free", { exact: true })).toBeVisible();
    await expect(
      page.getByText("Upgrade to Pro to unlock all features."),
    ).toBeVisible();
  });

  test("subscription success page loads", async ({ page }) => {
    await page.goto("/subscription-success");

    await expect(page.getByText("You're Pro!")).toBeVisible();
    await expect(
      page.getByText("Thank you for upgrading. Your subscription is now active."),
    ).toBeVisible();
    await expect(
      page.getByRole("link", { name: "Manage subscription" }),
    ).toBeVisible();
    await expect(page.getByRole("link", { name: "Go to Home" })).toBeVisible();
  });
});
