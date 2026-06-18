import { describe, expect, it } from "vitest";

import type { BookChapterNotesGroupDto } from "@/services/openapi";

import type { FlatBookNote } from "../bookNotesUtils";
import {
  flattenBookNotes,
  getNewestNoteChapterTitle,
  prependNoteToChapterGroups,
  sortBookNotesNewestFirst,
} from "../bookNotesUtils";

function note(
  partial: FlatBookNote,
): FlatBookNote {
  return partial;
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
});
