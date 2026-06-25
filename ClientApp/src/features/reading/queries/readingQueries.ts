import { keepPreviousData, queryOptions } from "@tanstack/react-query";
import { isAxiosError } from "axios";

import {
  ReadingApi,
  ReadingOverviewType,
  ReadingSessionsApi,
} from "@/services/openapi";

const readingApi = new ReadingApi();
const readingSessionsApi = new ReadingSessionsApi();

async function getRandomReadingNote() {
  try {
    const { data } = await readingApi.getRandomReadingNote();
    return data;
  } catch (error) {
    if (isAxiosError(error) && error.response?.status === 404) {
      return null;
    }

    throw error;
  }
}

export const readingSessionsQueryKeys = {
  all: ["readingSessions"] as const,
  active: ["readingSessions", "active"] as const,
  history: ["readingSessions", "history"] as const,
  bookChapterNotes: (bookId: string) =>
    ["readingSessions", "bookChapterNotes", bookId] as const,
  sessionNotes: (sessionId: string) =>
    ["readingSessions", "sessionNotes", sessionId] as const,
};

export const readingSessionsQueryOptions = {
  active: queryOptions({
    queryKey: readingSessionsQueryKeys.active,
    queryFn: () =>
      readingSessionsApi.getActiveReadingSession().then((r) => r.data),
  }),
  history: queryOptions({
    queryKey: readingSessionsQueryKeys.history,
    queryFn: () =>
      readingSessionsApi.getReadingSessionHistory().then((r) => r.data),
  }),
  bookChapterNotes: (bookId: string) =>
    queryOptions({
      queryKey: readingSessionsQueryKeys.bookChapterNotes(bookId),
      queryFn: () =>
        readingSessionsApi.getBookReadingNotes(bookId).then((r) => r.data),
    }),
  sessionNotes: (sessionId: string) =>
    queryOptions({
      queryKey: readingSessionsQueryKeys.sessionNotes(sessionId),
      queryFn: () =>
        readingSessionsApi.getReadingSessionNotes(sessionId).then((r) => r.data),
      staleTime: 0,
    }),
};

export const readingDashboardQueryKeys = {
  all: ["readingDashboard"] as const,
  dashboard: ["readingDashboard", "summary"] as const,
  streak: ["readingDashboard", "streak"] as const,
  dailyProgress: (date?: string) =>
    ["readingDashboard", "dailyProgress", date ?? "today"] as const,
  randomNote: (scope: "home" | "dashboard") =>
    ["readingDashboard", "randomNote", scope] as const,
};

export const readingDashboardQueryOptions = {
  dashboard: queryOptions({
    queryKey: readingDashboardQueryKeys.dashboard,
    queryFn: () => readingApi.getReadingDashboard().then((r) => r.data),
  }),
  streak: queryOptions({
    queryKey: readingDashboardQueryKeys.streak,
    queryFn: () => readingApi.getReadingStreak().then((r) => r.data),
  }),
  dailyProgress: (date?: string) =>
    queryOptions({
      queryKey: readingDashboardQueryKeys.dailyProgress(date),
      queryFn: () =>
        readingApi.getDailyReadingProgress(date).then((r) => r.data),
    }),
};

export const randomReadingNoteQueryOptions = (scope: "home" | "dashboard") =>
  queryOptions({
    queryKey: readingDashboardQueryKeys.randomNote(scope),
    queryFn: getRandomReadingNote,
    staleTime: 0,
    refetchOnMount: "always",
  });

export const readingPagesHistoryQueryKeys = {
  all: ["readingPagesHistory"] as const,
  byOverviewType: (overviewType: ReadingOverviewType) =>
    [...readingPagesHistoryQueryKeys.all, overviewType] as const,
};

export const readingPagesHistoryQueryOptions = {
  byOverviewType: (overviewType: ReadingOverviewType = "Weekly") =>
    queryOptions({
      queryKey: readingPagesHistoryQueryKeys.byOverviewType(overviewType),
      queryFn: () =>
        readingApi.getReadingPagesHistory(overviewType).then((r) => r.data),
      placeholderData: keepPreviousData,
    }),
};
