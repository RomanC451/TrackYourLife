import { createFileRoute, Outlet, useLocation } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import BooksTable from "@/features/reading/books/components/BooksTable";
import { booksQueryOptions } from "@/features/reading/books/queries/booksQuery";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/books",
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
    segments.length === 2 &&
    segments[0] === "books" &&
    segments[1] !== "create";

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
