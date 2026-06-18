import { createFileRoute } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import ReadingDashboardPage from "@/features/reading/dashboard/components/ReadingDashboardPage";
import { readingDashboardQueryOptions } from "@/features/reading/queries/readingQueries";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/reading/dashboard",
)({
  component: RouteComponent,
  loader: () => {
    queryClient.ensureQueryData(readingDashboardQueryOptions.dashboard);
  },
});

function RouteComponent() {
  return (
    <PageCard>
      <ReadingDashboardPage />
    </PageCard>
  );
}
