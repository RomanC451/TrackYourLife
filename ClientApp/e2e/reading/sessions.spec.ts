import { expect, test } from "../fixtures/authenticatedTest";

import {
  cancelActiveReadingSessionIfAny,
  cancelReadingSessionFromUi,
  createBookViaUi,
  finishReadingSession,
  gotoReadingHistory,
  openBookDetail,
  resetReadingState,
  startReadingFromBookDetail,
} from "../fixtures/reading";

test.describe("reading sessions", () => {
  test.describe.configure({ mode: "serial" });

  test.beforeEach(async ({ page, e2eUser }) => {
    await resetReadingState(page, e2eUser);
  });

  test("starts and finishes a reading session", async ({ page }) => {
    const title = await createBookViaUi(page, { currentPage: 10 });
    await openBookDetail(page, title);
    await startReadingFromBookDetail(page);

    await expect(page.getByText(title)).toBeVisible();
    await expect(page.getByText("Started at page 10")).toBeVisible();

    await finishReadingSession(page, 25, {
      chapterTitle: "Cap. 1",
      noteContent: "Finished chapter one",
    });

    await expect(page).toHaveURL(/\/reading\/dashboard/, { timeout: 15_000 });

    await gotoReadingHistory(page);
    const row = page.getByRole("row").filter({ hasText: title });
    await expect(row).toBeVisible();
    await expect(row.getByText("15")).toBeVisible();
  });

  test("cancels an active reading session", async ({ page }) => {
    const title = await createBookViaUi(page, { currentPage: 5 });
    await openBookDetail(page, title);
    await startReadingFromBookDetail(page);

    await cancelReadingSessionFromUi(page);

    await expect(page).toHaveURL(/\/books/, { timeout: 15_000 });

    await page.goto("/reading/ongoing-session");
    await expect(page.getByText("No active session.")).toBeVisible();
  });

  test("shows resume card on home when session is active", async ({ page }) => {
    const title = await createBookViaUi(page, { currentPage: 3 });
    await openBookDetail(page, title);
    await startReadingFromBookDetail(page);

    await page.goto("/home");
    await expect(page.getByText(title, { exact: true })).toBeVisible();

    const readingSection = page
      .locator("section")
      .filter({ hasText: "Daily reading progress and sessions" });
    await expect(
      readingSection.getByRole("link", { name: "Resume" }),
    ).toBeVisible();

    await cancelActiveReadingSessionIfAny(page);
  });
});
