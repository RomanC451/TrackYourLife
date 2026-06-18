import { expect, test } from "../fixtures/authenticatedTest";

import { expectPageTitle } from "../fixtures/navigation";
import {
  createBookViaUi,
  editSessionFromHistory,
  finishReadingSession,
  gotoReadingDashboard,
  openBookDetail,
  resetReadingState,
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
    await expect(page.getByText("Recent notes")).toBeVisible();
    await expect(page.getByRole("link", { name: "Daily goal" })).toBeVisible();
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

  test("reflects finished session in dashboard progress", async ({ page }) => {
    await setDailyReadingGoal(page, 15);

    const title = await createBookViaUi(page, { currentPage: 0 });
    await openBookDetail(page, title);
    await startReadingFromBookDetail(page);
    await finishReadingSession(page, 15, "Dashboard progress test");

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
    await finishReadingSession(page, 10, "Original note");

    await editSessionFromHistory(page, title, "Updated e2e note");

    await expectPageTitle(page, "Reading history");
    await expect(page.getByRole("row").filter({ hasText: title })).toBeVisible();
  });
});
