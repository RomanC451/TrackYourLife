import { Separator } from "@/components/ui/separator";
import { cn } from "@/lib/utils";
import type { BookChapterNoteEntryDto } from "@/services/openapi";

import { getSharedNoteDate } from "../bookNotesUtils";

type ChapterNotesCardProps = {
  notes: BookChapterNoteEntryDto[];
  className?: string;
  contentClassName?: string;
};

function getNoteContentClassName(
  sharedDate: string | null,
  index: number,
  contentClassName?: string,
) {
  const spacing =
    sharedDate === null || index === 0 ? "mt-1.5" : undefined;

  return cn(
    "text-sm leading-relaxed text-foreground/90",
    spacing,
    contentClassName,
  );
}

function ChapterNotesCard({
  notes,
  className,
  contentClassName,
}: ChapterNotesCardProps) {
  const sharedDate = getSharedNoteDate(notes);

  return (
    <div
      className={cn(
        "rounded-lg border border-border bg-card p-4",
        className,
      )}
    >
      {sharedDate && (
        <span className="text-xs text-muted-foreground">{sharedDate}</span>
      )}
      {notes.map((note, index) => (
        <div key={note.noteId}>
          {index > 0 && <Separator className="my-3" />}
          {!sharedDate && (
            <span className="text-xs text-muted-foreground">{note.date}</span>
          )}
          <p className={getNoteContentClassName(sharedDate, index, contentClassName)}>
            {note.content}
          </p>
        </div>
      ))}
    </div>
  );
}

export default ChapterNotesCard;
