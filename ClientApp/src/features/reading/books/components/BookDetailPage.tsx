import { useSuspenseQuery } from "@tanstack/react-query";
import { Link, useNavigate } from "@tanstack/react-router";

import PageTitle from "@/components/common/PageTitle";
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
import BookStatusBadge from "@/features/reading/components/BookStatusBadge";
import SuggestFinishedBanner from "@/features/reading/components/SuggestFinishedBanner";
import ChapterNotesCard from "@/features/reading/notes/components/ChapterNotesCard";

import { useStartReadingSessionMutation } from "../../ongoing-session/mutations/readingSessionMutations";
import { readingSessionsQueryOptions } from "../../queries/readingQueries";
import { booksQueryOptions } from "../queries/booksQuery";

type BookDetailPageProps = {
  bookId: string;
};

function BookDetailPage({ bookId }: BookDetailPageProps) {
  const navigate = useNavigate();
  const { data: book } = useSuspenseQuery(booksQueryOptions.byId(bookId));
  const { data: chapterGroups } = useSuspenseQuery(
    readingSessionsQueryOptions.bookChapterNotes(bookId),
  );
  const startSessionMutation = useStartReadingSessionMutation();

  return (
    <>
      <PageTitle
        title={
          <Breadcrumb className="mb-4">
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
        }
      >
        <div className="flex gap-2">
          <Button asChild variant="outline">
            <Link to="/reading/books/edit/$bookId" params={{ bookId }}>
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
            Start reading
          </ButtonWithLoading>
        </div>
      </PageTitle>

      <div className="mb-4 space-y-1">
        <p className="text-muted-foreground">{book.author}</p>
        <BookStatusBadge status={book.status} />
        <p>
          Page {book.currentPage} of {book.totalPages}
        </p>
      </div>

      {book.suggestMarkAsFinished && <SuggestFinishedBanner />}

      <Card className="mt-6">
        <CardHeader>
          <CardTitle>Reading notes</CardTitle>
        </CardHeader>
        <CardContent className="space-y-6">
          {chapterGroups.length === 0 ? (
            <p className="text-muted-foreground text-sm">
              No notes from reading sessions yet.
            </p>
          ) : (
            <div className="space-y-4">
              {chapterGroups.map((chapter) => (
                <div key={chapter.chapterTitle} className="space-y-2">
                  <p className="text-sm font-semibold">{chapter.chapterTitle}</p>
                  <div className="border-l-2 border-border pl-3">
                    <ChapterNotesCard
                      notes={chapter.notes}
                      className="p-3"
                      contentClassName="text-foreground"
                    />
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </>
  );
}

export default BookDetailPage;
