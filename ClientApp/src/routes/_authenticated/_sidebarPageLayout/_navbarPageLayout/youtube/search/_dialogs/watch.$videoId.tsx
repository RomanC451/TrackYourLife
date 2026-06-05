import { useEffect } from "react";
import { createFileRoute } from "@tanstack/react-router";

import { useYoutubePlayback } from "@/features/youtube/playback/useYoutubePlayback";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/search/_dialogs/watch/$videoId",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const { videoId } = Route.useParams();
  const { openYoutubeVideo } = useYoutubePlayback();

  useEffect(() => {
    openYoutubeVideo(videoId, { syncRoute: false });
  }, [openYoutubeVideo, videoId]);

  return null;
}
