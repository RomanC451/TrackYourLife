import { describe, expect, it } from "vitest";

import {
  clampNeedle,
  formatCents,
  frequencyFromMidi,
  isInTune,
  matchPitch,
  tuningPresetById,
} from "../tuningPresets";

const standard = tuningPresetById("standard");
const dropD = tuningPresetById("drop-d");

describe("tuning presets", () => {
  it("builds standard open strings from A4 = 440", () => {
    expect(frequencyFromMidi(40)).toBeCloseTo(82.41, 1);
    expect(frequencyFromMidi(45)).toBeCloseTo(110, 2);
    expect(frequencyFromMidi(64)).toBeCloseTo(329.63, 1);
    expect(standard.strings.map((tunerString) => tunerString.name)).toEqual([
      "E",
      "A",
      "D",
      "G",
      "B",
      "E",
    ]);
    expect(
      tuningPresetById("half-step-down").strings.map(
        (tunerString) => tunerString.name,
      ),
    ).toEqual(["Eb", "Ab", "Db", "Gb", "Bb", "Eb"]);
  });

  it("matches the nearest string and folds one octave of error", () => {
    const lowE = matchPitch(frequencyFromMidi(40) * 2, standard.strings);
    expect(lowE?.stringIndex).toBe(0);
    expect(lowE?.cents).toBeCloseTo(0, 5);
    expect(isInTune(lowE?.cents ?? 99)).toBe(true);

    const highE = matchPitch(frequencyFromMidi(64), standard.strings);
    expect(highE?.stringIndex).toBe(5);
    expect(highE?.cents).toBeCloseTo(0, 5);
  });

  it("keeps a true higher D on the higher D string in Drop D", () => {
    const higherD = matchPitch(frequencyFromMidi(50), dropD.strings);
    expect(higherD?.stringIndex).toBe(2);
    expect(higherD?.cents).toBeCloseTo(0, 5);

    const lowD = matchPitch(frequencyFromMidi(38), dropD.strings);
    expect(lowD?.stringIndex).toBe(0);
  });

  it("judges a locked string with the same octave fold", () => {
    const lockedLowE = matchPitch(frequencyFromMidi(40) * 2, [
      standard.strings[0],
    ]);
    expect(lockedLowE?.stringIndex).toBe(0);
    expect(lockedLowE?.cents).toBeCloseTo(0, 5);
  });

  it("treats 7 cents sharp as still sharp", () => {
    const heard = frequencyFromMidi(45) * 2 ** (7 / 1200);
    const match = matchPitch(heard, standard.strings);
    expect(match?.stringIndex).toBe(1);
    expect(match?.cents).toBeCloseTo(7, 1);
    expect(isInTune(match?.cents ?? 0)).toBe(false);
    expect(formatCents(match?.cents ?? 0)).toBe("+7");
  });

  it("pegs the needle outside 50 cents and ignores silence", () => {
    expect(clampNeedle(80)).toBe(50);
    expect(clampNeedle(-80)).toBe(-50);
    expect(formatCents(-4)).toBe("-4");
    expect(formatCents(0)).toBe("0");
    expect(matchPitch(0, standard.strings)).toBeNull();
    expect(matchPitch(Number.NaN, standard.strings)).toBeNull();
  });
});
