import { useEffect } from "react";
import { createFileRoute } from "@tanstack/react-router";

import { useYoutubePlayerHost } from "@/features/youtube/contexts/YoutubePlayerHostContext";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/videos/_dialogs/watch/$videoId",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const { videoId } = Route.useParams();
  const { openYoutubePlayer } = useYoutubePlayerHost();
  useEffect(() => {
    openYoutubePlayer({
      videoId,
    });
  }, [openYoutubePlayer, videoId]);

  return null;
}
