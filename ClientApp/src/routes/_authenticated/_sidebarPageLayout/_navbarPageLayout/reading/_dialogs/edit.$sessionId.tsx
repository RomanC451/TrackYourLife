import { createFileRoute } from "@tanstack/react-router";
import { useSuspenseQuery } from "@tanstack/react-query";

import SessionEditDialog from "@/features/reading/sessions/components/SessionEditDialog";
import { readingSessionsQueryOptions } from "@/features/reading/queries/readingQueries";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/reading/_dialogs/edit/$sessionId",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const { sessionId } = Route.useParams();
  const navigateBack = useNavigateBackOrDefault({ to: "/reading/history" });
  const { data: sessions } = useSuspenseQuery(
    readingSessionsQueryOptions.history,
  );

  const session = sessions.find((s) => s.id === sessionId);

  if (!session) {
    navigateBack();
    return null;
  }

  return <SessionEditDialog session={session} onClose={navigateBack} />;
}
