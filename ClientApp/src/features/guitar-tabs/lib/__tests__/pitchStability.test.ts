import { describe, expect, it } from "vitest";

import {
  stabilizePitch,
  stepCents,
  type PitchReading,
  type PitchStability,
} from "../pitchStability";

const idle: PitchStability = { shown: null, pending: null };

function read(cents: number, stringIndex = 1): PitchReading {
  return { stringIndex, cents };
}

function feed(state: PitchStability, readings: Array<PitchReading | null>) {
  return readings.reduce(
    (current, reading) => stabilizePitch(current, reading),
    state,
  );
}

describe("cent steps", () => {
  it("sits at zero inside the in-tune range and otherwise uses steps of five", () => {
    expect(stepCents(null, 4)).toBe(0);
    expect(stepCents(null, -5)).toBe(0);
    expect(stepCents(null, 7)).toBe(5);
    expect(stepCents(null, -8)).toBe(-10);
    expect(stepCents(null, -150)).toBe(-150);
  });

  it("holds a step until the pitch moves five cents away", () => {
    expect(stepCents(5, 9)).toBe(5);
    expect(stepCents(5, 10)).toBe(10);
    expect(stepCents(5, 4)).toBe(0);
    expect(stepCents(10, 6)).toBe(10);
    expect(stepCents(-10, -6)).toBe(-10);
  });
});

describe("pitch stability", () => {
  it("hides a single far reading and shows it after it holds", () => {
    const once = stabilizePitch(idle, read(-150));
    expect(once.shown).toBeNull();

    const held = feed(idle, [read(-150), read(-148), read(-151)]);
    expect(held.shown).toEqual(read(-151));
  });

  it("keeps the shown pitch when one frame jumps away", () => {
    const shown = feed(idle, [read(7), read(6), read(8)]);
    const spiked = stabilizePitch(shown, read(-150));
    expect(spiked.shown).toEqual(read(8));

    const returned = stabilizePitch(spiked, read(5));
    expect(returned.shown).toEqual(read(5));
    expect(returned.pending).toBeNull();
  });

  it("follows a small move on the same string immediately", () => {
    const shown = feed(idle, [read(0), read(0), read(0)]);
    const moved = stabilizePitch(shown, read(12));
    expect(moved.shown).toEqual(read(12));
    expect(moved.pending).toBeNull();
  });

  it("waits before switching strings", () => {
    const shown = feed(idle, [read(0, 0), read(0, 0), read(0, 0)]);
    const firstJump = stabilizePitch(shown, read(2, 1));
    expect(firstJump.shown).toEqual(read(0, 0));

    const switched = feed(firstJump, [read(2, 1), read(1, 1)]);
    expect(switched.shown).toEqual(read(1, 1));
  });

  it("clears the meter as soon as the pitch goes silent", () => {
    const shown = feed(idle, [read(4), read(4), read(4)]);
    const pending = stabilizePitch(shown, read(-150));
    expect(stabilizePitch(pending, null)).toEqual(idle);
  });
});
