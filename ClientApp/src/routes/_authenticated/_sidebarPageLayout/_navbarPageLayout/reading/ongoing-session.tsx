import { createFileRoute } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import OngoingReadingSessionPage from "@/features/reading/ongoing-session/components/OngoingReadingSessionPage";
import { readingSessionsQueryOptions } from "@/features/reading/queries/readingQueries";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/reading/ongoing-session",
)({
  component: RouteComponent,
  loader: async () => {
    const session = await queryClient.ensureQueryData(
      readingSessionsQueryOptions.active,
    );

    if (session?.bookId) {
      await queryClient.ensureQueryData(
        readingSessionsQueryOptions.bookChapterNotes(session.bookId),
      );
    }
  },
});

function RouteComponent() {
  return (
    <PageCard>
      <OngoingReadingSessionPage />
    </PageCard>
  );
}
