import { expect, test } from "../fixtures/authenticatedTest";

import {
  createBookViaUi,
  deleteBookViaUi,
  editBookViaUi,
  gotoBooks,
  resetReadingState,
} from "../fixtures/reading";

test.describe("reading books", () => {
  test.describe.configure({ mode: "serial" });

  test.beforeEach(async ({ page, e2eUser }) => {
    await resetReadingState(page, e2eUser);
  });

  test("creates a book", async ({ page }) => {
    const title = await createBookViaUi(page);

    await expect(page.getByText(title, { exact: true })).toBeVisible();
    await expect(page.getByText("E2E Author", { exact: true })).toBeVisible();
  });

  test("opens book detail from list", async ({ page }) => {
    const title = await createBookViaUi(page, { currentPage: 12 });

    await page.getByRole("link", { name: title, exact: true }).click();

    await expect(page.getByRole("link", { name: "Books" })).toBeVisible();
    await expect(
      page.locator('[aria-current="page"]').filter({ hasText: title }),
    ).toBeVisible();
    await expect(page.getByText("Page 12 of 200")).toBeVisible();
    await expect(page.getByRole("button", { name: "Start reading" })).toBeVisible();
  });

  test("edits a book", async ({ page }) => {
    const title = await createBookViaUi(page);
    const updatedTitle = `${title} Updated`;

    await editBookViaUi(page, title, updatedTitle);
  });

  test("deletes a book", async ({ page }) => {
    const title = await createBookViaUi(page);

    await deleteBookViaUi(page, title);
    await expect(page.getByText("No books yet. Add your first book.")).toBeVisible();
  });

  test("filters books by status", async ({ page }) => {
    const ongoingTitle = await createBookViaUi(page, {
      title: `E2E Ongoing ${Date.now()}`,
      status: "Ongoing",
    });
    const notStartedTitle = await createBookViaUi(page, {
      title: `E2E NotStarted ${Date.now()}`,
      status: "NotStarted",
    });

    await gotoBooks(page);
    await page.getByRole("combobox").first().click();
    await page.getByRole("option", { name: "Ongoing" }).click();

    await expect(page.getByText(ongoingTitle, { exact: true })).toBeVisible();
    await expect(page.getByText(notStartedTitle, { exact: true })).not.toBeVisible();
  });
});
