import { VideoCategory, YoutubeApi } from "@/services/openapi";
import { retryQueryExcept404 } from "@/services/openapi/retry";

const youtubeApi = new YoutubeApi();

export const youtubeQueryKeys = {
  all: ["youtube"] as const,
  channels: (category?: VideoCategory | null) =>
    [...youtubeQueryKeys.all, "channels", category] as const,
  videos: (category?: VideoCategory | null, maxResultsPerChannel?: number) =>
    [
      ...youtubeQueryKeys.all,
      "videos",
      category,
      maxResultsPerChannel,
    ] as const,
  videoDetails: (videoId: string) =>
    [...youtubeQueryKeys.all, "video", videoId] as const,
  channelSearch: (query: string, maxResults: number = 10) =>
    [...youtubeQueryKeys.all, "channelSearch", query, maxResults] as const,
};

export const youtubeQueryOptions = {
  channels: (category?: VideoCategory | null) =>
    ({
      queryKey: youtubeQueryKeys.channels(category),
      queryFn: () =>
        youtubeApi.getChannelsByCategory(category).then((res) => res.data),
    }) as const,

  videos: (maxResultsPerChannel: number = 5, category?: VideoCategory | null) =>
    ({
      queryKey: youtubeQueryKeys.videos(category, maxResultsPerChannel),
      queryFn: () =>
        youtubeApi
          .getAllLatestVideos(maxResultsPerChannel, category)
          .then((res) => res.data),
    }) as const,

  videoDetails: (videoId: string) =>
    ({
      queryKey: youtubeQueryKeys.videoDetails(videoId),
      queryFn: () =>
        youtubeApi.getVideoDetails(videoId).then((res) => res.data),
    }) as const,

  channelSearch: (query: string, maxResults: number = 10) =>
    ({
      queryKey: youtubeQueryKeys.channelSearch(query, maxResults),
      queryFn: () =>
        youtubeApi
          .searchYoutubeChannels(query, maxResults)
          .then((res) => res.data),
      retry: retryQueryExcept404,
      enabled: query.length > 0,
    }) as const,
};
