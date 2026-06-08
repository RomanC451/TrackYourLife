import { expect, test } from "../fixtures/authenticatedTest";

test.describe("youtube search", () => {
  test("shows empty search state", async ({ page }) => {
    await page.goto("/youtube/search");

    await expect(page.getByPlaceholder("Search for videos...")).toBeVisible();
    await expect(page.getByText("Search for Videos")).toBeVisible();
  });

  test("runs a video search and shows results or no-results state", async ({
    page,
  }) => {
    test.setTimeout(60_000);

    await page.goto("/youtube/search");

    const searchInput = page.getByPlaceholder("Search for videos...");
    await searchInput.fill("workout");

    const searchResponse = page.waitForResponse(
      (response) =>
        response.url().includes("/api/videos/search") && response.ok(),
      { timeout: 45_000 },
    );
    await searchResponse;

    const resultCard = page.locator("[class*='cursor-pointer']").first();
    const noResults = page.getByText(/No videos found|no results/i);

    if (await resultCard.isVisible()) {
      await expect(resultCard).toBeVisible();
      return;
    }

    await expect(noResults).toBeVisible();
  });
});
