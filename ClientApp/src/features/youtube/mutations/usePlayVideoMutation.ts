import { useNavigate } from "@tanstack/react-router";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { VideosApi } from "@/services/openapi";

import { youtubeQueryKeys } from "../queries/youtubeQueries";

const videosApi = new VideosApi();

function usePlayVideoMutation() {
  const navigate = useNavigate();
  const playVideoMutation = useCustomMutation({
    mutationFn: (videoId: string) => {
      return videosApi.playVideo(videoId);
    },
    meta: {
      onErrorToast: {
        message: "Limit reached.",
        type: "error",
        data: {
          description: "You can change your limit in the settings.",
          action: {
            label: "Go to settings",
            onClick: () => {
              navigate({ to: "/youtube/settings" });
            },
          },
        },
      },
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
  });

  return playVideoMutation;
}

export default usePlayVideoMutation;
