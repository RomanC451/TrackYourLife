import type { YoutubeVideoPreview } from "@/services/openapi";

export interface WatchedVideoHistoryEntry {
  video: YoutubeVideoPreview | null;
  videoId: string;
  watchedAtUtc: string;
}

export interface PagedWatchHistory {
  items: WatchedVideoHistoryEntry[];
  page: number;
  pageSize: number;
  maxPage: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
