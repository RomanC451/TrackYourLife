import { expect, test } from "../fixtures/authenticatedTest";

import {
  cancelActiveWorkoutIfAny,
  completeOngoingWorkout,
  createExercise,
  createWorkout,
  gotoWorkoutHistory,
  openWorkoutHistorySession,
  startWorkout,
} from "../fixtures/trainings";

test.describe("workout history", () => {
  test.beforeEach(async ({ page, e2eUser }) => {
    await cancelActiveWorkoutIfAny(page, e2eUser);
  });

  test("shows a completed workout and opens session details", async ({ page }) => {
    test.setTimeout(90_000);

    const exerciseName = await createExercise(page);
    const workoutName = await createWorkout(page, undefined, exerciseName);

    await startWorkout(page, workoutName);
    await completeOngoingWorkout(page);

    await gotoWorkoutHistory(page);
    await expect(page.getByRole("heading", { name: workoutName, exact: true })).toBeVisible();
    await openWorkoutHistorySession(page, workoutName);
  });
});
