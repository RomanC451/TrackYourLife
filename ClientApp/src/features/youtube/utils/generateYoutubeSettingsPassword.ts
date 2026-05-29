const LOWER = "abcdefghijklmnopqrstuvwxyz";
const UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
const DIGITS = "0123456789";
const SPECIAL = "@#$%^&+=!.";
const MIN_LENGTH = 10;
const DEFAULT_LENGTH = 16;

function randomChar(charset: string): string {
  const index = crypto.getRandomValues(new Uint32Array(1))[0]! % charset.length;
  return charset[index]!;
}

function shuffle<T>(items: T[]): T[] {
  const copy = [...items];
  for (let i = copy.length - 1; i > 0; i--) {
    const j = crypto.getRandomValues(new Uint32Array(1))[0]! % (i + 1);
    [copy[i], copy[j]] = [copy[j]!, copy[i]!];
  }
  return copy;
}

export function generateYoutubeSettingsPassword(
  length: number = DEFAULT_LENGTH,
): string {
  const size = Math.max(length, MIN_LENGTH);
  const required = [
    randomChar(LOWER),
    randomChar(UPPER),
    randomChar(DIGITS),
    randomChar(SPECIAL),
  ];
  const all = LOWER + UPPER + DIGITS + SPECIAL;
  while (required.length < size) {
    required.push(randomChar(all));
  }
  return shuffle(required).join("");
}

export const YOUTUBE_SETTINGS_PASSWORD_HINT =
  "At least 10 characters with upper, lower, number, and special (@#$%^&+=!.)";
