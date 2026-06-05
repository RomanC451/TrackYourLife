export const youtubeVideosWatchRoute =
  "/youtube/videos/watch/$videoId" as const;

export const youtubeSearchWatchRoute =
  "/youtube/search/watch/$videoId" as const;

export type YoutubeWatchRoute =
  | typeof youtubeVideosWatchRoute
  | typeof youtubeSearchWatchRoute;

const watchPathSuffix = "/watch/";

export function isYoutubeWatchPath(pathname: string): boolean {
  return pathname.includes(watchPathSuffix);
}

/** Resolves which watch URL to sync when opening a video from the current page. */
export function getYoutubeWatchRouteForPath(
  pathname: string,
): YoutubeWatchRoute | null {
  if (pathname.startsWith("/youtube/search")) {
    return youtubeSearchWatchRoute;
  }

  if (pathname.startsWith("/youtube/videos")) {
    return youtubeVideosWatchRoute;
  }

  return null;
}

export function isAlreadyOnWatchPath(
  pathname: string,
  videoId: string,
): boolean {
  return pathname.endsWith(`${watchPathSuffix}${videoId}`);
}
