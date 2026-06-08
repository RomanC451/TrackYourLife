import { expect, test } from "../fixtures/authenticatedTest";

import {
  cancelActiveWorkoutIfAny,
  completeOngoingWorkout,
  createExercise,
  createWorkout,
  openWorkoutStats,
  startWorkout,
} from "../fixtures/trainings";

test.describe("workout stats", () => {
  test("opens stats for a workout with no sessions yet", async ({ page }) => {
    const exerciseName = await createExercise(page);
    const workoutName = await createWorkout(page, undefined, exerciseName);

    await openWorkoutStats(page, workoutName);

    await expect(
      page.getByRole("heading", { name: workoutName, exact: true }),
    ).toBeVisible();
    await expect(
      page.getByText("No completed sessions in this period yet."),
    ).toBeVisible();
    await expect(page.getByRole("button", { name: "Start workout" })).toBeVisible();
  });

  test("shows stats after completing a workout session", async ({ page, e2eUser }) => {
    test.setTimeout(90_000);

    await cancelActiveWorkoutIfAny(page, e2eUser);

    const exerciseName = await createExercise(page);
    const workoutName = await createWorkout(page, undefined, exerciseName);

    await startWorkout(page, workoutName);
    await completeOngoingWorkout(page);

    await openWorkoutStats(page, workoutName);

    await expect(
      page.getByRole("heading", { name: workoutName, exact: true }),
    ).toBeVisible();
    await expect(page.getByText("1 completed", { exact: true })).toBeVisible();
    await expect(page.getByText("Duration over time")).toBeVisible();
  });
});
