import { createFileRoute } from "@tanstack/react-router";

import { useSyncWatchRoutePlayer } from "@/features/youtube/playback/useSyncWatchRoutePlayer";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/search/_dialogs/watch/$videoId",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const { videoId } = Route.useParams();
  useSyncWatchRoutePlayer(videoId);

  return null;
}
