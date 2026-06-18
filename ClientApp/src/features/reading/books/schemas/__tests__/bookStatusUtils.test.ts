import { describe, expect, it } from "vitest";

import { BookStatus } from "@/services/openapi";

import {
  normalizeBookStatus,
  showsFinishedFields,
  showsStartingDate,
} from "../bookStatusUtils";

describe("bookStatusUtils", () => {
  it("normalizes string statuses", () => {
    expect(normalizeBookStatus("Ongoing")).toBe(BookStatus.Ongoing);
    expect(normalizeBookStatus("Finished")).toBe(BookStatus.Finished);
  });

  it("normalizes numeric statuses", () => {
    expect(normalizeBookStatus(1)).toBe(BookStatus.Ongoing);
    expect(normalizeBookStatus(2)).toBe(BookStatus.Finished);
  });

  it("shows fields based on status", () => {
    expect(showsStartingDate(BookStatus.Ongoing)).toBe(true);
    expect(showsStartingDate(BookStatus.NotStarted)).toBe(false);
    expect(showsFinishedFields(BookStatus.Finished)).toBe(true);
    expect(showsFinishedFields(BookStatus.Ongoing)).toBe(false);
  });
});
