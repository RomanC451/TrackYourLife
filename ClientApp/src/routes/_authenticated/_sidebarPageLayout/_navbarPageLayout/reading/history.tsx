import { createFileRoute } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import ReadingHistoryPage from "@/features/reading/sessions/components/ReadingHistoryPage";
import { readingSessionsQueryOptions } from "@/features/reading/queries/readingQueries";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/reading/history",
)({
  component: RouteComponent,
  loader: () => {
    queryClient.ensureQueryData(readingSessionsQueryOptions.history);
  },
});

function RouteComponent() {
  return (
    <PageCard>
      <ReadingHistoryPage />
    </PageCard>
  );
}
