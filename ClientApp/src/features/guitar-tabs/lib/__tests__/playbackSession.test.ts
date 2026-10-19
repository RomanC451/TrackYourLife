import { beforeEach, describe, expect, it } from "vitest";

import { resetGuitarTabPlaybackSettings } from "../playbackSettings";
import {
  applyRemoteVideoState,
  getAutoscrollPlaying,
  resetAutoscrollAndVideo,
  resetPlaybackSession,
  stopAutoscroll,
  toggleFromSpace,
} from "../playbackSession";
import { playerCommandType, playerResetType } from "../youtubePlayerBridge";

describe("stopAutoscroll", () => {
  beforeEach(() => {
    resetGuitarTabPlaybackSettings();
    resetPlaybackSession();
  });

  it("stops scrolling at the bottom without pausing the video", () => {
    const commands: boolean[] = [];
    const onCommand = (event: Event) => {
      commands.push((event as CustomEvent<{ playing: boolean }>).detail.playing);
    };
    window.addEventListener(playerCommandType, onCommand);

    toggleFromSpace();
    stopAutoscroll();
    applyRemoteVideoState(true);

    expect(getAutoscrollPlaying()).toBe(false);
    expect(commands).toEqual([true]);
    window.removeEventListener(playerCommandType, onCommand);
  });

  it("returns scrolling and the video to the start", () => {
    let resets = 0;
    const onReset = () => {
      resets += 1;
    };
    window.addEventListener(playerResetType, onReset);

    toggleFromSpace();
    resetAutoscrollAndVideo();
    applyRemoteVideoState(true);

    expect(getAutoscrollPlaying()).toBe(false);
    expect(resets).toBe(1);

    applyRemoteVideoState(false);
    toggleFromSpace();
    expect(getAutoscrollPlaying()).toBe(true);
    window.removeEventListener(playerResetType, onReset);
  });
});
