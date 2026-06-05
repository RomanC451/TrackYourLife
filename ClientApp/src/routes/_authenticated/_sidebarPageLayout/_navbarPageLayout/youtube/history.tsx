import { createFileRoute } from "@tanstack/react-router";

import WatchHistoryPage from "@/pages/youtube/WatchHistoryPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/history",
)({
  component: RouteComponent,
});

function RouteComponent() {
  return <WatchHistoryPage />;
}
