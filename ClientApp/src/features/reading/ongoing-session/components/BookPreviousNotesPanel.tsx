import { useQuery } from "@tanstack/react-query";
import { ChevronDown } from "lucide-react";
import { useMemo, useState } from "react";

import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import ChapterNotesCard from "../../notes/components/ChapterNotesCard";
import { getVisibleChapterGroups } from "../../notes/bookNotesUtils";
import { readingSessionsQueryOptions } from "../../queries/readingQueries";

type BookPreviousNotesPanelProps = {
  bookId: string;
};

function BookPreviousNotesPanel({ bookId }: BookPreviousNotesPanelProps) {
  const { data: chapters = [] } = useQuery(
    readingSessionsQueryOptions.bookChapterNotes(bookId),
  );
  const [expanded, setExpanded] = useState(false);

  const totalNoteCount = useMemo(
    () => chapters.reduce((count, chapter) => count + chapter.notes.length, 0),
    [chapters],
  );

  const visibleChapterGroups = useMemo(
    () => getVisibleChapterGroups(chapters, expanded),
    [chapters, expanded],
  );

  const visibleNoteCount = useMemo(
    () =>
      visibleChapterGroups.reduce(
        (count, chapter) => count + chapter.notes.length,
        0,
      ),
    [visibleChapterGroups],
  );

  const hiddenNoteCount = totalNoteCount - visibleNoteCount;

  if (totalNoteCount === 0) {
    return null;
  }

  return (
    <section className="flex flex-col gap-3">
      <h2 className="text-sm font-medium text-muted-foreground">
        Previous notes
      </h2>
      <div className="flex flex-col gap-4">
        {visibleChapterGroups.map((chapter) => (
          <article
            key={chapter.chapterTitle}
            className="flex flex-col gap-2"
          >
            <h3 className="text-sm font-semibold text-foreground">
              {chapter.chapterTitle}
            </h3>
            <div className="border-l-2 border-border pl-3">
              <ChapterNotesCard notes={chapter.notes} />
            </div>
          </article>
        ))}
      </div>
      {hiddenNoteCount > 0 && (
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => setExpanded((value) => !value)}
          className="self-center gap-1.5 text-muted-foreground"
        >
          {expanded ? "Collapse" : `Expand (${hiddenNoteCount} more notes)`}
          <ChevronDown
            className={cn(
              "size-4 transition-transform",
              expanded && "rotate-180",
            )}
            aria-hidden="true"
          />
        </Button>
      )}
    </section>
  );
}

export default BookPreviousNotesPanel;
