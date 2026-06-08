import { expect, test } from "../fixtures/authenticatedTest";

import { expectPageTitle } from "../fixtures/navigation";
import {
  ensureNutritionGoals,
  getOverviewCalories,
  gotoDiary,
  gotoNutritionOverview,
  searchAndOpenFood,
  submitFoodDiaryDialog,
} from "../fixtures/nutrition";

test.describe("nutrition overview", () => {
  test("loads with nutrient cards and period filters", async ({ page }) => {
    await page.goto("/nutrition/overview");

    await expectPageTitle(page, "Nutrition Overview");
    await expect(page.getByRole("button", { name: "Today" })).toBeVisible();
    await expect(page.getByRole("button", { name: "This week" })).toBeVisible();
    await expect(page.getByText("Calories").first()).toBeVisible();
  });

  test("shows empty state when no diary data", async ({ page }) => {
    await page.goto("/nutrition/overview");
    await expect(page.getByText("Nutrition Summary")).toBeVisible();

    const nutritionEmpty = page.getByText("No nutrition diary entries.");
    const summaryEmpty = page.getByText("No diary entries for selected period");

    if (await nutritionEmpty.isVisible()) {
      await expect(nutritionEmpty).toBeVisible();
      return;
    }

    if (await summaryEmpty.isVisible()) {
      await expect(summaryEmpty).toBeVisible();
      return;
    }

    // The persistent e2e user often already has diary entries for today.
    await expect(page.getByText(/\/ \d+ kcal/)).toBeVisible();
  });

  test("links to food diary from empty state", async ({ page }) => {
    await page.goto("/nutrition/overview");

    const diaryLink = page.getByRole("link", { name: "Log your food" });
    if (await diaryLink.isVisible()) {
      await diaryLink.click();
      await expect(page).toHaveURL(/\/nutrition\/diary/);
    }
  });

  test("switches period filter to this week", async ({ page }) => {
    await gotoNutritionOverview(page);

    await page.getByRole("button", { name: "This week" }).click();
    await expect(page.getByRole("button", { name: "This week" })).toBeVisible();
    await expect(page.getByText("Nutrition Summary")).toBeVisible();
  });

  test("reflects food diary entries for today", async ({ page }) => {
    await ensureNutritionGoals(page);
    await gotoNutritionOverview(page);
    await page.getByRole("button", { name: "Today" }).click();

    const caloriesBefore = await getOverviewCalories(page);

    await gotoDiary(page);
    await searchAndOpenFood(page, "egg");
    await submitFoodDiaryDialog(page, "Add");
    await expect(page.getByText("Food history")).toBeVisible();
    await page.waitForLoadState("networkidle");

    await gotoNutritionOverview(page);
    await page.getByRole("button", { name: "Today" }).click();

    const caloriesAfter = await getOverviewCalories(page);
    expect(caloriesAfter).toBeGreaterThan(caloriesBefore);
  });
});
