import { expect, test } from "../fixtures/authenticatedTest";

import { expectPageTitle } from "../fixtures/navigation";

test.describe("youtube smoke", () => {
  test("channels page loads", async ({ page }) => {
    await page.goto("/youtube/channels");
    await expectPageTitle(page, "Channels");
  });

  test("videos page loads", async ({ page }) => {
    await page.goto("/youtube/videos");
    await expectPageTitle(page, "Videos");
  });

  test("library page loads", async ({ page }) => {
    await page.goto("/youtube/library");
    await expect(page.getByText("Library").first()).toBeVisible();
  });

  test("search page loads", async ({ page }) => {
    await page.goto("/youtube/search");
    await expect(
      page.getByPlaceholder("Search for videos..."),
    ).toBeVisible();
  });

  test("history page loads", async ({ page }) => {
    await page.goto("/youtube/history");
    await expectPageTitle(page, "Watch history");
  });

  test("settings page loads", async ({ page }) => {
    await page.goto("/youtube/settings");
    await expectPageTitle(page, "YouTube Settings");
  });
});

