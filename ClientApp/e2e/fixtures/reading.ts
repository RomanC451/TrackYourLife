import { expect, Page } from "@playwright/test";

import {
  createAuthenticatedApiContext,
  getPageE2eCredentials,
  type E2eUserCredentials,
} from "./auth";
import { uniqueSuffix } from "./helpers";
import { expectPageTitle } from "./navigation";

export async function saveReadingSessionNote(
  page: Page,
  chapterNumber: string,
  chapterTitle: string,
  content: string,
) {
  await page.getByLabel("Chapter number").fill(chapterNumber);
  await page.getByLabel("Chapter title").fill(chapterTitle);
  await page.getByLabel("Note").fill(content);

  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/reading-sessions") &&
      apiResponse.url().includes("/notes") &&
      apiResponse.request().method() === "POST" &&
      apiResponse.ok(),
  );

  await page.getByRole("button", { name: "Save note" }).click();
  await response;
}

export async function gotoBooks(page: Page) {
  await page.goto("/books");
  await expectPageTitle(page, "Books");
}

export async function gotoReadingDashboard(page: Page) {
  await page.goto("/reading/dashboard");
  await expectPageTitle(page, "Reading dashboard");
}

export async function gotoReadingHistory(page: Page) {
  await page.goto("/reading/history");
  await expectPageTitle(page, "Reading history");
}

export async function gotoOngoingReadingSession(page: Page) {
  await page.goto("/reading/ongoing-session");
  await expectPageTitle(page, "Reading session");
}

export async function cancelActiveReadingSessionIfAny(
  page: Page,
  credentials?: E2eUserCredentials,
) {
  const resolvedCredentials =
    credentials ?? (await getPageE2eCredentials(page));
  const { apiContext, authHeaders, dispose } =
    await createAuthenticatedApiContext(resolvedCredentials);

  try {
    const activeResponse = await apiContext.get(
      "/api/reading-sessions/active",
      { headers: authHeaders },
    );

    if (!activeResponse.ok()) {
      return;
    }

    const body = await activeResponse.text();
    if (!body) {
      return;
    }

    const active = JSON.parse(body) as { id?: string } | null;
    if (!active?.id) {
      return;
    }

    const deleteResponse = await apiContext.delete(
      `/api/reading-sessions/${active.id}`,
      { headers: authHeaders },
    );

    if (!deleteResponse.ok()) {
      const body = await deleteResponse.text();
      throw new Error(
        `Failed to cancel active reading session (${deleteResponse.status()}): ${body}`,
      );
    }
  } finally {
    await dispose();
  }
}

export async function deleteAllBooks(
  page: Page,
  credentials?: E2eUserCredentials,
) {
  const resolvedCredentials =
    credentials ?? (await getPageE2eCredentials(page));
  const { apiContext, authHeaders, dispose } =
    await createAuthenticatedApiContext(resolvedCredentials);

  try {
    const booksResponse = await apiContext.get("/api/books/", {
      headers: authHeaders,
    });

    if (!booksResponse.ok()) {
      return;
    }

    const books = (await booksResponse.json()) as Array<{ id: string }>;
    for (const book of books) {
      await apiContext.delete(`/api/books/${book.id}`, { headers: authHeaders });
    }
  } finally {
    await dispose();
  }
}

export async function deleteAllReadingSessions(
  page: Page,
  credentials?: E2eUserCredentials,
) {
  const resolvedCredentials =
    credentials ?? (await getPageE2eCredentials(page));
  const { apiContext, authHeaders, dispose } =
    await createAuthenticatedApiContext(resolvedCredentials);

  try {
    const historyResponse = await apiContext.get("/api/reading-sessions/", {
      headers: authHeaders,
    });

    if (!historyResponse.ok()) {
      return;
    }

    const sessions = (await historyResponse.json()) as Array<{ id: string }>;
    for (const session of sessions) {
      await apiContext.delete(`/api/reading-sessions/${session.id}/history`, {
        headers: authHeaders,
      });
    }
  } finally {
    await dispose();
  }
}

export async function resetReadingState(
  page: Page,
  credentials?: E2eUserCredentials,
) {
  await cancelActiveReadingSessionIfAny(page, credentials);
  await deleteAllReadingSessions(page, credentials);
  await deleteAllBooks(page, credentials);
}

export type CreateBookOptions = {
  title?: string;
  author?: string;
  totalPages?: number;
  currentPage?: number;
  status?: "NotStarted" | "Ongoing" | "Finished";
  startingDate?: string;
};

export async function createBookViaUi(
  page: Page,
  options: CreateBookOptions = {},
) {
  const title = options.title ?? `E2E Book ${uniqueSuffix()}`;
  const author = options.author ?? "E2E Author";
  const totalPages = options.totalPages ?? 200;
  const currentPage = options.currentPage ?? 0;
  const status = options.status ?? "NotStarted";
  const startingDate =
    options.startingDate ?? new Date().toISOString().slice(0, 10);

  await gotoBooks(page);
  await page.getByRole("link", { name: "Add book" }).click();
  await expect(page.getByRole("heading", { name: "Add book" })).toBeVisible();

  const dialog = page.getByRole("dialog");
  await dialog.getByLabel("Title").fill(title);
  await dialog.getByLabel("Author").fill(author);
  await dialog.getByLabel("Total pages").fill(String(totalPages));
  await dialog.getByLabel("Current page").fill(String(currentPage));

  if (status !== "NotStarted") {
    await dialog.getByRole("combobox").click();
    await page.getByRole("option", { name: status }).click();

    if (status === "Ongoing" || status === "Finished") {
      await dialog.getByLabel("Starting date").fill(startingDate);
    }

    if (status === "Finished") {
      await dialog.getByLabel("Finish date").fill(startingDate);
      await dialog.getByRole("combobox", { name: "Rating (1–5)" }).click();
      await page.getByRole("option", { name: "5" }).click();
    }
  }

  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/books") &&
      apiResponse.request().method() === "POST" &&
      apiResponse.ok(),
  );

  await dialog.getByRole("button", { name: "Add book", exact: true }).click();
  await response;

  await expect(page.getByText(title, { exact: true })).toBeVisible({
    timeout: 15_000,
  });

  return title;
}

export async function openBookDetail(page: Page, bookTitle: string) {
  await page.getByRole("link", { name: bookTitle, exact: true }).click();
  await expect(
    page.locator('[aria-current="page"]').filter({ hasText: bookTitle }),
  ).toBeVisible({
    timeout: 15_000,
  });
}

export async function startReadingFromBookDetail(page: Page) {
  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/reading-sessions") &&
      apiResponse.request().method() === "POST" &&
      apiResponse.ok(),
  );

  await page.getByRole("button", { name: "Start reading" }).click();
  await response;

  await expect(page).toHaveURL(/\/reading\/ongoing-session/, { timeout: 15_000 });
}

export async function finishReadingSession(
  page: Page,
  endPage: number,
  options?: {
    chapterNumber?: string;
    chapterTitle?: string;
    noteContent?: string;
  },
) {
  if (
    options?.chapterNumber &&
    options?.chapterTitle &&
    options.noteContent
  ) {
    await saveReadingSessionNote(
      page,
      options.chapterNumber,
      options.chapterTitle,
      options.noteContent,
    );
  }

  await page.getByRole("button", { name: "Finish session" }).click();
  await expect(page.getByRole("heading", { name: "Finish session" })).toBeVisible();

  const dialog = page.getByRole("dialog");
  await dialog.getByLabel("End page").fill(String(endPage));

  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/reading-sessions") &&
      apiResponse.url().includes("/finish") &&
      apiResponse.request().method() === "POST" &&
      apiResponse.ok(),
  );

  await dialog.getByRole("button", { name: "Finish session" }).click();
  await response;
}

export async function cancelReadingSessionFromUi(page: Page) {
  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/reading-sessions") &&
      apiResponse.request().method() === "DELETE" &&
      apiResponse.ok(),
  );

  await page.getByRole("button", { name: "Cancel" }).click();
  await response;
}

export async function setDailyReadingGoal(page: Page, pagesPerDay: number) {
  await gotoReadingDashboard(page);
  await page.getByRole("link", { name: "Daily goal" }).click();

  await expect(
    page.getByRole("heading", { name: "Daily reading goal" }),
  ).toBeVisible();

  const dialog = page.getByRole("dialog");
  await dialog.getByLabel("Pages per day").fill(String(pagesPerDay));

  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/goals") && apiResponse.ok(),
  );
  const dashboardResponse = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/reading/dashboard") &&
      apiResponse.ok(),
  );

  await dialog.getByRole("button", { name: "Save goal" }).click();
  await response;

  await expect(page).toHaveURL(/\/reading\/dashboard/, { timeout: 15_000 });
  await dashboardResponse;
}

export async function openBookRowMenu(page: Page, bookTitle: string) {
  const row = page.getByRole("row").filter({ hasText: bookTitle });
  await row.getByRole("button", { name: "Open menu" }).click();
}

export async function editBookViaUi(
  page: Page,
  currentTitle: string,
  updatedTitle: string,
) {
  await openBookRowMenu(page, currentTitle);
  await page.getByRole("menuitem", { name: "Edit" }).click();

  const dialog = page.getByRole("dialog");
  await expect(page.getByRole("heading", { name: "Edit book" })).toBeVisible();
  await dialog.getByLabel("Title").fill(updatedTitle);

  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/books") &&
      apiResponse.request().method() === "PUT" &&
      apiResponse.ok(),
  );

  await dialog.getByRole("button", { name: "Save", exact: true }).click();
  await response;

  await expect(page.getByText(updatedTitle, { exact: true })).toBeVisible({
    timeout: 15_000,
  });
}

export async function deleteBookViaUi(page: Page, bookTitle: string) {
  await openBookRowMenu(page, bookTitle);
  await page.getByRole("menuitem", { name: "Delete" }).click();

  const dialog = page.getByRole("alertdialog");
  await dialog.getByPlaceholder("Book title").fill(bookTitle);

  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/books") &&
      apiResponse.request().method() === "DELETE" &&
      apiResponse.ok(),
  );

  await dialog.getByRole("button", { name: "Delete" }).click();
  await response;

  await expect(page.getByText(bookTitle, { exact: true })).not.toBeVisible();
}

export async function editSessionFromHistory(
  page: Page,
  bookTitle: string,
  options?: {
    endPage?: number;
    noteContent?: string;
  },
) {
  await gotoReadingHistory(page);

  const row = page.getByRole("row").filter({ hasText: bookTitle });
  await row.getByRole("link", { name: "Edit" }).click();

  await expect(page.getByRole("heading", { name: /Edit session/ })).toBeVisible();

  const dialog = page.getByRole("dialog");

  if (options?.endPage !== undefined) {
    await dialog.getByLabel("End page").fill(String(options.endPage));
  }

  if (options?.noteContent) {
    await dialog.getByLabel("Note").first().fill(options.noteContent);
  }

  const response = page.waitForResponse(
    (apiResponse) =>
      apiResponse.url().includes("/api/reading-sessions") &&
      apiResponse.request().method() === "PUT" &&
      apiResponse.ok(),
  );

  await dialog.getByRole("button", { name: "Save changes" }).click();
  await response;

  await expect(page).toHaveURL(/\/reading\/history/, { timeout: 15_000 });
}
