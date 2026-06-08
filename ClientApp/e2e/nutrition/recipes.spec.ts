import { expect, test } from "../fixtures/authenticatedTest";

import { uniqueSuffix } from "../fixtures/helpers";
import {
  clickFirstFoodSearchResult,
  createRecipe,
  gotoRecipesList,
  openRecipeRowMenu,
} from "../fixtures/nutrition";

test.describe("nutrition recipes", () => {
  test("creates a new recipe", async ({ page }) => {
    const recipeName = `E2E Recipe ${uniqueSuffix()}`;

    await createRecipe(page, recipeName);
    await gotoRecipesList(page);
    await expect(page.getByRole("row").filter({ hasText: recipeName })).toBeVisible();
  });

  test("edits recipe details", async ({ page }) => {
    const recipeName = `E2E Recipe ${uniqueSuffix()}`;
    const updatedName = `${recipeName} Updated`;

    await createRecipe(page, recipeName);
    await gotoRecipesList(page);

    await openRecipeRowMenu(page, recipeName);
    await page.getByRole("menuitem", { name: "Edit" }).click();

    await page.locator("#create-recipe-name").fill(updatedName);

    const response = page.waitForResponse(
      (response) =>
        response.url().includes("/api/recipes") &&
        response.request().method() === "PUT" &&
        response.ok(),
    );
    await page.getByRole("button", { name: "Update and Next", exact: true }).click();
    await response;

    await gotoRecipesList(page);
    await expect(page.getByRole("row").filter({ hasText: updatedName })).toBeVisible();
  });

  test("adds an ingredient to a recipe", async ({ page }) => {
    const recipeName = `E2E Recipe ${uniqueSuffix()}`;

    await createRecipe(page, recipeName);

    const dialog = page.getByRole("dialog");
    await dialog.getByRole("tab", { name: "Ingredients" }).click();

    const searchInput = dialog.getByPlaceholder("Search for ingredients...");
    await searchInput.click();
    await searchInput.fill("egg");

    const searchResponse = page.waitForResponse(
      (response) =>
        response.url().includes("/api/foods/search") && response.ok(),
      { timeout: 30_000 },
    );
    await page.waitForTimeout(1200);
    await searchResponse;

    await clickFirstFoodSearchResult(page, dialog);

    const ingredientDialog = page.getByRole("dialog", {
      name: "Create a new ingredient",
    });
    await expect(ingredientDialog).toBeVisible();

    const response = page.waitForResponse(
      (response) =>
        response.url().includes("/ingredients") &&
        response.request().method() === "POST",
    );
    await ingredientDialog
      .getByRole("button", { name: "Create", exact: true })
      .click();
    await response;

    await expect(page.getByText("Ingredients list")).toBeVisible();
  });

  test("deletes a recipe", async ({ page }) => {
    const recipeName = `E2E Recipe ${uniqueSuffix()}`;

    await createRecipe(page, recipeName);
    await gotoRecipesList(page);

    await openRecipeRowMenu(page, recipeName);

    const response = page.waitForResponse(
      (response) =>
        response.url().includes("/api/recipes") &&
        response.request().method() === "DELETE",
    );
    await page.getByRole("menuitem", { name: "Remove" }).click();
    await response;

    await expect(page.getByText(recipeName)).not.toBeVisible();
  });
});
