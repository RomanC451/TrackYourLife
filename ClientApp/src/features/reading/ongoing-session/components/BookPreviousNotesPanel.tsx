import { useQuery } from "@tanstack/react-query";
import { ChevronDown } from "lucide-react";
import { useMemo, useState } from "react";

import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

import {
  flattenBookNotes,
  sortBookNotesNewestFirst,
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

  const notes = useMemo(
    () => sortBookNotesNewestFirst(flattenBookNotes(chapters)),
    [chapters],
  );

  const visibleNotes = useMemo(
    () => (expanded ? notes : notes.slice(0, 3)),
    [expanded, notes],
  );

  if (notes.length === 0) {
    return null;
  }

  return (
    <section className="flex flex-col gap-3">
      <h2 className="text-sm font-medium text-muted-foreground">
        Previous notes
      </h2>
      <div className="flex flex-col gap-4">
        {visibleNotes.map((note) => (
          <article key={note.noteId} className="flex flex-col gap-2">
            <div className="flex items-baseline justify-between gap-3">
              <h3 className="text-sm font-semibold text-foreground">
                {note.chapterTitle}
              </h3>
            </div>
            <div className="border-l-2 border-border pl-3">
              <div className="rounded-lg border border-border bg-card p-4">
                <span className="text-xs text-muted-foreground">{note.date}</span>
                <p className="mt-1.5 text-sm leading-relaxed text-foreground/90">
                  {note.content}
                </p>
              </div>
            </div>
          </article>
        ))}
      </div>
      {notes.length > 3 && (
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => setExpanded((value) => !value)}
          className="self-center gap-1.5 text-muted-foreground"
        >
          {expanded ? "Collapse" : `Expand (${notes.length - 3} more notes)`}
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
