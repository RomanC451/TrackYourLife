import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import {
  AddChannelToCategoryRequest,
  YoutubeApi,
  YoutubeChannelDto,
} from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import { youtubeQueryKeys } from "../queries/youtubeQueries";

const youtubeApi = new YoutubeApi();

type Variables = AddChannelToCategoryRequest & {
  channelName: string;
};

function useAddChannelMutation() {
  const addChannelMutation = useCustomMutation({
    mutationFn: ({ youtubeChannelId, category }: Variables) => {
      return youtubeApi.addChannelToCategory({ youtubeChannelId, category });
    },

    meta: {
      onSuccessToast: {
        message: "Channel added successfully",
        type: "success",
      },
      invalidateQueries: [youtubeQueryKeys.all],
    },

    onMutate: async (variables) => {
      // Cancel any outgoing refetches
      await queryClient.cancelQueries({
        queryKey: youtubeQueryKeys.channels(variables.category),
      });

      // Snapshot previous data
      const previousChannels = queryClient.getQueryData<YoutubeChannelDto[]>(
        youtubeQueryKeys.channels(variables.category),
      );

      // Optimistically add the new channel
      if (previousChannels) {
        const optimisticChannel: YoutubeChannelDto = {
          id: `temp-${Date.now()}`,
          youtubeChannelId: variables.youtubeChannelId,
          name: variables.channelName,
          category: variables.category,
          createdOnUtc: new Date().toISOString(),
          isLoading: true,
          isDeleting: false,
        };

        queryClient.setQueryData(
          youtubeQueryKeys.channels(variables.category),
          [...previousChannels, optimisticChannel],
        );
      }

      return { previousChannels };
    },

    onError: (error: ApiError, variables, context) => {
      // Rollback on error
      if (context?.previousChannels) {
        queryClient.setQueryData(
          youtubeQueryKeys.channels(variables.category),
          context.previousChannels,
        );
      }

      const errorMessage =
        error.response?.data?.detail || "Failed to add channel";
      toast.error(errorMessage);
    },
  });

  return addChannelMutation;
}

export default useAddChannelMutation;
