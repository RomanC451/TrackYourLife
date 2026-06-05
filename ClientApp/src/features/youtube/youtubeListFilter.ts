import {
  ChannelsApi,
  VideosApi,
  type YoutubeChannelDto,
  type YoutubeVideoPreview,
} from "@/services/openapi";

import {
  type YoutubeCategoryListFilter,
  youtubeCategoryListFilterFavorites,
  toYoutubeCategoryApiParam,
} from "./youtubeCategoryListFilters";

const channelsApi = new ChannelsApi();
const videosApi = new VideosApi();

export async function fetchYoutubeChannelsByListFilter(
  categoryFilter: YoutubeCategoryListFilter,
): Promise<YoutubeChannelDto[]> {
  if (categoryFilter === youtubeCategoryListFilterFavorites) {
    const response = await channelsApi.getChannelsByCategory(true);
    return response.data;
  }

  const response = await channelsApi.getChannelsByCategory(
    false,
    toYoutubeCategoryApiParam(categoryFilter),
  );
  return response.data;
}

async function getFavoriteYoutubeChannelIds(): Promise<Set<string>> {
  const channels = await fetchYoutubeChannelsByListFilter(
    youtubeCategoryListFilterFavorites,
  );
  return new Set(channels.map((channel) => channel.youtubeChannelId));
}

function filterVideosToFavoriteChannels(
  videos: YoutubeVideoPreview[],
  favoriteChannelIds: Set<string>,
): YoutubeVideoPreview[] {
  return videos.filter(
    (video) => video.channelId.length > 0 && favoriteChannelIds.has(video.channelId),
  );
}

export async function fetchYoutubeVideosByListFilter(
  maxResultsPerChannel: number,
  categoryFilter: YoutubeCategoryListFilter,
): Promise<YoutubeVideoPreview[]> {
  if (categoryFilter === youtubeCategoryListFilterFavorites) {
    const favoriteChannelIds = await getFavoriteYoutubeChannelIds();
    if (favoriteChannelIds.size === 0) {
      return [];
    }

    const response = await videosApi.getAllLatestVideos(
      true,
      maxResultsPerChannel,
    );
    return filterVideosToFavoriteChannels(response.data, favoriteChannelIds);
  }

  const response = await videosApi.getAllLatestVideos(
    false,
    maxResultsPerChannel,
    toYoutubeCategoryApiParam(categoryFilter),
  );
  return response.data;
}
