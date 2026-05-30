import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { ChannelsApi, YoutubeChannelDto } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import {
  youtubeCategoryListFilterFavorites,
  youtubeQueryKeys,
  type YoutubeCategoryListFilter,
} from "../queries/youtubeQueries";

const channelsApi = new ChannelsApi();

type Variables = {
  youtubeChannelId: string;
  isFavorite: boolean;
};

function useSetChannelFavoriteMutation() {
  return useCustomMutation({
    mutationFn: ({ youtubeChannelId, isFavorite }: Variables) =>
      channelsApi.setChannelFavorite(youtubeChannelId, { isFavorite }),

    meta: {
      invalidateQueries: [
        youtubeQueryKeys.homeRecommendation(),
        youtubeQueryKeys.all,
        youtubeQueryKeys.channels(youtubeCategoryListFilterFavorites),
        youtubeQueryKeys.videos(youtubeCategoryListFilterFavorites, 5),
      ],
    },

    onMutate: async (variables) => {
      await queryClient.cancelQueries({
        queryKey: [...youtubeQueryKeys.all, "channels"],
      });

      const previous = queryClient.getQueriesData<YoutubeChannelDto[]>({
        queryKey: [...youtubeQueryKeys.all, "channels"],
      });

      for (const [queryKey] of previous) {
        const categoryFilter = queryKey[2] as YoutubeCategoryListFilter;
        queryClient.setQueryData<YoutubeChannelDto[]>(queryKey, (old) => {
          if (!old) {
            return old;
          }

          const updated = old.map((channel) =>
            channel.youtubeChannelId === variables.youtubeChannelId
              ? { ...channel, isFavorite: variables.isFavorite }
              : channel,
          );

          if (categoryFilter === youtubeCategoryListFilterFavorites) {
            return updated.filter((channel) => channel.isFavorite);
          }

          return updated;
        });
      }

      return { previous };
    },

    onError: (error: ApiError, _variables, context) => {
      for (const [key, data] of context?.previous ?? []) {
        queryClient.setQueryData(key, data);
      }

      const detail = error.response?.data?.detail;
      toast.error(typeof detail === "string" ? detail : "Could not update favorite");
    },
  });
}

export default useSetChannelFavoriteMutation;
