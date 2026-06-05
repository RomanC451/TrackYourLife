import { describe, expect, it } from "vitest";

import {
  getYoutubeWatchRouteForPath,
  isAlreadyOnWatchPath,
  isYoutubeWatchPath,
  youtubeSearchWatchRoute,
  youtubeVideosWatchRoute,
} from "../youtubePlaybackRoutes";

describe("getYoutubeWatchRouteForPath", () => {
  it("returns the videos watch route under /youtube/videos", () => {
    expect(getYoutubeWatchRouteForPath("/youtube/videos")).toBe(
      youtubeVideosWatchRoute,
    );
    expect(
      getYoutubeWatchRouteForPath("/youtube/videos/watch/abc123"),
    ).toBe(youtubeVideosWatchRoute);
  });

  it("returns the search watch route under /youtube/search", () => {
    expect(getYoutubeWatchRouteForPath("/youtube/search")).toBe(
      youtubeSearchWatchRoute,
    );
    expect(
      getYoutubeWatchRouteForPath("/youtube/search/watch/abc123"),
    ).toBe(youtubeSearchWatchRoute);
  });

  it("returns null for pages that do not sync a watch URL", () => {
    expect(getYoutubeWatchRouteForPath("/youtube/library")).toBeNull();
    expect(getYoutubeWatchRouteForPath("/home")).toBeNull();
    expect(getYoutubeWatchRouteForPath("/youtube/history")).toBeNull();
  });
});

describe("isYoutubeWatchPath", () => {
  it("detects watch dialog paths", () => {
    expect(isYoutubeWatchPath("/youtube/videos/watch/abc")).toBe(true);
    expect(isYoutubeWatchPath("/youtube/videos")).toBe(false);
  });
});

describe("isAlreadyOnWatchPath", () => {
  it("matches the current video id in the path", () => {
    expect(isAlreadyOnWatchPath("/youtube/videos/watch/abc", "abc")).toBe(true);
    expect(isAlreadyOnWatchPath("/youtube/videos/watch/abc", "xyz")).toBe(
      false,
    );
  });
});
