import { createFileRoute } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import WatchHistoryList from "@/features/youtube/watchHistory/components/WatchHistoryList";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/history",
)({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <PageCard>
      <PageTitle title="Watch history" />
      <WatchHistoryList />
    </PageCard>
  );
}
