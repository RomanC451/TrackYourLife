import { useNavigate } from "@tanstack/react-router";
import { StatusCodes } from "http-status-codes";
import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { VideosApi } from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";

import { youtubeQueryKeys } from "../queries/youtubeQueries";

const videosApi = new VideosApi();

function usePlayVideoMutation() {
  const navigate = useNavigate();
  const playVideoMutation = useCustomMutation({
    mutationFn: (videoId: string) => {
      return videosApi.playVideo(videoId);
    },
    meta: {
      invalidateQueries: [youtubeQueryKeys.dailyCounter()],
    },

    onSuccess: (data, videoId) => {
      // Cache the video details in the query cache so VideoPlayerDialog can use it
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
