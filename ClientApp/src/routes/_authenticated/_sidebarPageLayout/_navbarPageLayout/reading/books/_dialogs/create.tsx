import { createFileRoute } from "@tanstack/react-router";

import BookDialog from "@/features/reading/books/components/BookDialog";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/reading/books/_dialogs/create",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const navigateBack = useNavigateBackOrDefault({ to: "/reading/books" });
  return <BookDialog dialogType="create" onClose={navigateBack} />;
}
