import { expect, test } from "../fixtures/authenticatedTest";

import { expectPageTitle } from "../fixtures/navigation";
import {
  createBookViaUi,
  editSessionFromHistory,
  finishReadingSession,
  gotoReadingDashboard,
  openBookDetail,
  resetReadingState,
  saveReadingSessionNote,
  setDailyReadingGoal,
  startReadingFromBookDetail,
} from "../fixtures/reading";

test.describe("reading dashboard", () => {
  test.describe.configure({ mode: "serial" });

  test.beforeEach(async ({ page, e2eUser }) => {
    await resetReadingState(page, e2eUser);
  });

  test("loads dashboard sections", async ({ page }) => {
    await gotoReadingDashboard(page);

    await expect(page.getByText("Today's progress")).toBeVisible();
    await expect(page.getByText("Streak")).toBeVisible();
    await expect(page.getByText("Recent books")).toBeVisible();
    await expect(page.getByText("Reading note")).toBeVisible();
    await expect(page.getByText("Pages read")).toBeVisible();
    await expect(page.getByRole("link", { name: "Daily goal" })).toBeVisible();
  });

  test("shows random reading note after finishing a session with a note", async ({
    page,
  }) => {
    const title = await createBookViaUi(page, { currentPage: 0 });
    await openBookDetail(page, title);
    await startReadingFromBookDetail(page);
    await saveReadingSessionNote(page, "4", "Focus", "Remember this idea");
    await finishReadingSession(page, 10);

    const randomNoteResponse = page.waitForResponse(
      (apiResponse) =>
        apiResponse.url().includes("/api/reading/random-note") &&
        apiResponse.ok(),
    );
    await gotoReadingDashboard(page);
    await randomNoteResponse;

    await expect(page.getByText("Chapter 4 - Focus")).toBeVisible({
      timeout: 15_000,
    });
    await expect(page.getByText("Remember this idea")).toBeVisible();
    await expect(
      page.getByRole("link", { name: title, exact: true }),
    ).toBeVisible();
  });

  test("sets a daily reading goal", async ({ page }) => {
    await setDailyReadingGoal(page, 20);

    await expect(
      page.getByText("Set a daily reading goal to track progress."),
    ).not.toBeVisible();
    await expect(page.getByText("0 / 20 pages")).toBeVisible({
      timeout: 15_000,
    });
  });

  test("shows pages chart after finishing a session", async ({ page }) => {
    const title = await createBookViaUi(page, { currentPage: 0 });
    await openBookDetail(page, title);
    await startReadingFromBookDetail(page);
    await finishReadingSession(page, 12);

    await gotoReadingDashboard(page);

    await expect(page.getByText("Pages read")).toBeVisible();
    await expect(
      page.getByText("Finish a reading session to see your pages chart."),
    ).not.toBeVisible({ timeout: 15_000 });
  });

  test("reflects finished session in dashboard progress", async ({ page }) => {
    await setDailyReadingGoal(page, 15);

    const title = await createBookViaUi(page, { currentPage: 0 });
    await openBookDetail(page, title);
    await startReadingFromBookDetail(page);
    await finishReadingSession(page, 15);

    const dashboardResponse = page.waitForResponse(
      (apiResponse) =>
        apiResponse.url().includes("/api/reading/dashboard") &&
        apiResponse.ok(),
    );
    await gotoReadingDashboard(page);
    await dashboardResponse;

    await expect(page.getByText("15 / 15 pages")).toBeVisible({
      timeout: 15_000,
    });
    await expect(page.getByText(title)).toBeVisible();
  });

  test("edits a completed session from history", async ({ page }) => {
    const title = await createBookViaUi(page, { currentPage: 0 });
    await openBookDetail(page, title);
    await startReadingFromBookDetail(page);
    await saveReadingSessionNote(page, "1", "Start", "Original session note");
    await finishReadingSession(page, 10);
    await editSessionFromHistory(page, title, {
      endPage: 12,
      noteContent: "Updated session note",
    });

    await expectPageTitle(page, "Reading history");
    const row = page.getByRole("row").filter({ hasText: title });
    await expect(row).toBeVisible();
    await expect(row.getByText("12")).toBeVisible();

    await openBookDetail(page, title);
    await expect(page.getByText("Updated session note")).toBeVisible();
  });
});
