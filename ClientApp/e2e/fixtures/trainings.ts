import { expect, Page, type Locator } from "@playwright/test";

import {
  createAuthenticatedApiContext,
  getPageE2eCredentials,
} from "./auth";
import { baseURL, type E2eUserCredentials } from "../env";
import { uniqueSuffix } from "./helpers";

export async function clickDialogTab(dialog: Locator, name: string) {
  const tab = dialog.getByRole("tab", { name });
  await tab.scrollIntoViewIfNeeded();
  await tab.click();
  await expect(dialog.getByRole("tab", { name })).toHaveAttribute(
    "data-state",
    "active",
  );
}

export async function gotoExercises(page: Page) {
  await page.goto("/trainings/exercises");
  await expect(page.locator(".text-3xl").filter({ hasText: "Exercises" })).toBeVisible();
}

export async function gotoWorkouts(page: Page) {
  await page.goto("/trainings/workouts");
  await expect(page.locator(".text-3xl").filter({ hasText: "Workouts" })).toBeVisible();
}

export async function selectMuscleGroup(
  page: Page,
  groupName: string,
  dismissSelector = "#create-exercise-name",
) {
  const dialog = page.getByRole("dialog");
  const input = dialog.getByPlaceholder("Select a muscle group");
  await input.click();
  await input.fill(groupName.slice(0, 3));
  await dialog.getByRole("button", { name: groupName, exact: true }).click();
  await dialog.locator(dismissSelector).click();
}

export async function createExercise(page: Page, name?: string) {
  const exerciseName = name ?? `E2E Exercise ${uniqueSuffix()}`;

  await page.setViewportSize({ width: 1280, height: 900 });
  await gotoExercises(page);
  await page.getByRole("button", { name: "Create", exact: true }).click();
  await expect(page.getByRole("heading", { name: "Create New Exercise" })).toBeVisible();

  const dialog = page.getByRole("dialog");
  await dialog.locator("#create-exercise-name").fill(exerciseName);
  await selectMuscleGroup(page, "Chest");

  await clickDialogTab(dialog, "Sets");
  const firstSetCount = dialog.locator("#set-0-count1");
  await expect(firstSetCount).toBeVisible();
  await firstSetCount.fill("10");
  await dialog.locator("#set-0-count2").fill("10");

  const response = page.waitForResponse(
    (response) =>
      response.url().includes("/api/exercises") &&
      response.request().method() === "POST" &&
      response.ok(),
  );

  await dialog.getByRole("button", { name: "Create", exact: true }).click();
  await response;

  await expect(page).toHaveURL(/\/trainings\/exercises$/);
  await expect(page.getByText(exerciseName)).toBeVisible({ timeout: 15_000 });

  return exerciseName;
}

export async function createWorkout(
  page: Page,
  name?: string,
  exerciseName?: string,
) {
  const workoutName = name ?? `E2E Workout ${uniqueSuffix()}`;

  await page.setViewportSize({ width: 1280, height: 900 });
  await gotoWorkouts(page);
  await page.getByRole("button", { name: "Create", exact: true }).click();
  await expect(page.getByRole("heading", { name: "Create New Workout" })).toBeVisible();

  const dialog = page.getByRole("dialog");
  await dialog.locator("#create-training-name").fill(workoutName);
  await selectMuscleGroup(page, "Chest", "#create-training-name");

  await clickDialogTab(dialog, "Exercises");

  if (exerciseName) {
    await dialog.getByPlaceholder("Search exercises...").fill(exerciseName);
    const exerciseHeading = dialog.getByRole("heading", {
      name: exerciseName,
      exact: true,
    });
    await exerciseHeading.scrollIntoViewIfNeeded();
    await exerciseHeading.click();
    await expect(dialog.getByText("1 exercises selected")).toBeVisible();
    await dialog.getByRole("button", { name: "Next: Order Exercises" }).click();
    await expect(
      dialog.getByRole("heading", { name: "Step 2: Order Exercises" }),
    ).toBeVisible();
  }

  const response = page.waitForResponse(
    (response) =>
      response.url().includes("/api/trainings") &&
      response.request().method() === "POST" &&
      response.ok(),
  );

  await dialog.getByRole("button", { name: "Create", exact: true }).click();
  await response;

  await expect(page).toHaveURL(/\/trainings\/workouts/);
  await expect(page.getByText(workoutName)).toBeVisible({ timeout: 15_000 });

  return workoutName;
}

export function getWorkoutCard(page: Page, workoutName: string) {
  return page
    .locator(".overflow-hidden[class~='@container']")
    .filter({ has: page.getByText(workoutName, { exact: true }) })
    .first();
}

export async function clearWorkoutTimer(page: Page) {
  if (!page.url().startsWith(baseURL)) {
    return;
  }

  await page.evaluate(() => localStorage.removeItem("timerStartedAt"));
}

async function skipRestTimerIfVisible(page: Page) {
  const restTimerSkip = page
    .locator("text=Rest Timer")
    .locator("xpath=ancestor::div[contains(@class,'rounded-xl')][1]")
    .getByRole("button", { name: "Skip" });
  if (await restTimerSkip.isVisible()) {
    await restTimerSkip.click();
  }
}

export async function startWorkout(page: Page, workoutName: string) {
  await cancelActiveWorkoutIfAny(page);
  await clearWorkoutTimer(page);
  await gotoWorkouts(page);

  const card = getWorkoutCard(page, workoutName);
  await card.scrollIntoViewIfNeeded();

  const response = page.waitForResponse(
    (response) =>
      response.url().includes("/api/ongoing-trainings") &&
      response.request().method() === "POST" &&
      response.ok(),
  );

  await card.getByRole("button", { name: "Start", exact: true }).click();
  await response;

  await page.waitForURL(/\/trainings\/ongoing-workout/, { timeout: 30_000 }).catch(
    async () => {
      await page.goto("/trainings/ongoing-workout");
    },
  );
  await expect(page).toHaveURL(/\/trainings\/ongoing-workout/);
}

async function submitAdjustExerciseStep(page: Page) {
  const form = page.locator("form").filter({
    has: page.getByRole("button", { name: "Save adjustments" }),
  });
  const weightInput = form.getByRole("spinbutton").first();
  const currentWeight = await weightInput.inputValue();
  if (currentWeight === "10") {
    await weightInput.fill("11");
  }

  const saveButton = form.getByRole("button", { name: "Save adjustments" });
  await expect(saveButton).toBeEnabled({ timeout: 15_000 });

  const adjustResponse = page.waitForResponse(
    (response) =>
      response.url().includes("/adjust-sets") &&
      response.request().method() === "PUT" &&
      response.ok(),
    { timeout: 30_000 },
  );
  await saveButton.click();
  await adjustResponse;

  try {
    await page.waitForURL(
      (url) => !url.pathname.includes("/adjust-exercise/"),
      { timeout: 5_000 },
    );
    return;
  } catch {
    const credentials = await getPageE2eCredentials(page);
    const { apiContext, authHeaders, dispose } =
      await createAuthenticatedApiContext(credentials);

    try {
      const activeResponse = await apiContext.get(
        "/api/ongoing-trainings/active-training",
        { headers: authHeaders },
      );
      if (!activeResponse.ok()) {
        throw new Error(
          `Could not load active training after adjust step (${activeResponse.status()})`,
        );
      }

      const ongoing = (await activeResponse.json()) as { id: string };
      await page.goto(
        `/trainings/ongoing-workout/finish-workout-confirmation/${ongoing.id}`,
      );
      await page.waitForLoadState("networkidle");
    } finally {
      await dispose();
    }
  }
}

export async function completeOngoingWorkout(page: Page) {
  for (let i = 0; i < 30; i++) {
    const pathname = new URL(page.url()).pathname;

    if (/finish-workout-confirmation/.test(pathname)) {
      break;
    }

    if (await page.getByRole("heading", { name: "Workout Complete" }).isVisible()) {
      break;
    }

    await clearWorkoutTimer(page);
    await skipRestTimerIfVisible(page);

    if (/adjust-exercise/.test(pathname)) {
      await submitAdjustExerciseStep(page);
      continue;
    }

    const finishButton = page.getByRole("button", { name: "Finish Workout" });
    if (await finishButton.isVisible()) {
      if (/finish-workout-confirmation/.test(pathname)) {
        break;
      }
      await finishButton.click();
      await page.waitForURL(/finish-workout-confirmation/, { timeout: 15_000 });
      break;
    }

    const nextButton = page.getByRole("button", {
      name: /Next set|Next exercise/,
    });
    if (await nextButton.isVisible()) {
      await expect(nextButton).toBeEnabled({ timeout: 60_000 });
      await nextButton.click();
      await page.waitForLoadState("networkidle");
      continue;
    }

    const moreActions = page.getByRole("button", {
      name: "More workout actions",
    });
    if (await moreActions.isVisible()) {
      if (!(await moreActions.isEnabled())) {
        await page.waitForTimeout(1000);
        continue;
      }
      await moreActions.click();
      const skipItem = page.getByRole("menuitem", { name: "Skip" });
      await skipItem.click();
      await page.waitForLoadState("networkidle");
      continue;
    }

    break;
  }

  await expect(page).toHaveURL(/finish-workout-confirmation/, { timeout: 15_000 });
  await page.getByRole("button", { name: "Finish Workout" }).click();
  await expect(page).toHaveURL(/\/trainings\/workout-finished\//);
}

export async function cancelActiveWorkoutIfAny(
  page: Page,
  credentials?: E2eUserCredentials,
) {
  const resolvedCredentials = credentials ?? (await getPageE2eCredentials(page));
  const { apiContext, authHeaders, dispose } =
    await createAuthenticatedApiContext(resolvedCredentials);

  try {
    const activeResponse = await apiContext.get(
      "/api/ongoing-trainings/active-training",
      { headers: authHeaders },
    );

    if (activeResponse.status() === 404) {
      await clearWorkoutTimer(page);
      return;
    }

    if (!activeResponse.ok()) {
      const body = await activeResponse.text();
      throw new Error(
        `Failed to fetch active ongoing training (${activeResponse.status()}): ${body}`,
      );
    }

    const ongoing = (await activeResponse.json()) as {
      training: { id: string };
    };

    const deleteResponse = await apiContext.delete(
      `/api/ongoing-trainings/${ongoing.training.id}`,
      { headers: authHeaders },
    );

    if (!deleteResponse.ok()) {
      const body = await deleteResponse.text();
      throw new Error(
        `Failed to cancel active ongoing training (${deleteResponse.status()}): ${body}`,
      );
    }
  } finally {
    await dispose();
  }

  await clearWorkoutTimer(page);
}

export async function setWeeklyWorkoutGoal(page: Page, goal = 3) {
  await gotoWorkouts(page);

  const setTargetButton = page.getByRole("button", { name: "Set weekly target" });
  if (await setTargetButton.isVisible()) {
    await setTargetButton.click();
  } else {
    await page.goto("/trainings/workouts/workouts-goal");
  }

  await expect(
    page.getByRole("heading", { name: "Weekly workout target" }),
  ).toBeVisible({ timeout: 15_000 });
  await page.locator("#weekly-workouts-goal").fill(String(goal));

  const response = page.waitForResponse(
    (response) =>
      response.url().includes("/api/goals") && response.ok(),
  );

  await page.getByRole("button", { name: "Save", exact: true }).click();
  await response;
  await expect(page).toHaveURL(/\/trainings\/workouts\/?$/, { timeout: 15_000 });
}

export async function openExerciseMenu(page: Page, exerciseName: string) {
  const card = page
    .locator('[class*="border-t-primary"]')
    .filter({ has: page.getByText(exerciseName, { exact: true }) })
    .first();
  await expect(card).toBeVisible();
  await card.getByRole("button", { name: "Exercise menu" }).click();
}

export async function editExercise(
  page: Page,
  currentName: string,
  updatedName: string,
) {
  await openExerciseMenu(page, currentName);

  const editLink = page.getByRole("menuitem", { name: "Edit" });
  const href = await editLink.getAttribute("href");
  const exerciseId = href?.match(/\/edit\/([^/?]+)/)?.[1];
  if (!exerciseId) {
    throw new Error(`Could not resolve exercise id from href: ${href}`);
  }

  await page.keyboard.press("Escape");
  await page.goto(`/trainings/exercises/edit/${exerciseId}`);

  const dialog = page.getByRole("dialog", { name: "Edit Exercise" });
  await expect(dialog.locator("#create-exercise-name")).toBeVisible({
    timeout: 15_000,
  });
  await dialog.locator("#create-exercise-name").fill(updatedName);

  const putResponse = page.waitForResponse(
    (response) =>
      response.url().includes("/api/exercises") &&
      response.request().method() === "PUT",
    { timeout: 60_000 },
  );
  await dialog.locator("form").evaluate((form: HTMLFormElement) => {
    form.requestSubmit();
  });
  const response = await putResponse;
  expect(response.ok()).toBeTruthy();

  await expect(page.getByText(updatedName, { exact: true })).toBeVisible({
    timeout: 15_000,
  });
}

export async function gotoWorkoutHistory(page: Page) {
  await page.goto("/trainings/history");
  await expect(
    page.locator(".text-3xl").filter({ hasText: "Workout history" }),
  ).toBeVisible({ timeout: 15_000 });
}

export async function deleteExercise(page: Page, exerciseName: string) {
  await openExerciseMenu(page, exerciseName);

  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/exercises") &&
      apiResponse.request().method() === "DELETE" &&
      apiResponse.ok(),
  );

  await page.getByRole("menuitem", { name: "Delete" }).click();
  await response;

  await expect(page.getByText(exerciseName, { exact: true })).not.toBeVisible();
}

export async function deleteWorkout(page: Page, workoutName: string) {
  const card = getWorkoutCard(page, workoutName);
  await card.scrollIntoViewIfNeeded();
  await card.getByRole("button", { name: "Training actions" }).click();
  await page.getByRole("menuitem", { name: "Delete" }).click();

  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/trainings") &&
      apiResponse.request().method() === "DELETE" &&
      apiResponse.ok(),
  );

  await page.getByRole("button", { name: "Delete training" }).click();
  await response;

  await expect(page.getByText(workoutName, { exact: true })).not.toBeVisible();
}

export async function openWorkoutHistorySession(
  page: Page,
  workoutName: string,
) {
  const card = page
    .locator("[class*='cursor-pointer']")
    .filter({ has: page.getByRole("heading", { name: workoutName, exact: true }) })
    .first();
  await card.scrollIntoViewIfNeeded();
  await card.click();

  await expect(page.getByRole("dialog")).toBeVisible();
  await expect(
    page.getByRole("heading", { name: workoutName, exact: true }),
  ).toBeVisible();
}

export async function gotoTrainingsOverview(page: Page) {
  await page.goto("/trainings/overview");
  await expect(
    page.locator(".text-3xl").filter({ hasText: "Trainings Overview" }),
  ).toBeVisible({ timeout: 15_000 });
}

export async function selectTrainingsOverviewDatePreset(
  page: Page,
  preset: "Last week" | "Last month" | "Last year" | "All time",
) {
  await page
    .locator("button")
    .filter({ has: page.locator(".lucide-calendar") })
    .first()
    .click();
  await page.getByRole("button", { name: preset, exact: true }).click();
}

export async function openWorkoutStats(page: Page, workoutName: string) {
  await gotoWorkouts(page);

  const card = getWorkoutCard(page, workoutName);
  await expect(card).toBeVisible({ timeout: 15_000 });
  await card.scrollIntoViewIfNeeded();
  await card.getByRole("button", { name: "Training actions" }).click();
  await page.getByRole("menuitem", { name: "View stats" }).click();
  await expect(page).toHaveURL(/\/trainings\/workouts\/.*\/stats/, {
    timeout: 15_000,
  });
}

export async function cancelOngoingWorkoutFromUi(page: Page) {
  await page.getByRole("button", { name: "More workout actions" }).click();
  await page.getByRole("menuitem", { name: "Cancel training" }).click();

  await expect(
    page.getByRole("heading", { name: "Cancel Training" }),
  ).toBeVisible();

  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/ongoing-trainings") &&
      apiResponse.request().method() === "DELETE" &&
      apiResponse.ok(),
  );

  await page
    .getByRole("alertdialog")
    .getByRole("button", { name: "Cancel Training", exact: true })
    .click();
  await response;

  await expect(page).toHaveURL(/\/trainings\/workouts/, { timeout: 15_000 });
}

export async function openExerciseInfoDialog(page: Page, exerciseName: string) {
  await openExerciseMenu(page, exerciseName);

  const editLink = page.getByRole("menuitem", { name: "Edit" });
  const href = await editLink.getAttribute("href");
  const exerciseId = href?.match(/\/edit\/([^/?]+)/)?.[1];
  if (!exerciseId) {
    throw new Error(`Could not resolve exercise id from href: ${href}`);
  }

  await page.keyboard.press("Escape");
  await page.goto(`/trainings/exercises/info/${exerciseId}`);
  await expect(page.getByRole("dialog")).toBeVisible();
}
