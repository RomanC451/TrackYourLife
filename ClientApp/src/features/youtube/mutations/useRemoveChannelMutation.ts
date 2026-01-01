import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { ChannelsApi, YoutubeChannelDto } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import { youtubeQueryKeys } from "../queries/youtubeQueries";

const channelsApi = new ChannelsApi();

type Variables = {
  id: string;
  name: string;
};

function useRemoveChannelMutation() {
  const removeChannelMutation = useCustomMutation({
    mutationFn: ({ id }: Variables) => {
      return channelsApi.removeChannel(id);
    },

    meta: {
      onSuccessToast: {
        message: "Channel removed successfully",
        type: "success",
      },
      invalidateQueries: [youtubeQueryKeys.all],
    },

    onMutate: async (variables) => {
      // Cancel any outgoing refetches
      await queryClient.cancelQueries({
        queryKey: youtubeQueryKeys.all,
      });

      // Snapshot previous data for all channel queries
      const previousAllChannels = queryClient.getQueryData<YoutubeChannelDto[]>(
        youtubeQueryKeys.channels(),
      );
      const previousDivertissement = queryClient.getQueryData<
        YoutubeChannelDto[]
      >(youtubeQueryKeys.channels("Divertissement"));
      const previousEducational = queryClient.getQueryData<YoutubeChannelDto[]>(
        youtubeQueryKeys.channels("Educational"),
      );

      // Optimistically update all channel lists
      const updateChannelList = (channels: YoutubeChannelDto[] | undefined) => {
        if (!channels) return channels;
        return channels.map((channel) =>
          channel.id === variables.id
            ? { ...channel, isDeleting: true }
            : channel,
        );
      };

      if (previousAllChannels) {
        queryClient.setQueryData(
          youtubeQueryKeys.channels(),
          updateChannelList(previousAllChannels),
        );
      }
      if (previousDivertissement) {
        queryClient.setQueryData(
          youtubeQueryKeys.channels("Divertissement"),
          updateChannelList(previousDivertissement),
        );
      }
      if (previousEducational) {
        queryClient.setQueryData(
          youtubeQueryKeys.channels("Educational"),
          updateChannelList(previousEducational),
        );
      }

      return {
        previousAllChannels,
        previousDivertissement,
        previousEducational,
      };
    },

    onError: (error: ApiError, _variables, context) => {
      // Rollback on error
      if (context?.previousAllChannels) {
        queryClient.setQueryData(
          youtubeQueryKeys.channels(),
          context.previousAllChannels,
        );
      }
      if (context?.previousDivertissement) {
        queryClient.setQueryData(
          youtubeQueryKeys.channels("Divertissement"),
          context.previousDivertissement,
        );
      }
      if (context?.previousEducational) {
        queryClient.setQueryData(
          youtubeQueryKeys.channels("Educational"),
          context.previousEducational,
        );
      }

      const errorMessage =
        error.response?.data?.detail || "Failed to remove channel";
      toast.error(errorMessage);
    },
  });

  return removeChannelMutation;
}

export default useRemoveChannelMutation;
