import { expect, test } from "../fixtures/authenticatedTest";

import { credentialsForSpecFile } from "../env";
import { waitForApi } from "../fixtures/api";
import { ensureTestUser, setupAuthenticatedContext } from "../fixtures/auth";
import {
  cancelActiveWorkoutIfAny,
  cancelOngoingWorkoutFromUi,
  completeOngoingWorkout,
  createExercise,
  createWorkout,
  startWorkout,
} from "../fixtures/trainings";

test.describe("ongoing workout", () => {
  test.describe.configure({ mode: "serial" });

  test.beforeAll(async ({ browser }) => {
    await waitForApi();
    const credentials = credentialsForSpecFile(test.info().file);
    await ensureTestUser(credentials);
    const context = await browser.newContext();
    await setupAuthenticatedContext(context, credentials);
    const page = await context.newPage();
    await cancelActiveWorkoutIfAny(page);
    await context.close();
  });

  test("redirects to workouts when no active session", async ({ page, e2eUser }) => {
    await cancelActiveWorkoutIfAny(page, e2eUser);
    await page.goto("/trainings/ongoing-workout");
    await expect(page).toHaveURL(/\/trainings\/workouts/, { timeout: 15_000 });
  });

  test("completes a full workout session", async ({ page }) => {
    test.setTimeout(90_000);

    const exerciseName = await createExercise(page);
    const workoutName = await createWorkout(page, undefined, exerciseName);

    await startWorkout(page, workoutName);
    await completeOngoingWorkout(page);

    await expect(page.getByText("Great job!")).toBeVisible();
    await expect(page.getByText("You've completed your workout")).toBeVisible();
  });

  test("cancels an active workout session", async ({ page, e2eUser }) => {
    await cancelActiveWorkoutIfAny(page, e2eUser);

    const exerciseName = await createExercise(page);
    const workoutName = await createWorkout(page, undefined, exerciseName);

    await startWorkout(page, workoutName);
    await expect(page.getByText(exerciseName)).toBeVisible();

    await cancelOngoingWorkoutFromUi(page);
    await expect(page.getByText(workoutName)).toBeVisible();
  });

  test("resumes workout from home", async ({ page, e2eUser }) => {
    await cancelActiveWorkoutIfAny(page, e2eUser);

    const exerciseName = await createExercise(page);
    const workoutName = await createWorkout(page, undefined, exerciseName);

    await startWorkout(page, workoutName);
    await page.goto("/home");

    await page.getByRole("link", { name: "Continue" }).click();
    await expect(page).toHaveURL(/\/trainings\/ongoing-workout/);
    await expect(
      page.locator(".text-3xl").filter({ hasText: "Ongoing workout" }),
    ).toBeVisible();
    await expect(page.getByText(exerciseName)).toBeVisible();
  });
});
