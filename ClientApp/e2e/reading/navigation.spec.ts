import { expect, test } from "../fixtures/authenticatedTest";

import { gotoHome, navigateViaSidebar } from "../fixtures/navigation";
import { gotoBooks, gotoReadingDashboard, gotoReadingHistory } from "../fixtures/reading";

test.describe("reading navigation", () => {
  test("sidebar reaches reading pages", async ({ page }) => {
    await gotoHome(page);

    await navigateViaSidebar(page, "Reading", "Dashboard");
    await expect(page).toHaveURL(/\/reading\/dashboard/);

    await navigateViaSidebar(page, "Reading", "Books");
    await expect(page).toHaveURL(/\/reading\/books/);

    await navigateViaSidebar(page, "Reading", "History");
    await expect(page).toHaveURL(/\/reading\/history/);
  });

  test("home reading section links to dashboard", async ({ page }) => {
    await gotoHome(page);

    await expect(page.getByText("Reading", { exact: true }).first()).toBeVisible();
    await page.getByRole("link", { name: "Open dashboard" }).click();
    await expect(page).toHaveURL(/\/reading\/dashboard/);
  });

  test("direct routes render page titles", async ({ page }) => {
    await gotoBooks(page);
    await expect(page.locator(".text-3xl").filter({ hasText: "Books" })).toBeVisible();

    await gotoReadingDashboard(page);
    await expect(
      page.locator(".text-3xl").filter({ hasText: "Reading dashboard" }),
    ).toBeVisible();

    await gotoReadingHistory(page);
    await expect(
      page.locator(".text-3xl").filter({ hasText: "Reading history" }),
    ).toBeVisible();
  });
});
