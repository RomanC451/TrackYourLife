import globalAxios from "axios";

import { ReadingSessionsApi } from "@/services/openapi";
import type { BookChapterNotesGroupDto } from "@/services/openapi";

import { flattenBookNotes } from "../notes/bookNotesUtils";

const readingSessionsApi = new ReadingSessionsApi();

export type ReadingSessionNoteDto = {
  id: string;
  chapterTitle: string;
  content: string;
  createdOnUtc: string;
};

export type UpdateReadingSessionNoteRequest = {
  chapterTitle: string;
  content: string;
};

function readField(
  record: Record<string, unknown>,
  ...keys: string[]
): string | undefined {
  for (const key of keys) {
    const value = record[key];
    if (typeof value === "string" && value.length > 0) {
      return value;
    }
  }

  return undefined;
}

function parseSessionNotesArray(data: unknown): ReadingSessionNoteDto[] {
  if (!Array.isArray(data)) {
    return [];
  }

  return data.flatMap((item) => {
    if (typeof item !== "object" || item === null) {
      return [];
    }

    const record = item as Record<string, unknown>;
    const id = readField(record, "id", "Id", "noteId", "NoteId");
    const chapterTitle = readField(record, "chapterTitle", "ChapterTitle");
    const content = readField(record, "content", "Content");

    if (!id || !chapterTitle || !content) {
      return [];
    }

    return [
      {
        id,
        chapterTitle,
        content,
        createdOnUtc:
          readField(record, "createdOnUtc", "CreatedOnUtc") ??
          new Date(0).toISOString(),
      },
    ];
  });
}

function notesForSessionFromGroups(
  groups: BookChapterNotesGroupDto[],
  sessionId: string,
): ReadingSessionNoteDto[] {
  const normalizedSessionId = sessionId.toLowerCase();

  return flattenBookNotes(groups)
    .filter(
      (note) => note.sessionId.toLowerCase() === normalizedSessionId,
    )
    .map((note) => ({
      id: note.noteId,
      chapterTitle: note.chapterTitle,
      content: note.content,
      createdOnUtc: note.createdOnUtc ?? note.date,
    }));
}

async function loadNotesFromBook(
  bookId: string,
  sessionId: string,
): Promise<ReadingSessionNoteDto[]> {
  const groups = await readingSessionsApi
    .getBookReadingNotes(bookId)
    .then((response) => response.data);

  return notesForSessionFromGroups(groups, sessionId);
}

export async function getReadingSessionNotes(
  sessionId: string,
  bookId: string,
): Promise<ReadingSessionNoteDto[]> {
  try {
    const { data, status } = await globalAxios.get<unknown>(
      `/api/reading-sessions/${sessionId}/notes`,
    );

    if (status === 200 && Array.isArray(data)) {
      const notes = parseSessionNotesArray(data);
      if (notes.length > 0) {
        return notes;
      }
    }
  } catch {
    // GET may not be deployed yet — fall back to book notes below.
  }

  return loadNotesFromBook(bookId, sessionId);
}

export async function updateReadingSessionNote(
  sessionId: string,
  noteId: string,
  body: UpdateReadingSessionNoteRequest,
) {
  await globalAxios.put(
    `/api/reading-sessions/${sessionId}/notes/${noteId}`,
    body,
  );
}

export async function deleteReadingSessionNote(
  sessionId: string,
  noteId: string,
) {
  await globalAxios.delete(
    `/api/reading-sessions/${sessionId}/notes/${noteId}`,
  );
}
