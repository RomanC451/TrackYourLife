import { describe, expect, it } from "vitest";

import type { BookChapterNotesGroupDto } from "@/services/openapi";

import type { FlatBookNote } from "../bookNotesUtils";
import {
  flattenBookNotes,
  formatChapterTitle,
  getNewestNoteChapterTitle,
  getSharedNoteDate,
  getVisibleChapterGroups,
  parseChapterTitle,
  prependNoteToChapterGroups,
  sortBookNotesNewestFirst,
} from "../bookNotesUtils";

function note(
  partial: FlatBookNote,
): FlatBookNote {
  return partial;
}

function chapterNote(
  partial: Partial<BookChapterNotesGroupDto["notes"][number]> &
    Pick<BookChapterNotesGroupDto["notes"][number], "noteId" | "content">,
): BookChapterNotesGroupDto["notes"][number] {
  return {
    sessionId: "session-1",
    date: "2026-06-22",
    isLoading: false,
    isDeleting: false,
    createdOnUtc: "2026-06-22T10:00:00Z",
    ...partial,
  };
}

describe("bookNotesUtils", () => {
  const groups: BookChapterNotesGroupDto[] = [
    {
      chapterTitle: "Cap. 4 — Focus",
      pageRange: undefined,
      isLoading: false,
      isDeleting: false,
      notes: [
        note({
          noteId: "note-1",
          sessionId: "session-1",
          chapterTitle: "Cap. 4 — Focus",
          date: "2026-06-10",
          content: "First note",
          createdOnUtc: "2026-06-10T10:00:00Z",
          isLoading: false,
          isDeleting: false,
        }),
      ],
    },
    {
      chapterTitle: "Cap. 3 — Distractions",
      pageRange: undefined,
      isLoading: false,
      isDeleting: false,
      notes: [
        note({
          noteId: "note-2",
          sessionId: "session-1",
          chapterTitle: "Cap. 3 — Distractions",
          date: "2026-06-15",
          content: "Newest chapter note",
          createdOnUtc: "2026-06-15T18:00:00Z",
          isLoading: false,
          isDeleting: false,
        }),
      ],
    },
  ];

  it("flattens chapter groups with chapter titles", () => {
    const notes = flattenBookNotes(groups);

    expect(notes).toHaveLength(2);
    expect(notes[0].chapterTitle).toBe("Cap. 4 — Focus");
    expect(notes[1].chapterTitle).toBe("Cap. 3 — Distractions");
  });

  it("sorts notes newest first", () => {
    const sorted = sortBookNotesNewestFirst(flattenBookNotes(groups));

    expect(sorted[0].content).toBe("Newest chapter note");
    expect(sorted[1].content).toBe("First note");
  });

  it("returns newest note chapter title", () => {
    expect(getNewestNoteChapterTitle(groups)).toBe("Cap. 3 — Distractions");
  });

  it("prepends a note to an existing chapter case-insensitively", () => {
    const updated = prependNoteToChapterGroups(groups, {
      noteId: "note-3",
      sessionId: "session-1",
      chapterTitle: "cap. 4 — focus",
      content: "Second note",
      createdOnUtc: "2026-06-15T12:00:00Z",
    });

    expect(updated[0].chapterTitle).toBe("cap. 4 — focus");
    expect(updated[0].notes).toHaveLength(2);
    expect(updated[0].notes[0].content).toBe("Second note");
  });

  it("creates a new chapter group when prepending an unknown chapter", () => {
    const updated = prependNoteToChapterGroups(groups, {
      noteId: "note-4",
      sessionId: "session-1",
      chapterTitle: "Cap. 5 — Momentum",
      content: "Brand new chapter",
      createdOnUtc: "2026-06-16T09:00:00Z",
    });

    expect(updated[0].chapterTitle).toBe("Cap. 5 — Momentum");
    expect(updated).toHaveLength(3);
  });

  it("formats chapter number and title for API requests", () => {
    expect(formatChapterTitle("8", "asdasdas")).toBe("Chapter 8 - asdasdas");
    expect(formatChapterTitle(" 5 ", " Flux ")).toBe("Chapter 5 - Flux");
  });

  it("parses stored chapter titles into number and title fields", () => {
    expect(parseChapterTitle("Chapter 8 - asdasdas")).toEqual({
      chapterNumber: "8",
      title: "asdasdas",
    });
    expect(parseChapterTitle("Cap. 5 — Flux și concentrare")).toEqual({
      chapterNumber: "5",
      title: "Flux și concentrare",
    });
    expect(parseChapterTitle("Cap. 1")).toEqual({
      chapterNumber: "1",
      title: "",
    });
    expect(parseChapterTitle("Introduction")).toEqual({
      chapterNumber: "",
      title: "Introduction",
    });
  });

  it("returns a shared date only when all notes match", () => {
    expect(
      getSharedNoteDate([
        { date: "2026-06-22" },
        { date: "2026-06-22" },
      ]),
    ).toBe("2026-06-22");
    expect(
      getSharedNoteDate([
        { date: "2026-06-22" },
        { date: "2026-06-21" },
      ]),
    ).toBeNull();
    expect(getSharedNoteDate([])).toBeNull();
  });

  it("limits collapsed chapter groups to three notes across chapters", () => {
    const chapters: BookChapterNotesGroupDto[] = [
      {
        chapterTitle: "Chapter 1",
        pageRange: undefined,
        isLoading: false,
        isDeleting: false,
        notes: [
          chapterNote({ noteId: "note-1", content: "first" }),
          chapterNote({ noteId: "note-2", content: "second" }),
        ],
      },
      {
        chapterTitle: "Chapter 2",
        pageRange: undefined,
        isLoading: false,
        isDeleting: false,
        notes: [
          chapterNote({
            noteId: "note-3",
            sessionId: "session-2",
            date: "2026-06-21",
            content: "third",
          }),
          chapterNote({
            noteId: "note-4",
            sessionId: "session-2",
            date: "2026-06-21",
            content: "fourth",
          }),
        ],
      },
    ];

    const collapsed = getVisibleChapterGroups(chapters, false);

    expect(collapsed).toHaveLength(1);
    expect(collapsed[0].chapterTitle).toBe("Chapter 1");
  });

  it("returns all chapter groups when expanded", () => {
    const chapters: BookChapterNotesGroupDto[] = [
      {
        chapterTitle: "Chapter 1",
        pageRange: undefined,
        isLoading: false,
        isDeleting: false,
        notes: [chapterNote({ noteId: "note-1", content: "first" })],
      },
      {
        chapterTitle: "Chapter 2",
        pageRange: undefined,
        isLoading: false,
        isDeleting: false,
        notes: [
          chapterNote({
            noteId: "note-2",
            sessionId: "session-2",
            date: "2026-06-21",
            content: "second",
          }),
        ],
      },
    ];

    expect(getVisibleChapterGroups(chapters, true)).toHaveLength(2);
  });
});
