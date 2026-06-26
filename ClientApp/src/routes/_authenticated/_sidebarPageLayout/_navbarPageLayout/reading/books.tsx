import { createFileRoute, Outlet, useLocation } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import BooksTable from "@/features/reading/books/components/BooksTable";
import { booksQueryOptions } from "@/features/reading/books/queries/booksQuery";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/reading/books",
)({
  component: RouteComponent,
  loader: () => {
    queryClient.ensureQueryData(booksQueryOptions.list());
  },
});

function RouteComponent() {
  const pathname = useLocation({ select: (loc) => loc.pathname });
  const segments = pathname.split("/").filter(Boolean);
  const isBookDetail =
    segments.length === 3 &&
    segments[0] === "reading" &&
    segments[1] === "books" &&
    segments[2] !== "create";

  if (isBookDetail) {
    return <Outlet />;
  }

  return (
    <PageCard>
      <BooksTable />
      <Outlet />
    </PageCard>
  );
}
