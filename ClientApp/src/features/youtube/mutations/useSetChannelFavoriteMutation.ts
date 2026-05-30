import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { ChannelsApi, YoutubeChannelDto } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import { youtubeQueryKeys } from "../queries/youtubeQueries";

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
      invalidateQueries: [youtubeQueryKeys.homeRecommendation()],
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
              ? { ...channel, isFavorite: variables.isFavorite }
              : channel,
          ),
      );

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
