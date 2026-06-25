import { AxiosError, type AxiosResponse } from "axios";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { queryClient } from "@/queryClient";
import { createQueryFnContext } from "@/test/queryFnContext";

const { mockGetRandomReadingNote } = vi.hoisted(() => ({
  mockGetRandomReadingNote: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockReadingApi {
    getRandomReadingNote = mockGetRandomReadingNote;
    getReadingDashboard = vi.fn();
    getReadingStreak = vi.fn();
    getDailyReadingProgress = vi.fn();
    getReadingPagesHistory = vi.fn();
  }
  class MockReadingSessionsApi {
    getActiveReadingSession = vi.fn();
    getReadingSessionHistory = vi.fn();
    getBookReadingNotes = vi.fn();
    getReadingSessionNotes = vi.fn();
  }
  return {
    ...actual,
    ReadingApi: MockReadingApi,
    ReadingSessionsApi: MockReadingSessionsApi,
  };
});

import {
  randomReadingNoteQueryOptions,
  readingDashboardQueryKeys,
} from "../readingQueries";

describe("readingQueries random note", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("uses separate query keys per scope", () => {
    expect(readingDashboardQueryKeys.randomNote("home")).toEqual([
      "readingDashboard",
      "randomNote",
      "home",
    ]);
    expect(readingDashboardQueryKeys.randomNote("dashboard")).toEqual([
      "readingDashboard",
      "randomNote",
      "dashboard",
    ]);
  });

  it("configures random note queries to refetch on mount", () => {
    expect(randomReadingNoteQueryOptions("home").staleTime).toBe(0);
    expect(randomReadingNoteQueryOptions("home").refetchOnMount).toBe(
      "always",
    );
  });

  it("returns random note data from the API", async () => {
    mockGetRandomReadingNote.mockResolvedValue({
      data: {
        noteId: "note-1",
        bookId: "book-1",
        bookTitle: "Flow Book",
        chapterTitle: "Cap. 1 — Start",
        content: "A memorable quote",
      },
    });

    const options = randomReadingNoteQueryOptions("dashboard");
    const result = await options.queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: options.queryKey,
      }),
    );

    expect(mockGetRandomReadingNote).toHaveBeenCalledOnce();
    expect(result).toEqual({
      noteId: "note-1",
      bookId: "book-1",
      bookTitle: "Flow Book",
      chapterTitle: "Cap. 1 — Start",
      content: "A memorable quote",
    });
  });

  it("returns null when the API responds with 404", async () => {
    const axiosError = new AxiosError(
      "Not found",
      "ERR_BAD_REQUEST",
      undefined,
      undefined,
      { status: 404 } as AxiosResponse,
    );
    mockGetRandomReadingNote.mockRejectedValue(axiosError);

    const options = randomReadingNoteQueryOptions("home");
    const result = await options.queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: options.queryKey,
      }),
    );

    expect(result).toBeNull();
  });

  it("rethrows non-404 API failures", async () => {
    const axiosError = new AxiosError(
      "Server error",
      "ERR_BAD_RESPONSE",
      undefined,
      undefined,
      { status: 500 } as AxiosResponse,
    );
    mockGetRandomReadingNote.mockRejectedValue(axiosError);

    const options = randomReadingNoteQueryOptions("home");

    await expect(
      options.queryFn!(
        createQueryFnContext({
          client: queryClient,
          queryKey: options.queryKey,
        }),
      ),
    ).rejects.toBe(axiosError);
  });
});
