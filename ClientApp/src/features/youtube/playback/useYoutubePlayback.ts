import { useCallback, useRef } from "react";
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
  const pathnameRef = useRef(location.pathname);
  pathnameRef.current = location.pathname;
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
      const pathname = pathnameRef.current;

      openYoutubePlayer({ videoId });

      if (!syncRoute) {
        return;
      }

      const watchRoute = getYoutubeWatchRouteForPath(pathname);
      if (!watchRoute) {
        return;
      }

      if (isAlreadyOnWatchPath(pathname, videoId)) {
        return;
      }

      navigate({
        to: watchRoute,
        params: { videoId },
        // Keep the active category tab (and other list search params) on the parent route.
        search: (prev) => prev,
      });
    },
    [navigate, openYoutubePlayer],
  );

  return {
    openYoutubeVideo,
    closeYoutubeVideo: closeYoutubePlayer,
    minimizeYoutubeVideo: minimizeYoutubePlayer,
    currentVideoId: playerState?.videoId,
    isPlayPending,
  };
}
