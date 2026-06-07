import { describe, expect, it } from "vitest";

import type { ServingSizeDto } from "@/services/openapi";

import { getServingSizeIndex } from "../filters";

function servingSize(id: string): ServingSizeDto {
  return {
    id,
    nutritionMultiplier: 1,
    isLoading: false,
    isDeleting: false,
  } as ServingSizeDto;
}

describe("getServingSizeIndex", () => {
  const servingSizes = {
    a: servingSize("ss-a"),
    b: servingSize("ss-b"),
    c: servingSize("ss-c"),
  };

  it("returns the index of a matching serving size", () => {
    expect(getServingSizeIndex(servingSizes, servingSize("ss-b"))).toBe(1);
  });

  it("returns -1 when the serving size is not found", () => {
    expect(getServingSizeIndex(servingSizes, servingSize("missing"))).toBe(-1);
  });
});
