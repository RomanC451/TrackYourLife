import { describe, expect, it } from "vitest";

import { capitalizeFirstLetter } from "../stringUtils";

describe("capitalizeFirstLetter", () => {
  it("capitalizes the first character", () => {
    expect(capitalizeFirstLetter("hello")).toBe("Hello");
  });

  it("leaves an already-capitalized string unchanged", () => {
    expect(capitalizeFirstLetter("Hello")).toBe("Hello");
  });

  it("coerces non-string values via String()", () => {
    expect(capitalizeFirstLetter(123 as unknown as string)).toBe("123");
  });

  it("returns empty string for empty input", () => {
    expect(capitalizeFirstLetter("")).toBe("");
  });
});
