import { beforeEach, describe, expect, it, vi } from "vitest";

import { queryClient } from "@/queryClient";

import { foodQueryKeys } from "../useFoodQuery";
import {
  invalidateFoodSearchQuery,
  removeFoodSearchQuery,
} from "../useFoodSearchQuery";

describe("food search query helpers", () => {
  beforeEach(() => {
    queryClient.clear();
    vi.restoreAllMocks();
  });

  it("invalidates all food search queries", () => {
    const spy = vi.spyOn(queryClient, "invalidateQueries");

    invalidateFoodSearchQuery();

    expect(spy).toHaveBeenCalledWith({
      queryKey: foodQueryKeys.searches(),
    });
  });

  it("removes queries for a specific search term", () => {
    const spy = vi.spyOn(queryClient, "removeQueries");

    removeFoodSearchQuery("oats");

    expect(spy).toHaveBeenCalledWith({
      queryKey: foodQueryKeys.search("oats"),
    });
  });

  it("removes all food search queries when no term is provided", () => {
    const spy = vi.spyOn(queryClient, "removeQueries");

    removeFoodSearchQuery();

    expect(spy).toHaveBeenCalledWith({
      queryKey: foodQueryKeys.searches(),
    });
  });
});
