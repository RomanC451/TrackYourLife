import { useNavigate } from "@tanstack/react-router";
import { StatusCodes } from "http-status-codes";
import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { VideosApi } from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";

import {
  youtubeMutationKeys,
  youtubeQueryKeys,
} from "../queries/youtubeQueries";
import { watchHistoryQueryKeys } from "../watchHistory/queries/useWatchHistoryQuery";
import { dedupePlayVideoRequest } from "./playVideoInFlight";

const videosApi = new VideosApi();

function usePlayVideoMutation() {
  const navigate = useNavigate();
  const playVideoMutation = useCustomMutation({
    mutationKey: youtubeMutationKeys.playVideo,
    mutationFn: (videoId: string) =>
      dedupePlayVideoRequest(videoId, () => videosApi.playVideo(videoId)),
    meta: {
      invalidateQueries: [
        youtubeQueryKeys.dailyCategoryWatchCounters(),
        watchHistoryQueryKeys.all,
      ],
    },

    onSuccess: (data, videoId) => {
      if (data?.data) {
        queryClient.setQueryData(
          youtubeQueryKeys.videoDetails(videoId),
          data.data,
        );
      }
    },
    onError: (error) => {
      handleApiError({
        error,
        errorHandlers: {
          [StatusCodes.FORBIDDEN]: {
            "Youtube.EntertainmentLimitReached": () => {
              toast.error("Limit reached.", {
                description: "You can change your limit in the settings.",
                action: {
                  label: "Go to settings",
                  onClick: () => {
                    navigate({ to: "/youtube/settings" });
                  },
                },
              });
            },
          },
        },
      });
    },
  });

  return playVideoMutation;
}

export default usePlayVideoMutation;
