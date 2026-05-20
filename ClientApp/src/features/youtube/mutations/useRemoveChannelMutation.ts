import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { ChannelsApi, YoutubeChannelDto } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import { youtubeQueryKeys } from "../queries/youtubeQueries";

const channelsApi = new ChannelsApi();

type Variables = {
  youtubeChannelId: string;
};

function useRemoveChannelMutation() {
  const removeChannelMutation = useCustomMutation({
    mutationFn: ({ youtubeChannelId }: Variables) => {
      return channelsApi.removeChannel(youtubeChannelId);
    },

    meta: {
      onSuccessToast: {
        message: "Channel removed successfully",
        type: "success",
      },
      invalidateQueries: [youtubeQueryKeys.all],
    },

    onMutate: async (variables) => {
      await queryClient.cancelQueries({
        queryKey: [...youtubeQueryKeys.all, "channels"],
      });

      const previous = queryClient.getQueriesData<YoutubeChannelDto[]>({
        queryKey: [...youtubeQueryKeys.all, "channels"],
      });

      queryClient.setQueriesData<YoutubeChannelDto[]>(
        { queryKey: [...youtubeQueryKeys.all, "channels"] },
        (old) =>
          old?.map((channel) =>
            channel.youtubeChannelId === variables.youtubeChannelId
              ? { ...channel, isDeleting: true }
              : channel,
          ),
      );

      return { previous };
    },

    onError: (error: ApiError, _variables, context) => {
      for (const [key, data] of context?.previous ?? []) {
        queryClient.setQueryData(key, data);
      }

      const errorMessage =
        error.response?.data?.detail || "Failed to remove channel";
      toast.error(errorMessage);
    },
  });

  return removeChannelMutation;
}

export default useRemoveChannelMutation;
