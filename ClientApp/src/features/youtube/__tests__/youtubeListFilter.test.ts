import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeChannel, youtubeVideo } from "./fixtures";
import {
  fetchYoutubeChannelsByListFilter,
  fetchYoutubeVideosByListFilter,
} from "../youtubeListFilter";

const { mockGetChannelsByCategory, mockGetAllLatestVideos } = vi.hoisted(() => ({
  mockGetChannelsByCategory: vi.fn(),
  mockGetAllLatestVideos: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockChannelsApi {
    getChannelsByCategory = mockGetChannelsByCategory;
  }
  class MockVideosApi {
    getAllLatestVideos = mockGetAllLatestVideos;
  }
  return {
    ...actual,
    ChannelsApi: MockChannelsApi,
    VideosApi: MockVideosApi,
  };
});

describe("youtubeListFilter", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("fetches channels for all categories", async () => {
    const channels = [youtubeChannel("ch-1")];
    mockGetChannelsByCategory.mockResolvedValue({ data: channels });

    await expect(fetchYoutubeChannelsByListFilter("all")).resolves.toEqual(channels);
    expect(mockGetChannelsByCategory).toHaveBeenCalledWith(false, undefined);
  });

  it("fetches favorite channels directly", async () => {
    const channels = [youtubeChannel("ch-1", { isFavorite: true })];
    mockGetChannelsByCategory.mockResolvedValue({ data: channels });

    await expect(fetchYoutubeChannelsByListFilter("favorites")).resolves.toEqual(
      channels,
    );
    expect(mockGetChannelsByCategory).toHaveBeenCalledWith(true);
  });

  it("filters favorite videos to starred channels only", async () => {
    mockGetChannelsByCategory.mockResolvedValue({
      data: [youtubeChannel("fav-1", { isFavorite: true })],
    });
    mockGetAllLatestVideos.mockResolvedValue({
      data: [
        youtubeVideo("v-1", { channelId: "fav-1" }),
        youtubeVideo("v-2", { channelId: "other" }),
      ],
    });

    const videos = await fetchYoutubeVideosByListFilter(5, "favorites");

    expect(videos).toHaveLength(1);
    expect(videos[0]?.videoId).toBe("v-1");
  });

  it("returns no favorite videos when there are no starred channels", async () => {
    mockGetChannelsByCategory.mockResolvedValue({ data: [] });

    await expect(fetchYoutubeVideosByListFilter(5, "favorites")).resolves.toEqual(
      [],
    );
    expect(mockGetAllLatestVideos).not.toHaveBeenCalled();
  });

  it("fetches videos for a specific category", async () => {
    const videos = [youtubeVideo("v-1")];
    mockGetAllLatestVideos.mockResolvedValue({ data: videos });

    await expect(fetchYoutubeVideosByListFilter(3, "cat-1")).resolves.toEqual(videos);
    expect(mockGetAllLatestVideos).toHaveBeenCalledWith(false, 3, "cat-1");
  });
});
