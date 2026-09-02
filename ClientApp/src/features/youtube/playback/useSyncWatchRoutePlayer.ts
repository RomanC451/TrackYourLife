import { useEffect } from "react";

import { useYoutubePlayback } from "./useYoutubePlayback";

/**
 * Keeps the host player in sync with a watch dialog route.
 * Does not reopen (and therefore unminimize) when this video is already playing.
 */
export function useSyncWatchRoutePlayer(videoId: string) {
  const { openYoutubeVideo, currentVideoId } = useYoutubePlayback();

  useEffect(() => {
    if (currentVideoId === videoId) {
      return;
    }

    openYoutubeVideo(videoId, { syncRoute: false });
  }, [currentVideoId, openYoutubeVideo, videoId]);
}
