import type {
  BookChapterNoteEntryDto,
  BookChapterNotesGroupDto,
} from "@/services/openapi";

export type FlatBookNote = BookChapterNoteEntryDto & {
  chapterTitle: string;
  createdOnUtc?: string;
};

export function flattenBookNotes(
  groups: BookChapterNotesGroupDto[],
): FlatBookNote[] {
  return groups.flatMap((group) =>
    group.notes.map((note) => ({
      ...note,
      chapterTitle: group.chapterTitle,
      createdOnUtc: (note as FlatBookNote).createdOnUtc,
    })),
  );
}

export function sortBookNotesNewestFirst(notes: FlatBookNote[]): FlatBookNote[] {
  if (!notes.every((note) => note.createdOnUtc)) {
    return notes;
  }

  return [...notes].sort((a, b) => {
    const aTime = Date.parse(a.createdOnUtc!);
    const bTime = Date.parse(b.createdOnUtc!);

    if (aTime !== bTime) {
      return bTime - aTime;
    }

    return b.noteId.localeCompare(a.noteId);
  });
}

export function getNewestNoteChapterTitle(
  groups: BookChapterNotesGroupDto[] | undefined,
): string | undefined {
  if (!groups?.length) {
    return undefined;
  }

  const newest = sortBookNotesNewestFirst(flattenBookNotes(groups))[0];
  return newest?.chapterTitle;
}

export function prependNoteToChapterGroups(
  groups: BookChapterNotesGroupDto[] | undefined,
  note: {
    noteId: string;
    sessionId: string;
    chapterTitle: string;
    content: string;
    createdOnUtc: string;
  },
): BookChapterNotesGroupDto[] {
  const date = note.createdOnUtc.slice(0, 10);
  const entry = {
    noteId: note.noteId,
    sessionId: note.sessionId,
    chapterTitle: note.chapterTitle,
    date,
    content: note.content,
    createdOnUtc: note.createdOnUtc,
    isLoading: false,
    isDeleting: false,
  } satisfies FlatBookNote;

  if (!groups?.length) {
    return [
      {
        chapterTitle: note.chapterTitle,
        pageRange: undefined,
        notes: [entry],
        isLoading: false,
        isDeleting: false,
      },
    ];
  }

  const existingIndex = groups.findIndex(
    (group) =>
      group.chapterTitle.toLowerCase() === note.chapterTitle.toLowerCase(),
  );

  if (existingIndex >= 0) {
    const existingGroup = groups[existingIndex];
    const updatedGroup: BookChapterNotesGroupDto = {
      ...existingGroup,
      chapterTitle: note.chapterTitle,
      notes: [entry, ...existingGroup.notes],
    };

    return [
      updatedGroup,
      ...groups.filter((_, index) => index !== existingIndex),
    ];
  }

  return [
    {
      chapterTitle: note.chapterTitle,
      pageRange: undefined,
      notes: [entry],
      isLoading: false,
      isDeleting: false,
    },
    ...groups,
  ];
}
