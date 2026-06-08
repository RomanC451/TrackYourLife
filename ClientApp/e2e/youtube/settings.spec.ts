import { expect, test } from "../fixtures/authenticatedTest";

import { expectPageTitle } from "../fixtures/navigation";

test.describe("youtube settings", () => {
  test("loads categories section", async ({ page }) => {
    await page.goto("/youtube/settings");

    await expectPageTitle(page, "YouTube Settings");
    await expect(page.getByRole("heading", { name: "Categories" })).toBeVisible();
    await expect(page.getByRole("button", { name: /Add category/i })).toBeVisible();
  });

  test("shows settings lock section", async ({ page }) => {
    await page.goto("/youtube/settings");

    await expect(page.getByRole("heading", { name: "Settings lock" })).toBeVisible();
  });
});
