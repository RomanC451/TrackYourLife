import { describe, expect, it } from "vitest";

import { getClosestMultiple, getPercentages } from "../MathExtension";

describe("getClosestMultiple", () => {
  it("rounds to the nearest multiple", () => {
    expect(getClosestMultiple(7, 5)).toBe(5);
    expect(getClosestMultiple(8, 5)).toBe(10);
    expect(getClosestMultiple(10, 5)).toBe(10);
  });

  it("handles negative numbers", () => {
    expect(getClosestMultiple(-7, 5)).toBe(-5);
    expect(getClosestMultiple(-8, 5)).toBe(-10);
  });
});

describe("getPercentages", () => {
  it("returns rounded percentages that sum to ~100", () => {
    expect(getPercentages([25, 25, 50])).toEqual([25, 25, 50]);
    expect(getPercentages([1, 1, 1])).toEqual([33, 33, 33]);
  });

  it("returns zeros when total is zero", () => {
    expect(getPercentages([0, 0, 0])).toEqual([0, 0, 0]);
  });

  it("returns empty array for empty input", () => {
    expect(getPercentages([])).toEqual([]);
  });
});
