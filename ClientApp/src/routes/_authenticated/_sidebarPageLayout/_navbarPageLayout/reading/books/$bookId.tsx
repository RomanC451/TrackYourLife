import { createFileRoute } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import BookDetailPage from "@/features/reading/books/components/BookDetailPage";
import { booksQueryOptions } from "@/features/reading/books/queries/booksQuery";
import { readingSessionsQueryOptions } from "@/features/reading/queries/readingQueries";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/reading/books/$bookId",
)({
  component: RouteComponent,
  loader: ({ params }) => {
    queryClient.ensureQueryData(booksQueryOptions.byId(params.bookId));
    queryClient.ensureQueryData(
      readingSessionsQueryOptions.bookChapterNotes(params.bookId),
    );
  },
});

function RouteComponent() {
  const { bookId } = Route.useParams();
  return (
    <PageCard>
      <BookDetailPage bookId={bookId} />
    </PageCard>
  );
}
