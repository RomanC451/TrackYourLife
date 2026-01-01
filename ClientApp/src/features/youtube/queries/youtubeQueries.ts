import {
  ChannelsApi,
  SettingsApi,
  VideoCategory,
  VideosApi,
} from "@/services/openapi";
import { retryQueryExcept404 } from "@/services/openapi/retry";

const channelsApi = new ChannelsApi();
const videosApi = new VideosApi();
const settingsApi = new SettingsApi();

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
  videoSearch: (query: string, maxResults: number = 10) =>
    [...youtubeQueryKeys.all, "videoSearch", query, maxResults] as const,
  settings: () => [...youtubeQueryKeys.all, "settings"] as const,
  dailyCounter: () => [...youtubeQueryKeys.all, "dailyCounter"] as const,
};

export const youtubeQueryOptions = {
  channels: (category?: VideoCategory | null) =>
    ({
      queryKey: youtubeQueryKeys.channels(category),
      queryFn: () =>
        channelsApi.getChannelsByCategory(category).then((res) => res.data),
    }) as const,

  videos: (maxResultsPerChannel: number = 5, category?: VideoCategory | null) =>
    ({
      queryKey: youtubeQueryKeys.videos(category, maxResultsPerChannel),
      queryFn: () =>
        videosApi
          .getAllLatestVideos(maxResultsPerChannel, category)
          .then((res) => res.data),
    }) as const,

  videoDetails: (videoId: string) =>
    ({
      queryKey: youtubeQueryKeys.videoDetails(videoId),
      queryFn: () => videosApi.getVideoDetails(videoId).then((res) => res.data),
    }) as const,

  channelSearch: (query: string, maxResults: number = 10) =>
    ({
      queryKey: youtubeQueryKeys.channelSearch(query, maxResults),
      queryFn: () =>
        channelsApi
          .searchYoutubeChannels(query, maxResults)
          .then((res) => res.data),
      retry: retryQueryExcept404,
      enabled: query.length > 0,
    }) as const,

  videoSearch: (query: string, maxResults: number = 10) =>
    ({
      queryKey: youtubeQueryKeys.videoSearch(query, maxResults),
      queryFn: () =>
        videosApi
          .searchYoutubeVideos(query, maxResults)
          .then((res) => res.data),
      retry: retryQueryExcept404,
      enabled: query.length > 0,
    }) as const,

  settings: () =>
    ({
      queryKey: youtubeQueryKeys.settings(),
      queryFn: () => settingsApi.getYoutubeSettings().then((res) => res.data),
    }) as const,

  dailyCounter: () =>
    ({
      queryKey: youtubeQueryKeys.dailyCounter(),
      queryFn: () =>
        settingsApi.getDailyDivertissmentCounter().then((res) => res.data),
    }) as const,
};
