import { useSyncExternalStore } from "react";

export type GuitarTabPlaybackSettings = {
  autoscroll: boolean;
  syncVideo: boolean;
};

const storageKey = "guitar-tabs.playback-settings";

const defaultSettings: GuitarTabPlaybackSettings = {
  autoscroll: true,
  syncVideo: true,
};

let current = readSettings();
const listeners = new Set<() => void>();

function readSettings(): GuitarTabPlaybackSettings {
  try {
    const raw = localStorage.getItem(storageKey);
    if (!raw) {
      return { ...defaultSettings };
    }

    const parsed = JSON.parse(raw) as Partial<GuitarTabPlaybackSettings>;
    return {
      autoscroll: parsed.autoscroll !== false,
      syncVideo: parsed.syncVideo !== false,
    };
  } catch {
    return { ...defaultSettings };
  }
}

function emit() {
  listeners.forEach((listener) => listener());
}

export function getGuitarTabPlaybackSettings() {
  return current;
}

export function setGuitarTabPlaybackSettings(
  patch: Partial<GuitarTabPlaybackSettings>,
) {
  current = { ...current, ...patch };
  localStorage.setItem(storageKey, JSON.stringify(current));
  emit();
}

export function subscribeGuitarTabPlaybackSettings(listener: () => void) {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export function useGuitarTabPlaybackSettings() {
  return useSyncExternalStore(
    subscribeGuitarTabPlaybackSettings,
    getGuitarTabPlaybackSettings,
    getGuitarTabPlaybackSettings,
  );
}

export function resetGuitarTabPlaybackSettings() {
  localStorage.removeItem(storageKey);
  current = { ...defaultSettings };
  emit();
}

export function nextPlayback(
  settings: GuitarTabPlaybackSettings,
  state: { autoscroll: boolean; video: boolean },
) {
  if (!settings.autoscroll && !settings.syncVideo) {
    return state;
  }

  const active =
    (settings.autoscroll && state.autoscroll) ||
    (settings.syncVideo && state.video);
  const next = !active;

  return {
    autoscroll: settings.autoscroll ? next : state.autoscroll,
    video: settings.syncVideo ? next : state.video,
  };
}
