import { expect, test } from "../fixtures/authenticatedTest";

import {
  cancelActiveWorkoutIfAny,
  clickDialogTab,
  createExercise,
  createWorkout,
  deleteWorkout,
  getWorkoutCard,
  gotoWorkouts,
  setWeeklyWorkoutGoal,
  startWorkout,
} from "../fixtures/trainings";

test.describe("trainings workouts", () => {
  test("creates a workout", async ({ page }) => {
    const exerciseName = await createExercise(page);
    const workoutName = await createWorkout(page, undefined, exerciseName);

    await expect(page.getByText(workoutName)).toBeVisible();
  });

  test("edits a workout", async ({ page }) => {
    const exerciseName = await createExercise(page);
    const workoutName = await createWorkout(page, undefined, exerciseName);
    const updatedName = `${workoutName} Updated`;

    const card = getWorkoutCard(page, workoutName);
    await card.getByRole("button", { name: "Training actions" }).click();
    await page.getByRole("menuitem", { name: "Edit" }).click();

    const dialog = page.getByRole("dialog");
    await dialog.locator("#create-training-name").fill(updatedName);
    await clickDialogTab(dialog, "Exercises");
    await dialog.getByRole("button", { name: "Next: Order Exercises" }).click();
    await expect(
      dialog.getByRole("heading", { name: "Step 2: Order Exercises" }),
    ).toBeVisible();

    const response = page.waitForResponse(
      (response) =>
        response.url().includes("/api/trainings") &&
        response.request().method() === "PUT" &&
        response.ok(),
    );
    await dialog.getByRole("button", { name: "Save", exact: true }).click();
    await response;

    await expect(page.getByText(updatedName)).toBeVisible();
  });

  test("sets weekly workout goal", async ({ page }) => {
    await setWeeklyWorkoutGoal(page, 4);

    await page.goto("/trainings/workouts/workouts-goal");
    await expect(page.locator("#weekly-workouts-goal")).toHaveValue("4");
  });

  test("opens create workout plan dialog", async ({ page }) => {
    await gotoWorkouts(page);

    const createPlanButton = page.getByRole("button", { name: "Create plan" });
    if (await createPlanButton.isVisible()) {
      await createPlanButton.click();
      await expect(page.getByText("Create Workout Plan")).toBeVisible();
      await expect(page.locator("#workout-plan-name")).toBeVisible();
    }
  });

  test("starts a workout", async ({ page }) => {
    const exerciseName = await createExercise(page);
    const workoutName = await createWorkout(page, undefined, exerciseName);

    await startWorkout(page, workoutName);
    await expect(
      page.locator(".text-3xl").filter({ hasText: "Ongoing workout" }),
    ).toBeVisible();

    await cancelActiveWorkoutIfAny(page);
  });

  test("deletes a workout", async ({ page }) => {
    const exerciseName = await createExercise(page);
    const workoutName = await createWorkout(page, undefined, exerciseName);

    await deleteWorkout(page, workoutName);
  });
});
