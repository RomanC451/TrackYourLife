import { beforeEach, describe, expect, it, vi } from "vitest";

import { exercise } from "@/features/trainings/__tests__/fixtures";
import { queryClient } from "@/queryClient";

const { mockGetExercises, mockGetExerciseById, mockPreloadImage } = vi.hoisted(
  () => ({
    mockGetExercises: vi.fn(),
    mockGetExerciseById: vi.fn(),
    mockPreloadImage: vi.fn(),
  }),
);

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockExercisesApi {
    getExercises = mockGetExercises;
    getExerciseById = mockGetExerciseById;
  }
  return { ...actual, ExercisesApi: MockExercisesApi };
});

vi.mock("@/services/openapi/preload", () => ({
  preloadImage: mockPreloadImage,
}));

import {
  ensureExerciseByIdWithPicturePreload,
  ensureExercisesList,
  exercisesQueryKeys,
} from "../exercisesQuery";

describe("exercisesQueryKeys", () => {
  it("builds list and by-id keys", () => {
    expect(exercisesQueryKeys.all).toEqual(["exercises"]);
    expect(exercisesQueryKeys.byId("ex-1")).toEqual(["exercises", "ex-1"]);
  });
});

describe("ensureExerciseByIdWithPicturePreload", () => {
  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
  });

  it("preloads the exercise picture when present", async () => {
    const bench = exercise("ex-1", {
      pictureUrl: "https://example.com/bench.jpg",
    });
    mockGetExerciseById.mockResolvedValue({ data: bench });

    const result = await ensureExerciseByIdWithPicturePreload("ex-1");

    expect(result).toEqual(bench);
    expect(mockPreloadImage).toHaveBeenCalledWith(
      "https://example.com/bench.jpg",
    );
  });

  it("skips preload when picture url is missing", async () => {
    const squat = exercise("ex-2", { pictureUrl: undefined });
    mockGetExerciseById.mockResolvedValue({ data: squat });

    await ensureExerciseByIdWithPicturePreload("ex-2");

    expect(mockPreloadImage).not.toHaveBeenCalled();
  });
});

describe("ensureExercisesList", () => {
  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
    mockGetExercises.mockResolvedValue({
      data: [exercise("ex-1")],
    });
  });

  it("fills the exercises list cache", async () => {
    await ensureExercisesList();

    expect(queryClient.getQueryData(exercisesQueryKeys.all)).toEqual([
      exercise("ex-1"),
    ]);
  });
});