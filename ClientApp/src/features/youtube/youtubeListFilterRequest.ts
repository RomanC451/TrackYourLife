import globalAxios from "axios";

import {
  ChannelsApi,
  VideosApi,
  type YoutubeChannelDto,
  type YoutubeVideoPreview,
} from "@/services/openapi";
import { BASE_PATH } from "@/services/openapi/base";

import {
  type YoutubeCategoryListFilter,
  youtubeCategoryListFilterFavorites,
  toYoutubeCategoryApiParam,
} from "./youtubeCategoryListFilters";

const channelsApi = new ChannelsApi();
const videosApi = new VideosApi();

/** OpenAPI client does not merge extra query params into the request URL; use axios directly. */
async function getWithFavoritesOnly<T>(path: string, params: Record<string, unknown>) {
  const { data } = await globalAxios.get<T>(`${BASE_PATH}${path}`, {
    params: { favoritesOnly: true, ...params },
  });
  return data;
}

export async function fetchYoutubeChannelsByListFilter(
  categoryFilter: YoutubeCategoryListFilter,
): Promise<YoutubeChannelDto[]> {
  if (categoryFilter === youtubeCategoryListFilterFavorites) {
    const data = await getWithFavoritesOnly<YoutubeChannelDto[]>("/api/channels/", {});
    return data.filter((channel) => channel.isFavorite);
  }

  const response = await channelsApi.getChannelsByCategory(
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

    const data = await getWithFavoritesOnly<YoutubeVideoPreview[]>("/api/videos/", {
      maxResultsPerChannel,
    });
    return filterVideosToFavoriteChannels(data, favoriteChannelIds);
  }

  const response = await videosApi.getAllLatestVideos(
    maxResultsPerChannel,
    toYoutubeCategoryApiParam(categoryFilter),
  );
  return response.data;
}
