import { keepPreviousData } from "@tanstack/react-query";

import {
  ChannelsApi,
  LibraryApi,
  SettingsApi,
  VideosApi,
} from "@/services/openapi";
import { retryQueryExcept404 } from "@/services/openapi/retry";

import { toYoutubeSettingsView } from "../data/youtubeSettingsTypes";
import type { YoutubeCategoryListFilter } from "../youtubeCategoryListFilters";
import {
  fetchYoutubeChannelsByListFilter,
  fetchYoutubeVideosByListFilter,
} from "../youtubeListFilterRequest";

const channelsApi = new ChannelsApi();
const videosApi = new VideosApi();
const settingsApi = new SettingsApi();
const libraryApi = new LibraryApi();

export {
  youtubeCategoryListFilterAll,
  youtubeCategoryListFilterFavorites,
  toYoutubeCategoryApiParam,
  type YoutubeCategoryListFilter,
  type YoutubeCategorySearchParam,
} from "../youtubeCategoryListFilters";

export const youtubeMutationKeys = {
  playVideo: ["youtube", "playVideo"] as const,
};

export const youtubeQueryKeys = {
  all: ["youtube"] as const,
  channels: (categoryFilter: YoutubeCategoryListFilter) =>
    [...youtubeQueryKeys.all, "channels", categoryFilter] as const,
  videos: (
    categoryFilter: YoutubeCategoryListFilter,
    maxResultsPerChannel?: number,
  ) =>
    [
      ...youtubeQueryKeys.all,
      "videos",
      categoryFilter,
      maxResultsPerChannel,
    ] as const,
  videoDetails: (videoId: string) =>
    [...youtubeQueryKeys.all, "video", videoId] as const,
  channelSearch: (query: string, maxResults: number = 10) =>
    [...youtubeQueryKeys.all, "channelSearch", query, maxResults] as const,
  videoSearch: (query: string, maxResults: number = 10) =>
    [...youtubeQueryKeys.all, "videoSearch", query, maxResults] as const,
  settings: () => [...youtubeQueryKeys.all, "settings"] as const,
  dailyCategoryWatchCounters: () =>
    [...youtubeQueryKeys.all, "dailyCategoryWatchCounters"] as const,
  libraryPlaylists: () =>
    [...youtubeQueryKeys.all, "libraryPlaylists"] as const,
  libraryPlaylist: (playlistId: string) =>
    [...youtubeQueryKeys.all, "libraryPlaylist", playlistId] as const,
  channelVideos: (youtubeChannelId: string, maxResults: number) =>
    [
      ...youtubeQueryKeys.all,
      "channelVideos",
      youtubeChannelId,
      maxResults,
    ] as const,
  homeRecommendation: () =>
    [...youtubeQueryKeys.all, "homeRecommendation"] as const,
};

export const youtubeQueryOptions = {
  channels: (categoryFilter: YoutubeCategoryListFilter) =>
    ({
      queryKey: youtubeQueryKeys.channels(categoryFilter),
      queryFn: () => fetchYoutubeChannelsByListFilter(categoryFilter),
    }) as const,

  videos: (
    maxResultsPerChannel: number = 5,
    categoryFilter: YoutubeCategoryListFilter = "all",
  ) =>
    ({
      queryKey: youtubeQueryKeys.videos(categoryFilter, maxResultsPerChannel),
      queryFn: () =>
        fetchYoutubeVideosByListFilter(maxResultsPerChannel, categoryFilter),
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
      placeholderData: keepPreviousData,
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
      placeholderData: keepPreviousData,
      enabled: query.length > 0,
    }) as const,

  settings: () =>
    ({
      queryKey: youtubeQueryKeys.settings(),
      queryFn: () =>
        settingsApi
          .getYoutubeSettings()
          .then((res) => toYoutubeSettingsView(res.data)),
    }) as const,

  dailyCategoryWatchCounters: () =>
    ({
      queryKey: youtubeQueryKeys.dailyCategoryWatchCounters(),
      queryFn: () =>
        settingsApi.getDailyCategoryWatchCounters().then((res) => res.data),
    }) as const,

  libraryPlaylists: () =>
    ({
      queryKey: youtubeQueryKeys.libraryPlaylists(),
      queryFn: () => libraryApi.getPlaylists().then((res) => res.data),
    }) as const,

  libraryPlaylist: (playlistId: string) =>
    ({
      queryKey: youtubeQueryKeys.libraryPlaylist(playlistId),
      queryFn: () =>
        libraryApi.getPlaylistById(playlistId).then((res) => res.data),
    }) as const,

  channelVideos: (youtubeChannelId: string, maxResults: number = 25) =>
    ({
      queryKey: youtubeQueryKeys.channelVideos(youtubeChannelId, maxResults),
      queryFn: () =>
        videosApi
          .getLatestVideosFromChannel(youtubeChannelId, maxResults)
          .then((res) => res.data),
    }) as const,

  homeRecommendation: () =>
    ({
      queryKey: youtubeQueryKeys.homeRecommendation(),
      queryFn: () =>
        videosApi
          .getHomeRecommendation()
          .then((res) => res.data.video ?? null),
      staleTime: 0,
      refetchOnWindowFocus: false,
    }) as const,
};
