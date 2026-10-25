import { describe, expect, it } from "vitest";

import type { GuitarTabChord } from "../../api/guitarTabsApi";
import {
  changeTranspose,
  displayChordName,
  transposeGuitarChord,
} from "../transposeChord";

const am: GuitarTabChord = {
  name: "Am",
  shapes: [
    {
      frets: [0, 1, 2, 2, 0, -1],
      fingers: [0, 1, 3, 2, 0, 0],
      position: 0,
      barreFret: null,
      barreFromString: null,
      barreToString: null,
      barreFinger: null,
    },
  ],
};

describe("transpose chords", () => {
  it("shifts chord names and wraps at an octave", () => {
    expect(displayChordName("Am", 2, false)).toBe("Bm");
    expect(displayChordName("F", 1, false)).toBe("F#");
    expect(displayChordName("F", 1, true)).toBe("Gb");
    expect(displayChordName("G/B", 2, false)).toBe("A/C#");
    expect(displayChordName("Bdim7", 1, false)).toBe("Cdim7");
    expect(displayChordName("Bb", 0, false)).toBe("Bb");
    expect(displayChordName("F#", 0, true)).toBe("Gb");
    expect(changeTranspose(11, 1)).toBe(0);
    expect(changeTranspose(-11, -1)).toBe(0);
    expect(changeTranspose(0, -1)).toBe(-1);
  });

  it("moves the chord shape with the name", () => {
    expect(transposeGuitarChord(am, 2, false)).toMatchObject({
      name: "Bm",
      shapes: [{ frets: [2, 3, 4, 4, 2, -1], position: 2 }],
    });
  });

  it("uses a low fingering of the new chord instead of sliding an open shape up the neck", () => {
    const chord: GuitarTabChord = {
      name: "Am",
      shapes: [
        am.shapes[0],
        {
          frets: [5, 5, 5, 7, 7, 5],
          fingers: [1, 1, 1, 3, 4, 1],
          position: 5,
          barreFret: 5,
          barreFromString: 0,
          barreToString: 5,
          barreFinger: 1,
        },
      ],
    };

    expect(transposeGuitarChord(chord, -1, false)).toMatchObject({
      name: "G#m",
      shapes: [
        {
          frets: [4, 4, 4, 6, 6, 4],
          position: 4,
          barreFret: 4,
        },
        { frets: [11, 12, 13, 13, 11, -1], position: 11 },
      ],
    });
  });

  it("drops a barre that lands on the nut", () => {
    const f: GuitarTabChord = {
      name: "F",
      shapes: [
        {
          frets: [1, 1, 2, 3, 3, 1],
          fingers: [1, 1, 2, 4, 3, 1],
          position: 1,
          barreFret: 1,
          barreFromString: 0,
          barreToString: 5,
          barreFinger: 1,
        },
      ],
    };

    expect(transposeGuitarChord(f, -1, false)).toMatchObject({
      name: "E",
      shapes: [
        {
          frets: [0, 0, 1, 2, 2, 0],
          fingers: [0, 0, 2, 4, 3, 0],
          position: 0,
          barreFret: null,
          barreFinger: null,
        },
      ],
    });
  });
});
