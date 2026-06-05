import { describe, expect, it, vi } from "vitest";

import { dedupePlayVideoRequest } from "../playVideoInFlight";

describe("dedupePlayVideoRequest", () => {
  it("returns the same promise for concurrent requests with the same video id", async () => {
    const request = vi.fn(
      () => new Promise<string>((resolve) => resolve("ok")),
    );

    const first = dedupePlayVideoRequest("video-1", request);
    const second = dedupePlayVideoRequest("video-1", request);

    expect(first).toBe(second);
    await expect(first).resolves.toBe("ok");
    expect(request).toHaveBeenCalledTimes(1);
  });

  it("starts a new request after the previous one settles", async () => {
    let resolveFirst: (value: string) => void = () => undefined;
    const request = vi
      .fn()
      .mockImplementationOnce(
        () =>
          new Promise<string>((resolve) => {
            resolveFirst = resolve;
          }),
      )
      .mockResolvedValueOnce("second");

    const first = dedupePlayVideoRequest("video-1", request);
    resolveFirst("first");
    await first;

    const second = await dedupePlayVideoRequest("video-1", request);
    expect(second).toBe("second");
    expect(request).toHaveBeenCalledTimes(2);
  });

  it("does not dedupe requests for different video ids", async () => {
    const request = vi
      .fn()
      .mockResolvedValueOnce("a")
      .mockResolvedValueOnce("b");

    const first = dedupePlayVideoRequest("video-a", request);
    const second = dedupePlayVideoRequest("video-b", request);

    expect(first).not.toBe(second);
    await expect(Promise.all([first, second])).resolves.toEqual(["a", "b"]);
    expect(request).toHaveBeenCalledTimes(2);
  });
});
