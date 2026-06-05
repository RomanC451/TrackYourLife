/** Coalesce concurrent playVideo API calls for the same video (e.g. React Strict Mode). */
const inFlightByVideoId = new Map<string, Promise<unknown>>();

export function dedupePlayVideoRequest<T>(
  videoId: string,
  request: () => Promise<T>,
): Promise<T> {
  const existing = inFlightByVideoId.get(videoId);
  if (existing !== undefined) {
    return existing as Promise<T>;
  }

  const promise = request().finally(() => {
    inFlightByVideoId.delete(videoId);
  });
  inFlightByVideoId.set(videoId, promise);
  return promise;
}
