import { describe, expect, it } from "vitest";

import { postPlayerReset } from "../youtubePlayerBridge";

describe("postPlayerReset", () => {
  it("sends the embed page a reset for a bridged player", () => {
    const messages: unknown[] = [];
    const frame = document.createElement("iframe");
    frame.src = "http://desktop.example.ts.net:5173/youtube-embed.html?v=RBumgq5yVrA";
    Object.defineProperty(frame, "contentWindow", {
      configurable: true,
      value: {
        postMessage: (message: unknown) => {
          messages.push(message);
        },
      },
    });

    postPlayerReset(frame);

    expect(messages).toEqual([{ type: "guitar-tab-player-reset" }]);
  });

  it("seeks a direct player to the start and pauses it", () => {
    const messages: unknown[] = [];
    const frame = document.createElement("iframe");
    frame.src = "https://www.youtube.com/embed/RBumgq5yVrA";
    Object.defineProperty(frame, "contentWindow", {
      configurable: true,
      value: {
        postMessage: (message: unknown) => {
          messages.push(message);
        },
      },
    });

    postPlayerReset(frame);

    expect(messages).toEqual([
      JSON.stringify({ event: "command", func: "seekTo", args: [0, true] }),
      JSON.stringify({ event: "command", func: "pauseVideo", args: [] }),
    ]);
  });
});
