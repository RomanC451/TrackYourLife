import { expect, test } from "../fixtures/authenticatedTest";

import { expectPageTitle } from "../fixtures/navigation";
import {
  cancelActiveWorkoutIfAny,
  completeOngoingWorkout,
  createExercise,
  createWorkout,
  gotoTrainingsOverview,
  selectTrainingsOverviewDatePreset,
  startWorkout,
} from "../fixtures/trainings";

test.describe("trainings overview", () => {
  test("loads summary cards and date range selector", async ({ page }) => {
    await gotoTrainingsOverview(page);

    await expectPageTitle(page, "Trainings Overview");
    await expect(page.getByText("Total Workouts", { exact: true }).first()).toBeVisible();
    await expect(page.getByText("Total Time", { exact: true }).first()).toBeVisible();
    await expect(page.getByText("Calories Burned", { exact: true }).first()).toBeVisible();
    await expect(page.getByText("Active Training", { exact: true }).first()).toBeVisible();
    await expect(
      page.locator("button").filter({ has: page.locator(".lucide-calendar") }).first(),
    ).toBeVisible();
  });

  test("switches date range preset", async ({ page }) => {
    await gotoTrainingsOverview(page);

    const dateRangeButton = page
      .locator("button")
      .filter({ has: page.locator(".lucide-calendar") })
      .first();

    await selectTrainingsOverviewDatePreset(page, "All time");
    await expect(dateRangeButton).toContainText("All time");
  });

  test("reflects a completed workout in summary cards", async ({ page, e2eUser }) => {
    test.setTimeout(90_000);

    await cancelActiveWorkoutIfAny(page, e2eUser);

    const exerciseName = await createExercise(page);
    const workoutName = await createWorkout(page, undefined, exerciseName);

    await startWorkout(page, workoutName);
    await completeOngoingWorkout(page);

    await gotoTrainingsOverview(page);

    const totalWorkoutsCard = page
      .locator(".h-\\[100px\\]")
      .filter({ has: page.getByText("Total Workouts", { exact: true }) })
      .first();
    await expect(totalWorkoutsCard).toBeVisible();
    await expect(totalWorkoutsCard.locator("p.text-2xl")).not.toHaveText("0");
  });
});
