import { useQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { ChevronRight } from "lucide-react";

import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import DashboardEyebrow from "@/features/reading/dashboard/components/DashboardEyebrow";
import { randomReadingNoteQueryOptions } from "@/features/reading/queries/readingQueries";
import { cn } from "@/lib/utils";

type ReadingNoteCardProps = {
  scope: "home" | "dashboard";
  className?: string;
  layout?: "default" | "chapter";
};

function ReadingNoteCard({
  scope,
  className,
  layout = "default",
}: ReadingNoteCardProps) {
  const { data: note, isLoading } = useQuery(
    randomReadingNoteQueryOptions(scope),
  );

  if (isLoading) {
    return (
      <Card className={cn("flex flex-col", className)}>
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
      <Card className={cn("flex flex-col", className)}>
        <CardHeader>
          <CardTitle>
            {layout === "chapter" ? "Current chapter" : "Reading note"}
          </CardTitle>
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

  if (layout === "chapter") {
    return (
      <Card className={cn("flex flex-col", className)}>
        <CardHeader className="flex flex-row flex-wrap items-start justify-between gap-x-3 gap-y-2 space-y-0">
          <div className="min-w-0 space-y-1">
            <DashboardEyebrow>Current chapter</DashboardEyebrow>
            <CardTitle className="text-base leading-snug">
              {note.chapterTitle}
            </CardTitle>
          </div>
          <span className="bg-muted text-muted-foreground shrink-0 rounded-full px-2.5 py-1 text-xs font-medium">
            {note.bookTitle}
          </span>
        </CardHeader>
        <CardContent className="flex min-h-0 flex-1 flex-col justify-between gap-4 pt-0">
          <p className="text-sm leading-relaxed text-muted-foreground">
            {note.content}
          </p>
          <Link
            to="/reading/books/$bookId"
            params={{ bookId: note.bookId }}
            className="text-primary inline-flex items-center gap-1 text-sm font-medium hover:underline"
          >
            Continue reading
            <ChevronRight className="h-3.5 w-3.5" />
          </Link>
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
