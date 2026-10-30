import { describe, expect, it } from "vitest";

import { REFERENCE_PLUCK_SECONDS, synthesizePluck } from "../referencePluck";

function rms(samples: Float32Array, start: number, end: number) {
  let energy = 0;
  for (let i = start; i < end; i++) {
    energy += samples[i] * samples[i];
  }
  return Math.sqrt(energy / (end - start));
}

describe("synthesizePluck", () => {
  it("lasts about one and a half seconds and fades out", () => {
    const sampleRate = 44100;
    const samples = synthesizePluck(110, sampleRate);
    expect(samples.length).toBe(Math.floor(sampleRate * REFERENCE_PLUCK_SECONDS));
    const head = rms(samples, 0, 2000);
    const tail = rms(samples, samples.length - 2000, samples.length);
    expect(head).toBeGreaterThan(0);
    expect(tail).toBeLessThan(head * 0.5);
  });
});
