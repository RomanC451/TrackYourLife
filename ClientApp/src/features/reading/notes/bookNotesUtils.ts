import type {
  BookChapterNoteEntryDto,
  BookChapterNotesGroupDto,
} from "@/services/openapi";

export type FlatBookNote = BookChapterNoteEntryDto & {
  chapterTitle: string;
  createdOnUtc?: string;
};

const CHAPTER_WITH_TITLE_PATTERN =
  /^(?:Chapter|Cap\.?)\s*(\d+)\s*[-—–]\s*(.*)$/i;
const CHAPTER_NUMBER_ONLY_PATTERN = /^(?:Chapter|Cap\.?)\s*(\d+)$/i;

export function formatChapterTitle(
  chapterNumber: string,
  title: string,
): string {
  return `Chapter ${chapterNumber.trim()} - ${title.trim()}`;
}

export function parseChapterTitle(fullChapterTitle: string): {
  chapterNumber: string;
  title: string;
} {
  const trimmed = fullChapterTitle.trim();
  const withTitle = CHAPTER_WITH_TITLE_PATTERN.exec(trimmed);
  if (withTitle) {
    return { chapterNumber: withTitle[1], title: withTitle[2].trim() };
  }

  const numberOnly = CHAPTER_NUMBER_ONLY_PATTERN.exec(trimmed);
  if (numberOnly) {
    return { chapterNumber: numberOnly[1], title: "" };
  }

  return { chapterNumber: "", title: trimmed };
}

export function getSharedNoteDate(
  notes: Pick<BookChapterNoteEntryDto, "date">[],
): string | null {
  if (notes.length === 0) {
    return null;
  }

  const firstDate = notes[0].date;
  return notes.every((note) => note.date === firstDate) ? firstDate : null;
}

export const COLLAPSED_NOTE_LIMIT = 3;

export function getVisibleChapterGroups(
  chapters: BookChapterNotesGroupDto[],
  expanded: boolean,
): BookChapterNotesGroupDto[] {
  if (expanded || chapters.length === 0) {
    return chapters;
  }

  const visible: BookChapterNotesGroupDto[] = [];
  let noteCount = 0;

  for (const chapter of chapters) {
    if (
      visible.length > 0 &&
      noteCount + chapter.notes.length > COLLAPSED_NOTE_LIMIT
    ) {
      break;
    }

    visible.push(chapter);
    noteCount += chapter.notes.length;
  }

  return visible.length > 0 ? visible : [chapters[0]];
}

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
