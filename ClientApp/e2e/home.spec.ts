import { expect, test } from "./fixtures/authenticatedTest";

import { expectPageTitle } from "./fixtures/navigation";

test.describe("home", () => {
  test("dashboard loads with main sections after login", async ({ page }) => {
    await page.goto("/home");

    await expectPageTitle(page, "Home");
    await expect(page.getByText("Nutrition", { exact: true }).first()).toBeVisible();
    await expect(page.getByText("Trainings", { exact: true }).first()).toBeVisible();
  });

  test("nutrition section links to food diary", async ({ page }) => {
    await page.goto("/home");
    await expect(page.locator(".text-3xl").filter({ hasText: "Home" })).toBeVisible();

    await page.locator('a[href="/nutrition/diary"]').first().click();
    await expect(page).toHaveURL(/\/nutrition\/diary/);
  });

  test("trainings section links to workouts", async ({ page }) => {
    await page.goto("/home");
    await expect(page.locator(".text-3xl").filter({ hasText: "Home" })).toBeVisible();

    await page.getByRole("link", { name: "View workouts" }).click();
    await expect(page).toHaveURL(/\/trainings\/workouts/);
  });

  test("nutrition section shows intake cards or empty state", async ({ page }) => {
    await page.goto("/home");

    await expect(page.getByText("Nutrition", { exact: true }).first()).toBeVisible();
    await expect(page.getByText("Today's intake")).toBeVisible();

    const emptyState = page.getByText("No nutrition diary entries.");
    const caloriesCard = page.getByText("Calories", { exact: true }).first();

    if (await emptyState.isVisible()) {
      await expect(page.getByRole("link", { name: "Log your food" })).toBeVisible();
      return;
    }

    await expect(caloriesCard).toBeVisible();
    await expect(page.getByRole("link", { name: "Food diary" })).toBeVisible();
  });

  test("youtube section loads with favorites heading", async ({ page }) => {
    await page.goto("/home");

    await expect(page.getByText("From your favorites")).toBeVisible();
    await expect(page.getByRole("link", { name: "All videos" })).toBeVisible();
  });
});
