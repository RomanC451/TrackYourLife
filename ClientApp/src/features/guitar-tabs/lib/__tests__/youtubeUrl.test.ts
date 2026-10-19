import { describe, expect, it } from "vitest";

import { youtubeEmbedUrl, youtubePlayerSrc, youtubeVideoId } from "../youtubeUrl";

describe("youtubeUrl", () => {
  it("reads a video id from watch, short, and embed links", () => {
    expect(youtubeVideoId("https://www.youtube.com/watch?v=RBumgq5yVrA")).toBe(
      "RBumgq5yVrA",
    );
    expect(youtubeVideoId("https://youtu.be/RBumgq5yVrA?t=12")).toBe("RBumgq5yVrA");
    expect(youtubeVideoId("https://www.youtube.com/embed/RBumgq5yVrA")).toBe(
      "RBumgq5yVrA",
    );
    expect(youtubeVideoId("https://example.com/video")).toBeNull();
  });

  it("builds an embed url", () => {
    expect(youtubeEmbedUrl("https://youtu.be/RBumgq5yVrA")).toBe(
      "https://www.youtube.com/embed/RBumgq5yVrA",
    );
  });

  it("plays through a hostname when the page was opened by IP address", () => {
    expect(
      youtubePlayerSrc(
        "https://youtu.be/RBumgq5yVrA",
        "100.75.70.70",
        "http://desktop.example.ts.net:5173/",
      ),
    ).toBe("http://desktop.example.ts.net:5173/youtube-embed.html?v=RBumgq5yVrA");
    expect(
      youtubePlayerSrc(
        "https://youtu.be/RBumgq5yVrA",
        "localhost",
        "http://desktop.example.ts.net:5173",
      ),
    ).toBe("https://www.youtube.com/embed/RBumgq5yVrA");
    expect(youtubePlayerSrc("https://youtu.be/RBumgq5yVrA", "100.75.70.70", null)).toBe(
      "https://www.youtube.com/embed/RBumgq5yVrA",
    );
  });
});
