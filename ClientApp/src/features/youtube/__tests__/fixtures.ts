import type {
  WatchedVideoHistoryEntry,
  YoutubeCategorySettingsDto,
  YoutubeChannelDto,
  YoutubeSettingsDto,
  YoutubeVideoPreview,
} from "@/services/openapi";

const baseDto = { isLoading: false, isDeleting: false };

const categoryIds = {
  "cat-1": "11111111-1111-4111-8111-111111111111",
  "cat-2": "22222222-2222-4222-8222-222222222222",
} as const;

export function youtubeCategory(
  id: keyof typeof categoryIds | string,
  overrides: Partial<YoutubeCategorySettingsDto> = {},
): YoutubeCategorySettingsDto {
  const resolvedId =
    id in categoryIds ? categoryIds[id as keyof typeof categoryIds] : id;

  return {
    ...baseDto,
    id: resolvedId,
    name: `Category ${id}`,
    displayOrder: 0,
    maxVideosPerDay: 5,
    subscribedChannelCount: 0,
    ...overrides,
  };
}

export function youtubeChannel(
  id: string,
  overrides: Partial<YoutubeChannelDto> = {},
): YoutubeChannelDto {
  return {
    ...baseDto,
    id: `channel-row-${id}`,
    youtubeChannelId: id,
    name: `Channel ${id}`,
    youtubeCategoryId: "cat-1",
    categoryName: "Fitness",
    isFavorite: false,
    createdOnUtc: "2026-01-01T00:00:00Z",
    ...overrides,
  };
}

export function youtubeVideo(
  videoId: string,
  overrides: Partial<YoutubeVideoPreview> = {},
): YoutubeVideoPreview {
  return {
    videoId,
    title: `Video ${videoId}`,
    thumbnailUrl: "https://example.com/thumb.jpg",
    channelName: "Fitness Channel",
    channelId: "channel-1",
    publishedAt: "2026-06-01T00:00:00Z",
    duration: "10:00",
    viewCount: 1500,
    isWatched: false,
    ...overrides,
  };
}

export function watchHistoryEntry(
  overrides: Partial<WatchedVideoHistoryEntry> = {},
): WatchedVideoHistoryEntry {
  return {
    videoId: "video-1",
    watchedAtUtc: "2026-06-05T12:00:00Z",
    video: youtubeVideo("video-1"),
    ...overrides,
  };
}

export function youtubeSettings(
  overrides: Partial<YoutubeSettingsDto> = {},
): YoutubeSettingsDto {
  return {
    ...baseDto,
    hasSettingsPassword: false,
    categories: [
      youtubeCategory("cat-1", { displayOrder: 1, name: "Fitness" }),
      youtubeCategory("cat-2", { displayOrder: 0, name: "Learning" }),
    ],
    ...overrides,
  };
}
