import { beforeEach, describe, expect, it, vi } from "vitest";

import { queryClient } from "@/queryClient";
import { Difficulty, type TrainingDto } from "@/services/openapi";

import {
  fetchTrainingById,
  findTrainingInListCache,
  selectTrainingFromList,
  TrainingNotFoundError,
  trainingsQueryKeys,
} from "../trainingsQueries";

const { mockGetTrainings } = vi.hoisted(() => ({
  mockGetTrainings: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  return {
    ...actual,
    TrainingsApi: vi.fn(() => ({
      getTrainings: mockGetTrainings,
    })),
  };
});

function training(id: string, name: string): TrainingDto {
  return {
    id,
    name,
    muscleGroups: [],
    difficulty: Difficulty.Easy,
    duration: 60,
    restSeconds: 60,
    exercises: [],
    createdOnUtc: "2026-01-01T09:00:00Z",
    isLoading: false,
    isDeleting: false,
  };
}

describe("selectTrainingFromList", () => {
  it("returns the matching training", () => {
    const trainings = [training("a", "Alpha"), training("b", "Beta")];
    expect(selectTrainingFromList(trainings, "b")?.name).toBe("Beta");
  });

  it("returns undefined when the list or id is missing", () => {
    expect(selectTrainingFromList(undefined, "a")).toBeUndefined();
    expect(selectTrainingFromList([training("a", "Alpha")], "missing")).toBeUndefined();
  });
});

describe("TrainingNotFoundError", () => {
  it("includes the training id in the message", () => {
    const error = new TrainingNotFoundError("workout-1");
    expect(error).toBeInstanceOf(Error);
    expect(error.trainingId).toBe("workout-1");
    expect(error.message).toContain("workout-1");
  });
});

describe("findTrainingInListCache", () => {
  beforeEach(() => {
    queryClient.clear();
  });

  it("reads a training from the list cache", () => {
    const trainings = [training("a", "Alpha"), training("b", "Beta")];
    queryClient.setQueryData(trainingsQueryKeys.all, trainings);

    expect(findTrainingInListCache("b")?.name).toBe("Beta");
    expect(findTrainingInListCache("missing")).toBeUndefined();
  });
});

describe("fetchTrainingById", () => {
  beforeEach(() => {
    queryClient.clear();
    mockGetTrainings.mockReset();
  });

  it("returns a cached training without calling the API", async () => {
    const cached = training("a", "Alpha");
    queryClient.setQueryData(trainingsQueryKeys.all, [cached]);

    await expect(fetchTrainingById("a")).resolves.toBe(cached);
    expect(mockGetTrainings).not.toHaveBeenCalled();
  });

  it("fetches the list when the training is not cached", async () => {
    const fetched = training("b", "Beta");
    mockGetTrainings.mockResolvedValue({ data: [fetched] });

    await expect(fetchTrainingById("b")).resolves.toBe(fetched);
    expect(mockGetTrainings).toHaveBeenCalledOnce();
    expect(queryClient.getQueryData(trainingsQueryKeys.all)).toEqual([fetched]);
  });

  it("throws when the training does not exist", async () => {
    mockGetTrainings.mockResolvedValue({ data: [training("a", "Alpha")] });

    await expect(fetchTrainingById("missing")).rejects.toBeInstanceOf(
      TrainingNotFoundError,
    );
  });
});
