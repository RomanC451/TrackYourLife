import { useQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";

import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";

import { randomReadingNoteQueryOptions } from "@/features/reading/queries/readingQueries";

type ReadingNoteCardProps = {
  scope: "home" | "dashboard";
  className?: string;
};

function ReadingNoteCard({ scope, className }: ReadingNoteCardProps) {
  const { data: note, isLoading } = useQuery(
    randomReadingNoteQueryOptions(scope),
  );

  if (isLoading) {
    return (
      <Card className={className}>
        <CardHeader className="flex flex-row items-baseline justify-between gap-3 space-y-0">
          <Skeleton className="h-5 w-36" />
          <Skeleton className="h-4 w-28" />
        </CardHeader>
        <CardContent className="space-y-2 pt-0">
          <Skeleton className="h-4 w-full" />
          <Skeleton className="h-4 w-4/5" />
        </CardContent>
      </Card>
    );
  }

  if (!note) {
    return (
      <Card className={className}>
        <CardHeader>
          <CardTitle>Reading note</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-muted-foreground text-sm">
            No notes yet. Add chapter notes while reading and one will show up
            here.
          </p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className={className}>
      <CardHeader className="flex flex-row flex-wrap items-baseline justify-between gap-x-3 gap-y-1 space-y-0">
        <CardTitle className="min-w-0 text-base leading-snug">
          {note.chapterTitle}
        </CardTitle>
        <Link
          to="/reading/books/$bookId"
          params={{ bookId: note.bookId }}
          className="text-muted-foreground hover:text-foreground shrink-0 text-sm hover:underline"
        >
          {note.bookTitle}
        </Link>
      </CardHeader>
      <CardContent className="pt-0">
        <p className="text-sm leading-relaxed">{note.content}</p>
      </CardContent>
    </Card>
  );
}

export default ReadingNoteCard;
