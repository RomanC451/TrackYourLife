import { useSuspenseQuery } from "@tanstack/react-query";
import { Link, useNavigate } from "@tanstack/react-router";
import { BookOpen, Check, Pencil, Play } from "lucide-react";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import BookStatusBadge from "@/features/reading/components/BookStatusBadge";
import SuggestFinishedBanner from "@/features/reading/components/SuggestFinishedBanner";
import DashboardEyebrow from "@/features/reading/dashboard/components/DashboardEyebrow";
import ChapterNotesCard from "@/features/reading/notes/components/ChapterNotesCard";
import { getSharedNoteDate } from "@/features/reading/notes/bookNotesUtils";
import { cn } from "@/lib/utils";
import type { BookChapterNotesGroupDto, BookDto } from "@/services/openapi";

import { useStartReadingSessionMutation } from "../../ongoing-session/mutations/readingSessionMutations";
import { readingSessionsQueryOptions } from "../../queries/readingQueries";
import { booksQueryOptions } from "../queries/booksQuery";

type BookDetailPageProps = {
  bookId: string;
};

function getBookProgress(book: BookDto) {
  const percent =
    book.totalPages > 0
      ? Math.min(
          100,
          Math.round((book.currentPage / book.totalPages) * 100),
        )
      : 0;
  const pagesLeft = Math.max(0, book.totalPages - book.currentPage);

  return { percent, pagesLeft };
}

function BookDetailPage({ bookId }: BookDetailPageProps) {
  const navigate = useNavigate();
  const { data: book } = useSuspenseQuery(booksQueryOptions.byId(bookId));
  const { data: chapterGroups } = useSuspenseQuery(
    readingSessionsQueryOptions.bookChapterNotes(bookId),
  );
  const startSessionMutation = useStartReadingSessionMutation();
  const { percent, pagesLeft } = getBookProgress(book);

  return (
    <>
      <Breadcrumb>
        <BreadcrumbList>
          <BreadcrumbItem>
            <BreadcrumbLink asChild>
              <Link to="/reading/books">Books</Link>
            </BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbSeparator />
          <BreadcrumbItem className="min-w-0 max-w-full">
            <BreadcrumbPage className="truncate">{book.title}</BreadcrumbPage>
          </BreadcrumbItem>
        </BreadcrumbList>
      </Breadcrumb>

      <Card>
        <CardContent className="space-y-6 p-6">
          <div className="space-y-3">
            <div className="flex items-center gap-1.5">
              <BookOpen className="size-3.5 text-muted-foreground" />
              <DashboardEyebrow className="w-auto">Book details</DashboardEyebrow>
            </div>
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between sm:gap-4">
              <h1 className="min-w-0 text-3xl font-bold tracking-tight">
                {book.title}
              </h1>
              <div className="flex shrink-0 flex-wrap gap-2">
                <Button asChild variant="outline">
                  <Link to="/reading/books/edit/$bookId" params={{ bookId }}>
                    <Pencil />
                    Edit
                  </Link>
                </Button>
                <ButtonWithLoading
                  isLoading={startSessionMutation.isPending}
                  onClick={() =>
                    startSessionMutation.mutate(bookId, {
                      onSuccess: () =>
                        navigate({ to: "/reading/ongoing-session" }),
                    })
                  }
                >
                  <Play className="fill-current" />
                  Start reading
                </ButtonWithLoading>
              </div>
            </div>
            <p className="text-muted-foreground">{book.author}</p>
            <BookStatusBadge status={book.status} />
          </div>

          <div className="w-full space-y-2">
            <Progress value={percent} className="h-2.5 w-full" />
            <div className="text-muted-foreground flex flex-wrap items-center justify-between gap-2 text-sm">
              <p>
                Page {book.currentPage} of {book.totalPages}
              </p>
              <span>{percent}% complete</span>
              <span>
                {pagesLeft} {pagesLeft === 1 ? "page" : "pages"} left
              </span>
            </div>
          </div>
        </CardContent>
      </Card>

      {book.suggestMarkAsFinished && <SuggestFinishedBanner />}

      <Card>
        <CardHeader className="space-y-1">
          <DashboardEyebrow>Your journey</DashboardEyebrow>
          <CardTitle>Reading notes</CardTitle>
        </CardHeader>
        <CardContent>
          {chapterGroups.length === 0 ? (
            <p className="text-muted-foreground text-sm">
              No notes from reading sessions yet.
            </p>
          ) : (
            <ol className="space-y-0">
              {chapterGroups.map((chapter, index) => (
                <ChapterNoteGroup
                  key={chapter.chapterTitle}
                  chapter={chapter}
                  index={index}
                  isLast={index === chapterGroups.length - 1}
                />
              ))}
            </ol>
          )}
        </CardContent>
      </Card>
    </>
  );
}

function ChapterNoteGroup({
  chapter,
  index,
  isLast,
}: {
  chapter: BookChapterNotesGroupDto;
  index: number;
  isLast: boolean;
}) {
  const sharedDate = getSharedNoteDate(chapter.notes);
  const displayDate = sharedDate ?? chapter.notes[0]?.date;

  return (
    <li className="flex gap-4">
      <div className="flex w-7 shrink-0 flex-col items-center">
        <span
          className={cn(
            "flex size-7 items-center justify-center rounded-full text-xs font-medium",
            index === 0
              ? "bg-primary text-primary-foreground"
              : "bg-muted text-muted-foreground",
          )}
        >
          {index === 0 ? <Check className="size-3.5" /> : index + 1}
        </span>
        {!isLast && <span className="bg-border mt-1 w-px flex-1" />}
      </div>
      <div className={cn("min-w-0 flex-1", !isLast && "pb-6")}>
        <div className="mb-2 flex flex-wrap items-baseline justify-between gap-2">
          <h3 className="text-sm font-semibold">{chapter.chapterTitle}</h3>
          {displayDate && (
            <time className="text-muted-foreground text-xs">{displayDate}</time>
          )}
        </div>
        <ChapterNotesCard
          notes={chapter.notes}
          className="bg-muted/40 p-3"
          contentClassName="text-foreground"
          showDate={false}
        />
      </div>
    </li>
  );
}

export default BookDetailPage;
