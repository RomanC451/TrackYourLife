import { expect, test } from "../fixtures/authenticatedTest";

import {
  createRecipe,
  ensureNutritionGoals,
  gotoDiary,
  hasGoalsOverlay,
  searchAndAddRecipeToDiary,
  searchAndOpenFood,
  submitFoodDiaryDialog,
  switchDiarySearchTo,
} from "../fixtures/nutrition";
import { uniqueSuffix } from "../fixtures/helpers";

test.describe("nutrition diary", () => {
  test.describe.configure({ mode: "serial" });
  test("shows goals overlay when goals are not set", async ({ page }) => {
    await gotoDiary(page);
    await expect(page.getByText("Food Diary")).toBeVisible();

    if (await hasGoalsOverlay(page)) {
      await expect(
        page.getByText("Goals are not defined. Calculate them first."),
      ).toBeVisible();
      await expect(page.getByPlaceholder("Search food...")).toBeDisabled();
    } else {
      await expect(page.getByPlaceholder("Search food...")).toBeEnabled();
    }
  });

  test("sets goals via calculator and enables food search", async ({ page }) => {
    await ensureNutritionGoals(page);
    await expect(page.getByPlaceholder("Search food...")).toBeEnabled();
  });

  test("adds a food entry to the diary", async ({ page }) => {
    await ensureNutritionGoals(page);

    await searchAndOpenFood(page, "egg");
    await submitFoodDiaryDialog(page, "Add");

    await expect(page.getByText("Food history")).toBeVisible();
    await expect(page.getByText("(F)", { exact: false }).first()).toBeVisible();
  });

  test("edits a food diary entry", async ({ page }) => {
    await ensureNutritionGoals(page);

    await searchAndOpenFood(page, "egg");
    await submitFoodDiaryDialog(page, "Add");

    const row = page.getByRole("row").filter({ hasText: "(F)" }).first();
    await row.getByRole("button", { name: "Open menu" }).click();
    await page.getByRole("menuitem", { name: "Edit" }).click();

    await expect(page.getByRole("heading", { name: "Edit food diary" })).toBeVisible();

    const dialog = page.getByRole("dialog", { name: "Edit food diary" });
    await dialog.getByRole("textbox").first().fill("3");
    await submitFoodDiaryDialog(page, "Save");

    await expect(page.getByText("3 Serving")).toBeVisible();
  });

  test("deletes a food diary entry", async ({ page }) => {
    await ensureNutritionGoals(page);

    await searchAndOpenFood(page, "egg");
    await submitFoodDiaryDialog(page, "Add");

    const row = page.getByRole("row").filter({ hasText: "(F)" }).last();
    await row.getByRole("button", { name: "Open menu" }).click();

    const deleteResponse = page.waitForResponse(
      (response) =>
        response.url().includes("/api/food-diaries") &&
        response.request().method() === "DELETE",
    );
    await page.getByRole("menuitem", { name: "Remove" }).click();
    await deleteResponse;
  });

  test("switches between foods and recipes search", async ({ page }) => {
    await ensureNutritionGoals(page);

    await switchDiarySearchTo(page, "Recipes");
    await expect(page.getByPlaceholder("Search recipe...")).toBeVisible();

    await switchDiarySearchTo(page, "Foods");
    await expect(page.getByPlaceholder("Search food...")).toBeEnabled();
  });

  test("adds a recipe entry to the diary", async ({ page }) => {
    const recipeName = `E2E Recipe ${uniqueSuffix()}`;

    await createRecipe(page, recipeName);
    await ensureNutritionGoals(page);
    await searchAndAddRecipeToDiary(page, recipeName);

    await expect(page.getByText("Food history")).toBeVisible();
    await expect(page.getByText("(R)", { exact: false }).first()).toBeVisible();
    await expect(page.getByRole("row").filter({ hasText: recipeName })).toBeVisible();
  });

  test("changes diary date in the table", async ({ page }) => {
    await ensureNutritionGoals(page);

    const dateButton = page
      .locator("button")
      .filter({ has: page.locator(".lucide-calendar") })
      .first();
    const initialLabel = (await dateButton.textContent()) ?? "";

    await dateButton.click();
    const diaryResponse = page.waitForResponse(
      (response) =>
        response.url().includes("/api/nutrition-diaries/") &&
        response.request().method() === "GET" &&
        response.ok(),
    );
    await page.getByRole("gridcell", { name: "1", exact: true }).first().click();
    await diaryResponse;

    await expect(dateButton).not.toHaveText(initialLabel);
  });
});
