import { describe, expect, it } from "vitest";

import { nextPlayback } from "../playbackSettings";

describe("nextPlayback", () => {
  it("starts autoscroll and the video together", () => {
    expect(
      nextPlayback(
        { autoscroll: true, syncVideo: true },
        { autoscroll: false, video: false },
      ),
    ).toEqual({ autoscroll: true, video: true });
  });

  it("stops both when either one is already running", () => {
    expect(
      nextPlayback(
        { autoscroll: true, syncVideo: true },
        { autoscroll: true, video: false },
      ),
    ).toEqual({ autoscroll: false, video: false });
  });

  it("changes only the controls that are enabled", () => {
    expect(
      nextPlayback(
        { autoscroll: true, syncVideo: false },
        { autoscroll: false, video: true },
      ),
    ).toEqual({ autoscroll: true, video: true });
    expect(
      nextPlayback(
        { autoscroll: false, syncVideo: true },
        { autoscroll: true, video: false },
      ),
    ).toEqual({ autoscroll: true, video: true });
  });
});
