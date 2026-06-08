import { expect, Page } from "@playwright/test";

export async function gotoDiary(page: Page) {
  await page.goto("/nutrition/diary");
  await expect(page.getByText("Food Diary")).toBeVisible({ timeout: 30_000 });
}

export async function hasGoalsOverlay(page: Page) {
  return page
    .getByText("Goals are not defined. Calculate them first.")
    .isVisible();
}

export async function fillNutritionCalculator(page: Page) {
  await page.getByLabel("Age").fill("30");
  await page.getByLabel("Weight (kg)").fill("75");
  await page.getByLabel("Height (cm)").fill("180");

  const comboboxes = page.getByRole("combobox");
  await comboboxes.nth(0).click();
  await page.getByRole("option", { name: "Male", exact: true }).click();
  await comboboxes.nth(1).click();
  await page
    .getByRole("option", {
      name: "Moderately active (exercise 3–5 days/week)",
      exact: true,
    })
    .click();
  await comboboxes.nth(2).click();
  await page.getByRole("option", { name: "Maintenance", exact: true }).click();

  const calculateResponse = page.waitForResponse(
    (response) =>
      response.url().includes("/api/goals/nutrition") &&
      response.request().method() === "PUT" &&
      response.ok(),
  );

  await page.getByRole("button", { name: "Calculate", exact: true }).click();
  await calculateResponse;
}

export async function ensureNutritionGoals(page: Page) {
  await gotoDiary(page);

  if (!(await hasGoalsOverlay(page))) {
    return;
  }

  await page.getByRole("button", { name: "Calculate goals" }).click();
  await expect(
    page.getByRole("heading", { name: "Nutrition goals calculator" }),
  ).toBeVisible();

  await fillNutritionCalculator(page);

  await expect(
    page.getByText("Goals are not defined. Calculate them first."),
  ).not.toBeVisible({ timeout: 15_000 });
}

export async function searchAndOpenFood(page: Page, searchTerm: string) {
  const searchInput = page.getByPlaceholder("Search food...");
  await searchInput.click();
  await searchInput.fill(searchTerm);

  const searchResponse = page.waitForResponse(
    (response) =>
      response.url().includes("/api/foods/search") && response.ok(),
    { timeout: 30_000 },
  );
  await page.waitForTimeout(1200);
  await searchResponse;

  const resultsCard = page.locator("[class*='backdrop-blur']").first();
  await expect(resultsCard).toBeVisible({ timeout: 15_000 });
  await resultsCard.locator(".relative > button.w-full").first().click();

  await expect(
    page.getByRole("heading", { name: "Add food diary" }),
  ).toBeVisible({ timeout: 15_000 });
}

export async function submitFoodDiaryDialog(page: Page, buttonName: "Add" | "Save") {
  const dialog = page.getByRole("dialog");
  const mealCombobox = dialog.getByRole("combobox", { name: "Meal" });

  if (await mealCombobox.isVisible()) {
    await mealCombobox.click();
    await page.getByRole("option", { name: "Breakfast" }).click();
  }

  const response = page.waitForResponse(
    (response) =>
      response.url().includes("/api/food-diaries") &&
      ["POST", "PUT"].includes(response.request().method()),
  );

  await dialog.getByRole("button", { name: buttonName, exact: true }).click();
  const apiResponse = await response;
  expect(apiResponse.ok()).toBe(true);
}

export async function switchDiarySearchTo(
  page: Page,
  category: "Foods" | "Recipes",
) {
  await page.getByRole("group").getByText(category, { exact: true }).click();
}

export async function searchAndAddRecipeToDiary(page: Page, recipeName: string) {
  await switchDiarySearchTo(page, "Recipes");

  const searchInput = page.getByPlaceholder("Search recipe...");
  await searchInput.click();
  await searchInput.fill(recipeName);

  const resultsCard = page.locator("[class*='backdrop-blur-2xl']").first();
  await expect(resultsCard).toBeVisible({ timeout: 15_000 });

  const recipeRow = resultsCard
    .locator(".relative")
    .filter({ hasText: recipeName })
    .first();
  await expect(recipeRow).toBeVisible({ timeout: 15_000 });

  const addResponse = page.waitForResponse(
    (response) =>
      response.url().includes("/api/recipe-diaries") &&
      response.request().method() === "POST" &&
      response.ok(),
  );

  await recipeRow.getByRole("button").last().click();
  await page.getByRole("menuitem", { name: "Breakfast" }).click();
  await addResponse;
}

export async function gotoNutritionOverview(page: Page) {
  await page.goto("/nutrition/overview", { waitUntil: "domcontentloaded" });
  await expect(page.getByText("Nutrition Summary")).toBeVisible({
    timeout: 15_000,
  });
}

export async function getOverviewCalories(page: Page) {
  const caloriesCard = page
    .locator(".min-w-\\[200px\\]")
    .filter({ has: page.getByText("Calories", { exact: true }) })
    .first();
  await expect(caloriesCard).toBeVisible();

  const valueText = await caloriesCard.locator("p.text-2xl").textContent();
  const current = valueText?.split("/")[0]?.trim() ?? "0";
  return Number.parseFloat(current.replace(/[^\d.]/g, "") || "0");
}

export async function createRecipe(page: Page, name: string) {
  await page.goto("/nutrition/recipes/create");
  await expect(page.getByRole("heading", { name: "Create a new recipe" })).toBeVisible();

  await page.locator("#create-recipe-name").fill(name);
  await page.locator("#create-recipe-portions").fill("2");
  await page.locator("#create-recipe-weight").fill("400");

  const response = page.waitForResponse(
    (response) =>
      response.url().includes("/api/recipes") &&
      response.request().method() === "POST" &&
      response.ok(),
  );

  await page.getByRole("button", { name: "Create", exact: true }).click();
  await response;

  await expect(page).toHaveURL(/\/nutrition\/recipes\/edit\//);
  const dialog = page.getByRole("dialog");
  await expect(dialog.getByRole("tab", { name: "Ingredients" })).toHaveAttribute(
    "data-state",
    "active",
    { timeout: 15_000 },
  );
}

export async function clickFirstFoodSearchResult(
  page: Page,
  scope: Page | ReturnType<Page["locator"]> = page,
) {
  const resultsCard = scope.locator("[class*='backdrop-blur-2xl']").first();
  await expect(resultsCard).toBeVisible({ timeout: 15_000 });
  await resultsCard.locator(".relative > button.w-full").first().click();
}

export async function gotoRecipesList(page: Page) {
  await page.goto("/nutrition/recipes");
  await expect(page).not.toHaveURL(/\/auth/, { timeout: 30_000 });
  await expect(
    page
      .getByRole("table")
      .or(page.getByRole("heading", { name: "Recipes" }))
      .or(page.getByText("You have no recipes")),
  ).toBeVisible({ timeout: 15_000 });
}

export async function openRecipeRowMenu(page: Page, recipeName: string) {
  const row = page.getByRole("row").filter({ hasText: recipeName });
  await row.getByRole("button", { name: "Open menu" }).click();
}
