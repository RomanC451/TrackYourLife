import { useCallback } from "react";
import { useMutationState } from "@tanstack/react-query";
import { useLocation, useNavigate } from "@tanstack/react-router";

import { useYoutubePlayerHost } from "../contexts/YoutubePlayerHostContext";
import { youtubeMutationKeys } from "../queries/youtubeQueries";
import {
  getYoutubeWatchRouteForPath,
  isAlreadyOnWatchPath,
} from "./youtubePlaybackRoutes";

export type OpenYoutubeVideoOptions = {
  /**
   * When false, opens the player without updating the URL.
   * Use from watch dialog routes that already encode the video id.
   */
  syncRoute?: boolean;
};

/**
 * Single entry point for opening a YouTube video: host player UI, optional URL sync, play API.
 */
export function useYoutubePlayback() {
  const navigate = useNavigate();
  const location = useLocation();
  const {
    openYoutubePlayer,
    closeYoutubePlayer,
    minimizeYoutubePlayer,
    playerState,
  } = useYoutubePlayerHost();

  const isPlayPending =
    useMutationState({
      filters: { mutationKey: youtubeMutationKeys.playVideo, status: "pending" },
    }).length > 0;

  const openYoutubeVideo = useCallback(
    (videoId: string, options: OpenYoutubeVideoOptions = {}) => {
      const { syncRoute = true } = options;

      openYoutubePlayer({ videoId });

      if (!syncRoute) {
        return;
      }

      const watchRoute = getYoutubeWatchRouteForPath(location.pathname);
      if (!watchRoute) {
        return;
      }

      if (isAlreadyOnWatchPath(location.pathname, videoId)) {
        return;
      }

      navigate({
        to: watchRoute,
        params: { videoId },
      });
    },
    [location.pathname, navigate, openYoutubePlayer],
  );

  return {
    openYoutubeVideo,
    closeYoutubeVideo: closeYoutubePlayer,
    minimizeYoutubeVideo: minimizeYoutubePlayer,
    currentVideoId: playerState?.videoId,
    isPlayPending,
  };
}
