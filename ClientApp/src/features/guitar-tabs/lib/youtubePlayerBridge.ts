export const playerCommandType = "guitar-tabs-player-command";
export const playerResetType = "guitar-tabs-player-reset";

export function requestVideoPlayback(playing: boolean) {
  window.dispatchEvent(
    new CustomEvent(playerCommandType, { detail: { playing } }),
  );
}

export function requestVideoReset() {
  window.dispatchEvent(new CustomEvent(playerResetType));
}

export function playerFrameUrl(embedUrl: string, pageOrigin: string) {
  const url = new URL(embedUrl, pageOrigin);
  if (url.pathname.endsWith("/youtube-embed.html")) {
    return url.toString();
  }

  url.searchParams.set("enablejsapi", "1");
  url.searchParams.set("origin", pageOrigin);
  return url.toString();
}

export function postPlayerCommand(
  frame: HTMLIFrameElement,
  playing: boolean,
) {
  const target = frame.contentWindow;
  if (!target) {
    return;
  }

  if (frame.src.includes("youtube-embed.html")) {
    target.postMessage({ type: "guitar-tab-player", playing }, "*");
    return;
  }

  target.postMessage(
    JSON.stringify({
      event: "command",
      func: playing ? "playVideo" : "pauseVideo",
      args: [],
    }),
    "*",
  );
}

export function postPlayerReset(frame: HTMLIFrameElement) {
  const target = frame.contentWindow;
  if (!target) {
    return;
  }

  if (frame.src.includes("youtube-embed.html")) {
    target.postMessage({ type: "guitar-tab-player-reset" }, "*");
    return;
  }

  target.postMessage(
    JSON.stringify({ event: "command", func: "seekTo", args: [0, true] }),
    "*",
  );
  target.postMessage(
    JSON.stringify({ event: "command", func: "pauseVideo", args: [] }),
    "*",
  );
}

export function listenToPlayer(frame: HTMLIFrameElement) {
  frame.contentWindow?.postMessage(
    JSON.stringify({ event: "listening", id: "guitar-tab" }),
    "*",
  );
}

function parsedMessage(data: unknown) {
  if (typeof data === "string") {
    try {
      return JSON.parse(data) as unknown;
    } catch {
      return null;
    }
  }

  return data;
}

export function readRemotePlaying(data: unknown) {
  const message = parsedMessage(data);
  if (!message || typeof message !== "object") {
    return null;
  }

  const record = message as {
    type?: unknown;
    playing?: unknown;
    event?: unknown;
    info?: unknown;
  };

  if (record.type === "guitar-tab-player-state" && typeof record.playing === "boolean") {
    return record.playing;
  }

  const state =
    record.event === "onStateChange" && typeof record.info === "number"
      ? record.info
      : record.event === "infoDelivery" &&
          record.info &&
          typeof record.info === "object" &&
          typeof (record.info as { playerState?: unknown }).playerState === "number"
        ? (record.info as { playerState: number }).playerState
        : null;

  if (state === 1) {
    return true;
  }

  if (state === 0 || state === 2) {
    return false;
  }

  return null;
}
