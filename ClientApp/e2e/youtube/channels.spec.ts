import { expect, test } from "../fixtures/authenticatedTest";

import { expectPageTitle } from "../fixtures/navigation";

test.describe("youtube channels", () => {
  test("opens add channel dialog", async ({ page }) => {
    await page.goto("/youtube/channels");
    await expectPageTitle(page, "Channels");

    await page.getByRole("button", { name: "Add Channel" }).click();

    await expect(page).toHaveURL(/\/youtube\/channels\/add/);
    await expect(
      page.getByRole("heading", { name: "Add YouTube Channel" }),
    ).toBeVisible();
    await expect(
      page.getByPlaceholder("Search for channels..."),
    ).toBeVisible();
  });

  test("closes add channel dialog and returns to channels list", async ({
    page,
  }) => {
    await page.goto("/youtube/channels/add");

    await expect(
      page.getByRole("heading", { name: "Add YouTube Channel" }),
    ).toBeVisible();

    await page.keyboard.press("Escape");

    await expect(page).toHaveURL(/\/youtube\/channels(\?|$)/);
    await expectPageTitle(page, "Channels");
  });
});
