import { useSyncExternalStore } from "react";

import {
  getGuitarTabPlaybackSettings,
  nextPlayback,
} from "./playbackSettings";
import { requestVideoPlayback, requestVideoReset } from "./youtubePlayerBridge";

let autoscrollPlaying = false;
let videoPlaying = false;
let ignoreRemotePlay = false;
const listeners = new Set<() => void>();

function emit() {
  listeners.forEach((listener) => listener());
}

export function subscribePlaybackSession(listener: () => void) {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export function getAutoscrollPlaying() {
  return autoscrollPlaying;
}

export function useAutoscrollPlaying() {
  return useSyncExternalStore(
    subscribePlaybackSession,
    getAutoscrollPlaying,
    getAutoscrollPlaying,
  );
}

export function toggleFromSpace() {
  ignoreRemotePlay = false;
  const settings = getGuitarTabPlaybackSettings();
  const next = nextPlayback(settings, {
    autoscroll: autoscrollPlaying,
    video: videoPlaying,
  });
  const videoChanged = next.video !== videoPlaying;
  autoscrollPlaying = next.autoscroll;
  videoPlaying = next.video;
  emit();
  if (videoChanged) {
    requestVideoPlayback(next.video);
  }
  return settings.autoscroll || settings.syncVideo;
}

export function toggleAutoscrollControl() {
  ignoreRemotePlay = false;
  const settings = getGuitarTabPlaybackSettings();
  const next = !autoscrollPlaying;
  autoscrollPlaying = next;
  if (settings.autoscroll && settings.syncVideo) {
    videoPlaying = next;
    requestVideoPlayback(next);
  }
  emit();
}

export function stopAutoscroll() {
  if (!autoscrollPlaying) {
    return;
  }

  autoscrollPlaying = false;
  emit();
}

export function applyRemoteVideoState(playing: boolean) {
  if (ignoreRemotePlay && playing) {
    return;
  }

  if (!playing) {
    ignoreRemotePlay = false;
  }

  const settings = getGuitarTabPlaybackSettings();
  const started = playing && !videoPlaying;
  const stopped = !playing && videoPlaying;
  const nextAutoscroll =
    settings.autoscroll && settings.syncVideo && started
      ? true
      : settings.autoscroll && settings.syncVideo && stopped
        ? false
        : autoscrollPlaying;
  if (videoPlaying === playing && autoscrollPlaying === nextAutoscroll) {
    return;
  }

  videoPlaying = playing;
  autoscrollPlaying = nextAutoscroll;
  emit();
}

export function resetAutoscrollAndVideo() {
  autoscrollPlaying = false;
  videoPlaying = false;
  ignoreRemotePlay = true;
  emit();
  const scroller = document.scrollingElement;
  if (scroller) {
    scroller.scrollTop = 0;
  }
  requestVideoReset();
}

export function resetPlaybackSession() {
  autoscrollPlaying = false;
  videoPlaying = false;
  ignoreRemotePlay = false;
  emit();
}
