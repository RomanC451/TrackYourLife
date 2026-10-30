import { describe, expect, it } from "vitest";

import { detectPitch } from "../pitchDetection";

function sineWave(frequency: number, sampleRate: number, length: number) {
  const samples = new Float32Array(length);
  for (let i = 0; i < length; i++) {
    samples[i] = Math.sin((2 * Math.PI * frequency * i) / sampleRate);
  }
  return samples;
}

describe("detectPitch", () => {
  it("finds a stable sine near the low A and the low E", () => {
    expect(detectPitch(sineWave(110, 44100, 4096), 44100)).toBeCloseTo(110, 0);
    expect(detectPitch(sineWave(82.41, 44100, 4096), 44100)).toBeCloseTo(
      82.41,
      0,
    );
  });

  it("returns null for silence and quiet noise", () => {
    expect(detectPitch(new Float32Array(4096), 44100)).toBeNull();
    const quiet = sineWave(110, 44100, 4096).map((sample) => sample * 0.001);
    expect(detectPitch(new Float32Array(quiet), 44100)).toBeNull();
  });
});
