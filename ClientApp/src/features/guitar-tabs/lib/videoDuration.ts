import { getYoutubeVideoDuration } from "../api/guitarTabsApi";

let durationSeconds: number | null = null;
let requestId = 0;
const listeners = new Set<() => void>();

export function getKnownVideoDuration() {
  return durationSeconds;
}

export function setKnownVideoDuration(seconds: number | null) {
  const next = seconds != null && seconds > 0 ? seconds : null;
  if (next === durationSeconds) {
    return;
  }

  durationSeconds = next;
  listeners.forEach((listener) => listener());
}

export function subscribeVideoDuration(listener: () => void) {
  listeners.add(listener);
  return () => {
    listeners.delete(listener);
  };
}

export function rememberVideoDuration(videoId: string | null) {
  const current = ++requestId;
  setKnownVideoDuration(null);
  if (!videoId) {
    return;
  }

  void getYoutubeVideoDuration(videoId)
    .then((seconds) => {
      if (current === requestId) {
        setKnownVideoDuration(seconds);
      }
    })
    .catch(() => {});
}
