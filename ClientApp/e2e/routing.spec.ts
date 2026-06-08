import { expect, test } from "./fixtures/authenticatedTest";

test.describe("routing", () => {
  test("shows not found page for unknown routes", async ({ page }) => {
    await page.goto("/this-route-does-not-exist");

    await expect(page).toHaveURL(/\/not-found/);
    await expect(page.getByText("Page not found")).toBeVisible();
    await expect(page.getByRole("button", { name: "Go back" })).toBeVisible();
  });
});
