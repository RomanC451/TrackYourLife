import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import {
  AddChannelToCategoryRequest,
  ChannelsApi,
  YoutubeChannelDto,
} from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import {
  type YoutubeCategoryListFilter,
  youtubeQueryKeys,
} from "../queries/youtubeQueries";

const channelsApi = new ChannelsApi();

type Variables = AddChannelToCategoryRequest & {
  channelName: string;
  categoryName: string;
};

const channelListKeysToTouch = (
  youtubeCategoryId: string,
): YoutubeCategoryListFilter[] => [youtubeCategoryId, "all"];

function useAddChannelMutation() {
  const addChannelMutation = useCustomMutation({
    mutationFn: ({
      youtubeChannelId,
      youtubeCategoryId,
    }: Variables) => {
      return channelsApi.addChannelToCategory({
        youtubeChannelId,
        youtubeCategoryId,
      });
    },

    meta: {
      onSuccessToast: {
        message: "Channel added successfully",
        type: "success",
      },
      invalidateQueries: [youtubeQueryKeys.all],
    },

    onMutate: async (variables) => {
      const keys = channelListKeysToTouch(variables.youtubeCategoryId);
      await queryClient.cancelQueries({
        predicate: (q) =>
          q.queryKey[0] === "youtube" &&
          q.queryKey[1] === "channels" &&
          keys.includes(q.queryKey[2] as YoutubeCategoryListFilter),
      });

      const snapshots: [
        ReturnType<typeof youtubeQueryKeys.channels>,
        YoutubeChannelDto[] | undefined,
      ][] = keys.map((k) => [
        youtubeQueryKeys.channels(k),
        queryClient.getQueryData<YoutubeChannelDto[]>(
          youtubeQueryKeys.channels(k),
        ),
      ]);

      const optimisticChannel: YoutubeChannelDto = {
        id: `temp-${Date.now()}`,
        youtubeChannelId: variables.youtubeChannelId,
        name: variables.channelName,
        thumbnailUrl: undefined,
        youtubeCategoryId: variables.youtubeCategoryId,
        categoryName: variables.categoryName,
        isFavorite: false,
        createdOnUtc: new Date().toISOString(),
        isLoading: true,
        isDeleting: false,
      };

      for (const [queryKey, previous] of snapshots) {
        if (previous) {
          queryClient.setQueryData(queryKey, [...previous, optimisticChannel]);
        }
      }

      return { snapshots };
    },

    onError: (error: ApiError, _variables, context) => {
      for (const [queryKey, data] of context?.snapshots ?? []) {
        queryClient.setQueryData(queryKey, data);
      }

      const errorMessage =
        error.response?.data?.detail || "Failed to add channel";
      toast.error(errorMessage);
    },
  });

  return addChannelMutation;
}

export default useAddChannelMutation;
