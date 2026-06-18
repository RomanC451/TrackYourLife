import { queryOptions } from "@tanstack/react-query";

import {
  ReadingApi,
  ReadingSessionsApi,
} from "@/services/openapi";

const readingApi = new ReadingApi();
const readingSessionsApi = new ReadingSessionsApi();

export const readingSessionsQueryKeys = {
  all: ["readingSessions"] as const,
  active: ["readingSessions", "active"] as const,
  history: ["readingSessions", "history"] as const,
  bookChapterNotes: (bookId: string) =>
    ["readingSessions", "bookChapterNotes", bookId] as const,
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
};

export const readingDashboardQueryKeys = {
  all: ["readingDashboard"] as const,
  dashboard: ["readingDashboard", "summary"] as const,
  streak: ["readingDashboard", "streak"] as const,
  dailyProgress: (date?: string) =>
    ["readingDashboard", "dailyProgress", date ?? "today"] as const,
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
