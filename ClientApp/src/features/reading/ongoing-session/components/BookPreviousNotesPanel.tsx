import { useQuery } from "@tanstack/react-query";
import { ChevronDown } from "lucide-react";
import { useMemo, useState } from "react";

import { Button } from "@/components/ui/button";
import DashboardEyebrow from "@/features/reading/dashboard/components/DashboardEyebrow";
import { cn } from "@/lib/utils";
import ChapterNotesCard from "../../notes/components/ChapterNotesCard";
import {
  getVisibleChapterGroups,
  parseChapterTitle,
} from "../../notes/bookNotesUtils";
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

  const collapsedChapterGroups = useMemo(
    () => getVisibleChapterGroups(chapters, false),
    [chapters],
  );

  const visibleChapterGroups = expanded
    ? chapters
    : collapsedChapterGroups;

  const collapsedNoteCount = useMemo(
    () =>
      collapsedChapterGroups.reduce(
        (count, chapter) => count + chapter.notes.length,
        0,
      ),
    [collapsedChapterGroups],
  );

  const hiddenNoteCount = totalNoteCount - collapsedNoteCount;

  return (
    <section className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div className="space-y-1">
          <DashboardEyebrow>Your notes</DashboardEyebrow>
          <h2 className="text-lg font-semibold leading-none">Recent thoughts</h2>
        </div>
        <span className="text-muted-foreground text-sm">
          {totalNoteCount} {totalNoteCount === 1 ? "note" : "notes"}
        </span>
      </div>
      {totalNoteCount === 0 ? (
        <p className="text-muted-foreground text-sm">
          Notes you save during this session will show up here.
        </p>
      ) : (
        <>
          <div className="flex flex-col gap-6">
            {visibleChapterGroups.map((chapter) => {
              const parsed = parseChapterTitle(chapter.chapterTitle);

              return (
                <article
                  key={chapter.chapterTitle}
                  className="grid grid-cols-1 items-start gap-2 sm:grid-cols-[minmax(8.5rem,28%)_minmax(0,1fr)] sm:gap-4"
                >
                  <div className="border-l border-border py-0.5 pl-3">
                    <h3 className="text-sm font-semibold leading-snug">
                      {parsed.title || chapter.chapterTitle}
                    </h3>
                  </div>
                  <ChapterNotesCard
                    notes={chapter.notes}
                    className="min-w-0 bg-muted/40 p-3"
                  />
                </article>
              );
            })}
          </div>
          {hiddenNoteCount > 0 && (
            <Button
              type="button"
              variant="ghost"
              size="sm"
              onClick={() => setExpanded((value) => !value)}
              className="mx-auto flex gap-1.5 text-muted-foreground"
            >
              {expanded
                ? "Show fewer notes"
                : `Show ${hiddenNoteCount} older notes`}
              <ChevronDown
                className={cn(
                  "size-4 transition-transform",
                  expanded && "rotate-180",
                )}
                aria-hidden="true"
              />
            </Button>
          )}
        </>
      )}
    </section>
  );
}

export default BookPreviousNotesPanel;
