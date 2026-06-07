import { StatusCodes } from "http-status-codes";
import { describe, expect, it, vi } from "vitest";

import { food } from "@/features/nutrition/__tests__/fixtures";
import { queryClient } from "@/queryClient";
import { createQueryFnContext } from "@/test/queryFnContext";

import { foodQueryKeys, foodQueryOptions } from "../useFoodQuery";

const mockGetFoodById = vi.hoisted(() => vi.fn());
const mockSearchFoodsByName = vi.hoisted(() => vi.fn());

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockFoodsApi {
    getFoodById = mockGetFoodById;
    searchFoodsByName = mockSearchFoodsByName;
  }
  return { ...actual, FoodsApi: MockFoodsApi };
});

describe("foodQueryKeys", () => {
  it("builds stable food query keys", () => {
    expect(foodQueryKeys.byId("food-1")).toEqual(["foods", "food-1"]);
    expect(foodQueryKeys.search("oats")).toEqual([
      "foods",
      "search",
      "oats",
    ]);
  });
});

describe("foodQueryOptions.search getNextPageParam", () => {
  const getNextPageParam = foodQueryOptions.search("oats", () => {})
    .getNextPageParam as (
    lastPage?: { hasNextPage: boolean; page: number; items: unknown[] },
  ) => number | undefined;

  it("returns the next page when more results exist", () => {
    expect(
      getNextPageParam({ hasNextPage: true, page: 2, items: [] }),
    ).toBe(3);
  });

  it("returns undefined when there is no next page", () => {
    expect(
      getNextPageParam({ hasNextPage: false, page: 2, items: [] }),
    ).toBeUndefined();
  });

  it("returns undefined when the last page is missing", () => {
    expect(getNextPageParam(undefined)).toBeUndefined();
  });
});

describe("foodQueryOptions.byId", () => {
  it("fetches food by id", async () => {
    const oats = food("food-1", "Oats");
    mockGetFoodById.mockResolvedValue({ data: oats });

    const result = await foodQueryOptions.byId("food-1").queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: foodQueryKeys.byId("food-1"),
      }),
    );

    expect(result).toEqual(oats);
    expect(mockGetFoodById).toHaveBeenCalledWith("food-1");
  });
});

describe("foodQueryOptions.search retry", () => {
  const setError = vi.fn();
  const retry = foodQueryOptions.search("oats", setError).retry as (
    failureCount: number,
    error: unknown,
  ) => boolean;

  it("invokes notFoundCallback and stops retrying on 404", () => {
    const shouldRetry = retry(0, {
      status: StatusCodes.NOT_FOUND,
    } as never);

    expect(shouldRetry).toBe(false);
    expect(setError).toHaveBeenCalledWith("No foods found");
  });

  it("retries on transient errors", () => {
    const shouldRetry = retry(0, {
      response: { status: StatusCodes.INTERNAL_SERVER_ERROR, data: {} },
    } as never);

    expect(shouldRetry).toBe(true);
  });
});

describe("foodQueryOptions.search queryFn", () => {
  it("searches foods by name with pagination", async () => {
    mockSearchFoodsByName.mockResolvedValue({
      data: { items: [food("food-1", "Oats")], page: 1, hasNextPage: false },
    });

    const result = await foodQueryOptions.search("oats", vi.fn()).queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: foodQueryKeys.search("oats"),
        pageParam: 2,
        direction: "forward",
      }),
    );

    expect(mockSearchFoodsByName).toHaveBeenCalledWith("oats", 2, 10);
    expect(result.items).toHaveLength(1);
  });
});
