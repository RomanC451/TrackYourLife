import { expect, test } from "../fixtures/authenticatedTest";

import {
  createExercise,
  deleteExercise,
  editExercise,
  openExerciseInfoDialog,
  openExerciseMenu,
} from "../fixtures/trainings";

test.describe("trainings exercises", () => {
  test("creates an exercise", async ({ page }) => {
    const name = await createExercise(page);
    await expect(page.getByText(name)).toBeVisible();
  });

  test("edits an exercise", async ({ page }) => {
    test.setTimeout(90_000);

    const name = await createExercise(page);
    const updatedName = `${name} Updated`;

    await editExercise(page, name, updatedName);
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
