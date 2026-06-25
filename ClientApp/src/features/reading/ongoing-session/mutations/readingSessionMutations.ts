import { useCustomMutation } from "@/hooks/useCustomMutation";
import {
  ReadingSessionsApi,
  type BookChapterNotesGroupDto,
  type FinishReadingSessionRequest,
  type UpdateReadingSessionRequest,
} from "@/services/openapi";
import { queryClient } from "@/queryClient";

import { prependNoteToChapterGroups } from "../../notes/bookNotesUtils";
import {
  deleteReadingSessionNote,
  updateReadingSessionNote,
} from "../../sessions/readingSessionNotesApi";
import { booksQueryKeys } from "../../books/queries/booksQuery";
import {
  readingDashboardQueryKeys,
  readingSessionsQueryKeys,
} from "../../queries/readingQueries";

const readingSessionsApi = new ReadingSessionsApi();

export const useStartReadingSessionMutation = () =>
  useCustomMutation({
    mutationFn: (bookId: string) =>
      readingSessionsApi
        .startReadingSession({ bookId })
        .then((r) => r.data),
    meta: {
      invalidateQueries: [
        readingSessionsQueryKeys.active,
        readingDashboardQueryKeys.all,
        booksQueryKeys.all,
      ],
    },
  });

export const useFinishReadingSessionMutation = () =>
  useCustomMutation({
    mutationFn: ({
      id,
      body,
    }: {
      id: string;
      body: FinishReadingSessionRequest;
    }) =>
      readingSessionsApi
        .finishReadingSession(id, body)
        .then((r) => r.data),
    meta: {
      invalidateQueries: [
        readingSessionsQueryKeys.all,
        readingDashboardQueryKeys.all,
        booksQueryKeys.all,
      ],
    },
  });

export const useCancelReadingSessionMutation = () =>
  useCustomMutation({
    mutationFn: (id: string) => readingSessionsApi.cancelReadingSession(id),
    meta: {
      invalidateQueries: [
        readingSessionsQueryKeys.active,
        readingDashboardQueryKeys.all,
      ],
    },
  });

export const useUpdateReadingSessionMutation = () =>
  useCustomMutation({
    mutationFn: ({
      id,
      body,
    }: {
      id: string;
      body: UpdateReadingSessionRequest;
    }) =>
      readingSessionsApi
        .updateReadingSession(id, body)
        .then((r) => r.data),
    meta: {
      invalidateQueries: [
        readingSessionsQueryKeys.all,
        readingDashboardQueryKeys.all,
        booksQueryKeys.all,
      ],
    },
  });

export const useAddReadingSessionNoteMutation = () =>
  useCustomMutation({
    mutationFn: ({
      sessionId,
      chapterTitle,
      content,
    }: {
      sessionId: string;
      bookId: string;
      chapterTitle: string;
      content: string;
    }) =>
      readingSessionsApi
        .addReadingSessionNote(sessionId, { chapterTitle, content })
        .then((r) => r.data.id),
    onSuccess: (noteId, { sessionId, bookId, chapterTitle, content }) => {
      const createdOnUtc = new Date().toISOString();

      queryClient.setQueryData<BookChapterNotesGroupDto[]>(
        readingSessionsQueryKeys.bookChapterNotes(bookId),
        (groups) =>
          prependNoteToChapterGroups(groups, {
            noteId,
            sessionId,
            chapterTitle,
            content,
            createdOnUtc,
          }),
      );
    },
    meta: {
      invalidateQueries: [
        readingSessionsQueryKeys.all,
        readingDashboardQueryKeys.all,
        ["readingSessions", "bookChapterNotes"],
        ["readingSessions", "sessionNotes"],
      ],
    },
  });

export const useUpdateReadingSessionNoteMutation = () =>
  useCustomMutation({
    mutationFn: ({
      sessionId,
      noteId,
      chapterTitle,
      content,
    }: {
      sessionId: string;
      noteId: string;
      chapterTitle: string;
      content: string;
    }) =>
      updateReadingSessionNote(sessionId, noteId, { chapterTitle, content }),
    meta: {
      invalidateQueries: [
        readingSessionsQueryKeys.all,
        readingDashboardQueryKeys.all,
        ["readingSessions", "bookChapterNotes"],
        ["readingSessions", "sessionNotes"],
      ],
    },
  });

export const useDeleteReadingSessionNoteMutation = () =>
  useCustomMutation({
    mutationFn: ({
      sessionId,
      noteId,
    }: {
      sessionId: string;
      noteId: string;
    }) => deleteReadingSessionNote(sessionId, noteId),
    meta: {
      invalidateQueries: [
        readingSessionsQueryKeys.all,
        readingDashboardQueryKeys.all,
        ["readingSessions", "bookChapterNotes"],
        ["readingSessions", "sessionNotes"],
      ],
    },
  });
