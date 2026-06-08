import { expect, test } from "../fixtures/authenticatedTest";

import {
  createExercise,
  deleteExercise,
  openExerciseInfoDialog,
  openExerciseMenu,
} from "../fixtures/trainings";

test.describe("trainings exercises", () => {
  test("creates an exercise", async ({ page }) => {
    const name = await createExercise(page);
    await expect(page.getByText(name)).toBeVisible();
  });

  test("edits an exercise", async ({ page }) => {
    const name = await createExercise(page);
    const updatedName = `${name} Updated`;

    await openExerciseMenu(page, name);
    await page.getByRole("menuitem", { name: "Edit" }).click();
    await page.waitForURL(/\/trainings\/exercises\/edit\//);

    await expect(page.locator("#create-exercise-name")).toBeVisible({
      timeout: 15_000,
    });
    await page.locator("#create-exercise-name").fill(updatedName);

    const response = page.waitForResponse(
      (response) =>
        response.url().includes("/api/exercises") &&
        response.request().method() === "PUT" &&
        response.ok(),
    );
    await page.getByRole("button", { name: "Save", exact: true }).click();
    await response;

    await expect(page.getByText(updatedName)).toBeVisible();
  });

  test("opens exercise info dialog", async ({ page }) => {
    const name = await createExercise(page);

    await openExerciseInfoDialog(page, name);

    await expect(page.getByRole("heading", { name })).toBeVisible();
  });

  test("navigates to exercise stats page", async ({ page }) => {
    const name = await createExercise(page);

    await openExerciseMenu(page, name);
    await page.getByRole("menuitem", { name: "Stats" }).click();

    await expect(page).toHaveURL(/\/trainings\/exercises\/.*\/stats/);
  });

  test("deletes an exercise", async ({ page }) => {
    const name = await createExercise(page);
    await deleteExercise(page, name);
  });
});
