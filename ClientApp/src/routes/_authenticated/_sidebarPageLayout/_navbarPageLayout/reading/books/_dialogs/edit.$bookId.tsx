import { createFileRoute } from "@tanstack/react-router";
import { useSuspenseQuery } from "@tanstack/react-query";

import BookDialog from "@/features/reading/books/components/BookDialog";
import { booksQueryOptions } from "@/features/reading/books/queries/booksQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/reading/books/_dialogs/edit/$bookId",
)({
  component: RouteComponent,
  loader: ({ params }) => ({
    bookId: params.bookId,
  }),
});

function RouteComponent() {
  const { bookId } = Route.useParams();
  const navigateBack = useNavigateBackOrDefault({ to: "/reading/books" });
  const { data: book } = useSuspenseQuery(booksQueryOptions.byId(bookId));

  return (
    <BookDialog dialogType="edit" book={book} onClose={navigateBack} />
  );
}
