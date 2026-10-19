import { displayChordName } from "./transposeChord";

export type TabSpan = {
  text: string;
  chord: boolean;
};

export type TabLine = {
  chord: TabSpan[];
  lyric: string;
};

export type TabBlock =
  | { kind: "blank" }
  | { kind: "section"; title: string }
  | { kind: "staff"; text: string }
  | { kind: "text"; text: string }
  | { kind: "chords"; line: string }
  | { kind: "pair"; chordLine: string; lyricLine: string };

const chordPattern = /\[ch\](.*?)\[\/ch\]/g;

function stripTabTags(line: string) {
  return line.replace(/\[\/?tab\]/g, "");
}

function isSectionLine(line: string) {
  return /^\[[^[\]]+\]$/.test(line.trim());
}

function hasChords(line: string) {
  return /\[ch\].*?\[\/ch\]/.test(line);
}

function isStaffLine(line: string) {
  const text = line.trim();
  if (!text.includes("|") || !text.includes("-")) {
    return false;
  }

  return /^[A-Ga-g]?\|[-0-9hpbrx/\\~|()[\]{}*.\s]+$/.test(text);
}

function isLyricLine(line: string | undefined) {
  if (line == null) {
    return false;
  }

  const text = stripTabTags(line);
  return (
    text.trim().length > 0 &&
    !hasChords(text) &&
    !isSectionLine(text) &&
    !isStaffLine(text)
  );
}

export function layoutTab(content: string): TabBlock[] {
  const lines = content.replace(/\r\n/g, "\n").split("\n");
  const blocks: TabBlock[] = [];

  for (let index = 0; index < lines.length; index++) {
    const text = stripTabTags(lines[index]);
    if (text.trim().length === 0) {
      blocks.push({ kind: "blank" });
      continue;
    }

    if (isSectionLine(text)) {
      blocks.push({ kind: "section", title: text.trim().slice(1, -1) });
      continue;
    }

    if (isStaffLine(text)) {
      blocks.push({ kind: "staff", text });
      continue;
    }

    if (hasChords(text)) {
      const next = lines[index + 1];
      if (isLyricLine(next)) {
        blocks.push({
          kind: "pair",
          chordLine: text,
          lyricLine: stripTabTags(next ?? ""),
        });
        index += 1;
        continue;
      }

      blocks.push({ kind: "chords", line: text });
      continue;
    }

    blocks.push({ kind: "text", text });
  }

  return blocks;
}

export function chordSpans(
  line: string,
  semitones: number,
  useFlats: boolean,
): TabSpan[] {
  const spans: TabSpan[] = [];
  const pattern = new RegExp(chordPattern);
  let cursor = 0;

  for (const match of line.matchAll(pattern)) {
    const start = match.index ?? 0;
    if (start > cursor) {
      spans.push({ text: line.slice(cursor, start), chord: false });
    }

    spans.push({
      text: displayChordName(match[1], semitones, useFlats),
      chord: true,
    });
    cursor = start + match[0].length;
  }

  if (cursor < line.length) {
    spans.push({ text: line.slice(cursor), chord: false });
  }

  return spans.filter((span) => span.text.length > 0);
}

export function spanText(spans: TabSpan[]) {
  return spans.map((span) => span.text).join("");
}

export function sliceSpans(spans: TabSpan[], start: number, end: number) {
  const sliced: TabSpan[] = [];
  let index = 0;

  for (const span of spans) {
    const spanEnd = index + span.text.length;
    const from = Math.max(0, start - index);
    const to = Math.min(span.text.length, end - index);
    if (to > from) {
      sliced.push({ text: span.text.slice(from, to), chord: span.chord });
    }

    index = spanEnd;
    if (index >= end) {
      break;
    }
  }

  return sliced;
}

function chordRanges(spans: TabSpan[]) {
  const ranges: { start: number; end: number }[] = [];
  let index = 0;
  for (const span of spans) {
    if (span.chord) {
      ranges.push({ start: index, end: index + span.text.length });
    }

    index += span.text.length;
  }

  return ranges;
}

function chooseBreak(
  text: string,
  ranges: { start: number; end: number }[],
  start: number,
  limit: number,
) {
  let best = -1;
  let index = start;
  while (index < text.length) {
    while (index < text.length && text[index] !== " ") {
      index += 1;
    }

    if (index >= text.length) {
      break;
    }

    const runStart = index;
    while (index < text.length && text[index] === " ") {
      index += 1;
    }

    const end = index;
    const splitsChord = ranges.some((range) => range.start < end && range.end > end);
    if (runStart >= limit) {
      break;
    }

    if (!splitsChord && runStart > start) {
      best = end;
    }

    if (end >= limit) {
      break;
    }
  }

  if (best > start) {
    return Math.min(best, text.length);
  }

  const straddling = ranges.find((range) => range.start < limit && range.end > limit);
  if (straddling && straddling.start > start) {
    return straddling.start;
  }

  if (straddling && straddling.end > start) {
    return Math.min(straddling.end, text.length);
  }

  return Math.min(limit, text.length);
}

export function wrapTabLine(
  spans: TabSpan[],
  lyric: string,
  columns: number,
): TabLine[] {
  const length = Math.max(spanText(spans).length, lyric.length);
  if (length === 0) {
    return [];
  }

  if (columns < 1 || length <= columns) {
    return [{ chord: spans, lyric }];
  }

  const text = lyric.padEnd(length, " ");
  const ranges = chordRanges(spans);
  const rows: TabLine[] = [];
  let start = 0;

  while (start < length) {
    const limit = Math.min(start + columns, length);
    const end = limit === length ? length : chooseBreak(text, ranges, start, limit);
    const next = end > start ? end : Math.min(start + 1, length);
    rows.push({
      chord: sliceSpans(spans, start, next),
      lyric: text.slice(start, next),
    });
    start = next;
  }

  return rows;
}

export function trimTabLine(line: TabLine): TabLine {
  const chord = spanText(line.chord).trimEnd();
  return {
    chord: sliceSpans(line.chord, 0, chord.length),
    lyric: line.lyric.trimEnd(),
  };
}
