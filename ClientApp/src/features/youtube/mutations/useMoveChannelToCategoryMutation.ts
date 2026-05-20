import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { ChannelsApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import { youtubeQueryKeys } from "../queries/youtubeQueries";

const channelsApi = new ChannelsApi();

type Variables = {
  youtubeChannelId: string;
  targetYoutubeCategoryId: string;
};

function useMoveChannelToCategoryMutation() {
  return useCustomMutation({
    mutationFn: ({ youtubeChannelId, targetYoutubeCategoryId }: Variables) =>
      channelsApi.moveChannelToCategory(youtubeChannelId, {
        youtubeCategoryId: targetYoutubeCategoryId,
      }),

    meta: {
      onSuccessToast: {
        message: "Channel moved to category",
        type: "success",
      },
      invalidateQueries: [youtubeQueryKeys.all],
    },

    onError: (error: ApiError) => {
      const detail = error.response?.data?.detail;
      toast.error(typeof detail === "string" ? detail : "Could not move channel");
    },
  });
}

export default useMoveChannelToCategoryMutation;
