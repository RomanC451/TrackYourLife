import { describe, expect, it } from "vitest";

import {
  chordSpans,
  layoutTab,
  spanText,
  trimTabLine,
  wrapTabLine,
} from "../tabLines";

describe("layoutTab", () => {
  it("keeps a chord line with the lyric line under it", () => {
    const blocks = layoutTab("[tab]    [ch]Am[/ch]\nhello there[/tab]\n[Verse]");

    expect(blocks).toEqual([
      { kind: "pair", chordLine: "    [ch]Am[/ch]", lyricLine: "hello there" },
      { kind: "section", title: "Verse" },
    ]);
  });

  it("leaves a chord run without lyrics on its own", () => {
    const blocks = layoutTab("[ch]Am[/ch]  [ch]C[/ch]\n\n[Intro]");

    expect(blocks.map((block) => block.kind)).toEqual([
      "chords",
      "blank",
      "section",
    ]);
  });
});

describe("wrapTabLine", () => {
  it("wraps both rows at the same word so the chord stays above it", () => {
    const spans = chordSpans("[ch]Am[/ch]    [ch]G[/ch]", 0, false);
    const rows = wrapTabLine(spans, "hello world", 8).map(trimTabLine);

    expect(rows.map((row) => spanText(row.chord))).toEqual(["Am", "G"]);
    expect(rows.map((row) => row.lyric)).toEqual(["hello", "world"]);
  });

  it("keeps a chord above the syllable it was written on", () => {
    const chordLine = `${" ".repeat(17)}[ch]Am[/ch]    [ch]C[/ch]`;
    const spans = chordSpans(chordLine, 0, false);
    const rows = wrapTabLine(spans, "Last call at the bar", 20).map(trimTabLine);

    expect(rows).toEqual([
      { chord: [], lyric: "Last call at the" },
      {
        chord: [
          { text: "Am", chord: true },
          { text: "    ", chord: false },
          { text: "C", chord: true },
        ],
        lyric: "bar",
      },
    ]);
  });

  it("moves the wrap to the start of a chord instead of cutting the name", () => {
    const spans = chordSpans(`${" ".repeat(4)}[ch]Gsus4[/ch]`, 0, false);
    const rows = wrapTabLine(spans, "hello world", 8).map(trimTabLine);

    expect(rows.map((row) => row.lyric)).toEqual(["hell", "o world"]);
    expect(rows.map((row) => spanText(row.chord))).toEqual(["", "Gsus4"]);
  });

  it("wraps a chord-only line between chords", () => {
    const spans = chordSpans("[ch]Am[/ch]  [ch]C[/ch]  [ch]G[/ch]", 0, false);
    const rows = wrapTabLine(spans, spanText(spans), 6).map(trimTabLine);

    expect(rows.map((row) => spanText(row.chord))).toEqual(["Am  C", "G"]);
  });
});
