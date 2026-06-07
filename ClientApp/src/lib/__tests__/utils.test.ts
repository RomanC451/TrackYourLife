import { describe, expect, it } from "vitest";

import { cn, formatDate, zip } from "../utils";

describe("cn", () => {
  it("merges class names and resolves tailwind conflicts", () => {
    expect(cn("px-2", "px-4")).toBe("px-4");
    const includeHidden = false;
    expect(cn("text-red-500", includeHidden && "hidden", "font-bold")).toBe(
      "text-red-500 font-bold",
    );
  });
});

describe("formatDate", () => {
  it("formats an ISO date string in en-US short form", () => {
    expect(formatDate("2026-06-05T12:00:00Z")).toBe("Jun 5, 2026");
  });
});

describe("zip", () => {
  it("pairs elements from two arrays by index", () => {
    expect(zip(["a", "b"], [1, 2])).toEqual([
      ["a", 1],
      ["b", 2],
    ]);
  });

  it("leaves undefined for missing right-hand values", () => {
    expect(zip(["a"], [])).toEqual([["a", undefined]]);
  });
});
