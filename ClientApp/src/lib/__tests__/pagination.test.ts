import { describe, expect, it } from "vitest";

import { generatePageNumbers } from "../pagination";

describe("generatePageNumbers", () => {
  it("returns all pages when maxPage is 7 or fewer", () => {
    expect(generatePageNumbers(1, 5)).toEqual([1, 2, 3, 4, 5]);
    expect(generatePageNumbers(3, 7)).toEqual([1, 2, 3, 4, 5, 6, 7]);
  });

  it("shows ellipsis around the current page for large page counts", () => {
    expect(generatePageNumbers(5, 10)).toEqual([
      1,
      "ellipsis",
      4,
      5,
      6,
      "ellipsis",
      10,
    ]);
  });

  it("expands the start window when near the beginning", () => {
    expect(generatePageNumbers(2, 10)).toEqual([1, 2, 3, 4, 5, "ellipsis", 10]);
  });

  it("expands the end window when near the end", () => {
    expect(generatePageNumbers(9, 10)).toEqual([1, "ellipsis", 6, 7, 8, 9, 10]);
  });
});
