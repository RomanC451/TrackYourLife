import { beforeEach, describe, expect, it, vi } from "vitest";

import {
  youtubeChannel,
  youtubeSettings,
  youtubeVideo,
} from "@/features/youtube/__tests__/fixtures";

const {
  mockGetChannelsByCategory,
  mockGetAllLatestVideos,
  mockGetVideoDetails,
  mockSearchYoutubeChannels,
  mockSearchYoutubeVideos,
  mockGetYoutubeSettings,
  mockGetDailyCategoryWatchCounters,
  mockGetPlaylists,
  mockGetPlaylistById,
  mockGetLatestVideosFromChannel,
  mockGetHomeRecommendation,
} = vi.hoisted(() => ({
  mockGetChannelsByCategory: vi.fn(),
  mockGetAllLatestVideos: vi.fn(),
  mockGetVideoDetails: vi.fn(),
  mockSearchYoutubeChannels: vi.fn(),
  mockSearchYoutubeVideos: vi.fn(),
  mockGetYoutubeSettings: vi.fn(),
  mockGetDailyCategoryWatchCounters: vi.fn(),
  mockGetPlaylists: vi.fn(),
  mockGetPlaylistById: vi.fn(),
  mockGetLatestVideosFromChannel: vi.fn(),
  mockGetHomeRecommendation: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockChannelsApi {
    getChannelsByCategory = mockGetChannelsByCategory;
    searchYoutubeChannels = mockSearchYoutubeChannels;
  }
  class MockVideosApi {
    getAllLatestVideos = mockGetAllLatestVideos;
    getVideoDetails = mockGetVideoDetails;
    searchYoutubeVideos = mockSearchYoutubeVideos;
    getLatestVideosFromChannel = mockGetLatestVideosFromChannel;
    getHomeRecommendation = mockGetHomeRecommendation;
  }
  class MockSettingsApi {
    getYoutubeSettings = mockGetYoutubeSettings;
    getDailyCategoryWatchCounters = mockGetDailyCategoryWatchCounters;
  }
  class MockLibraryApi {
    getPlaylists = mockGetPlaylists;
    getPlaylistById = mockGetPlaylistById;
  }
  return {
    ...actual,
    ChannelsApi: MockChannelsApi,
    VideosApi: MockVideosApi,
    SettingsApi: MockSettingsApi,
    LibraryApi: MockLibraryApi,
  };
});

import {
  youtubeMutationKeys,
  youtubeQueryKeys,
  youtubeQueryOptions,
} from "../youtubeQueries";

describe("youtubeQueries", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("defines stable query and mutation keys", () => {
    expect(youtubeMutationKeys.playVideo).toEqual(["youtube", "playVideo"]);
    expect(youtubeQueryKeys.channels("all")).toEqual([
      "youtube",
      "channels",
      "all",
    ]);
    expect(youtubeQueryKeys.videos("cat-1", 5)).toEqual([
      "youtube",
      "videos",
      "cat-1",
      5,
    ]);
    expect(youtubeQueryKeys.videoDetails("video-1")).toEqual([
      "youtube",
      "video",
      "video-1",
    ]);
  });

  it("fetches channels and videos", async () => {
    mockGetChannelsByCategory.mockResolvedValue({
      data: [youtubeChannel("ch-1")],
    });
    mockGetAllLatestVideos.mockResolvedValue({ data: [youtubeVideo("v-1")] });

    await expect(
      youtubeQueryOptions.channels("all").queryFn(),
    ).resolves.toEqual([youtubeChannel("ch-1")]);
    await expect(
      youtubeQueryOptions.videos(5, "all").queryFn(),
    ).resolves.toEqual([youtubeVideo("v-1")]);
  });

  it("fetches search, settings, library, and recommendation data", async () => {
    mockGetVideoDetails.mockResolvedValue({ data: youtubeVideo("v-1") });
    mockSearchYoutubeChannels.mockResolvedValue({ data: [youtubeChannel("ch-1")] });
    mockSearchYoutubeVideos.mockResolvedValue({ data: [youtubeVideo("v-1")] });
    mockGetYoutubeSettings.mockResolvedValue({ data: youtubeSettings() });
    mockGetDailyCategoryWatchCounters.mockResolvedValue({ data: [] });
    mockGetPlaylists.mockResolvedValue({ data: [] });
    mockGetPlaylistById.mockResolvedValue({ data: { id: "pl-1" } });
    mockGetLatestVideosFromChannel.mockResolvedValue({ data: [youtubeVideo("v-1")] });
    mockGetHomeRecommendation.mockResolvedValue({
      data: { video: youtubeVideo("v-1") },
    });

    await expect(
      youtubeQueryOptions.videoDetails("v-1").queryFn(),
    ).resolves.toEqual(youtubeVideo("v-1"));
    await expect(
      youtubeQueryOptions.channelSearch("fitness").queryFn(),
    ).resolves.toEqual([youtubeChannel("ch-1")]);
    await expect(
      youtubeQueryOptions.videoSearch("squat").queryFn(),
    ).resolves.toEqual([youtubeVideo("v-1")]);
    await expect(youtubeQueryOptions.settings().queryFn()).resolves.toEqual({
      ...youtubeSettings(),
      hasSettingsPassword: false,
    });
    await expect(
      youtubeQueryOptions.dailyCategoryWatchCounters().queryFn(),
    ).resolves.toEqual([]);
    await expect(youtubeQueryOptions.libraryPlaylists().queryFn()).resolves.toEqual(
      [],
    );
    await expect(
      youtubeQueryOptions.libraryPlaylist("pl-1").queryFn(),
    ).resolves.toEqual({ id: "pl-1" });
    await expect(
      youtubeQueryOptions.channelVideos("ch-1", 10).queryFn(),
    ).resolves.toEqual([youtubeVideo("v-1")]);
    await expect(
      youtubeQueryOptions.homeRecommendation().queryFn(),
    ).resolves.toEqual(youtubeVideo("v-1"));
  });

  it("disables search queries for empty terms", () => {
    expect(youtubeQueryOptions.channelSearch("").enabled).toBe(false);
    expect(youtubeQueryOptions.videoSearch("").enabled).toBe(false);
  });
});
