import { describe, expect, it } from "vitest";

import {
  toYoutubeCategoryApiParam,
  youtubeCategoryListFilterAll,
  youtubeCategoryListFilterFavorites,
} from "../youtubeCategoryListFilters";

describe("youtubeCategoryListFilters", () => {
  it("maps built-in filters to undefined API params", () => {
    expect(toYoutubeCategoryApiParam(youtubeCategoryListFilterAll)).toBeUndefined();
    expect(toYoutubeCategoryApiParam(youtubeCategoryListFilterFavorites)).toBeUndefined();
  });

  it("passes category ids through to the API", () => {
    expect(toYoutubeCategoryApiParam("cat-123")).toBe("cat-123");
  });
});
