import { describe, expect, it } from "vitest";

import type { GuitarTabChord } from "../../api/guitarTabsApi";
import {
  changeTranspose,
  displayChordName,
  transposeGuitarChord,
} from "../transposeChord";

const am: GuitarTabChord = {
  name: "Am",
  frets: [0, 1, 2, 2, 0, -1],
  position: 0,
  barreFret: null,
  barreFromString: null,
  barreToString: null,
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
      frets: [2, 3, 4, 4, 2, -1],
      position: 2,
    });
  });
});
