let suppressCardClickUntil = 0;

/** Ignore the next VideoCard play click (Radix dialog/dropdown dismiss click-through). */
export function suppressYoutubeCardClick(ms = 500) {
  suppressCardClickUntil = Date.now() + ms;
}

export function isYoutubeCardClickSuppressed() {
  return Date.now() < suppressCardClickUntil;
}
