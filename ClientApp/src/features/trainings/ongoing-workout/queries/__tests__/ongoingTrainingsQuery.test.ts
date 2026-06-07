import { beforeEach, describe, expect, it, vi } from "vitest";

import { ongoingTraining as ongoingTrainingFixture } from "@/features/trainings/__tests__/fixtures";
import { Difficulty, type OngoingTrainingDto } from "@/services/openapi";
import { queryClient } from "@/queryClient";

const { mockGetActiveOngoingTraining } = vi.hoisted(() => ({
  mockGetActiveOngoingTraining: vi.fn(),
}));

vi.mock("@/services/openapi/api", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi/api")>();
  class MockOngoingTrainingsApi {
    getActiveOngoingTraining = mockGetActiveOngoingTraining;
  }
  return { ...actual, OngoingTrainingsApi: MockOngoingTrainingsApi };
});

import {
  ensureActiveOngoingTraining,
  ongoingTrainingsQueryKeys,
  setActiveOngoingTrainingQueryDataByAction,
} from "../ongoingTrainingsQuery";
function ongoingTraining(setIndex: number): OngoingTrainingDto {
  return {
    id: "ongoing-1",
    training: {
      id: "training-1",
      name: "Workout",
      difficulty: Difficulty.Easy,
      muscleGroups: [],
      duration: 60,
      restSeconds: 60,
      exercises: [
        {
          id: "ex-1",
          name: "Squat",
          muscleGroups: [],
          difficulty: Difficulty.Easy,
          exerciseSets: [
            {
              id: "set-1",
              name: "Set 1",
              orderIndex: 0,
              count1: 10,
              unit1: "reps",
            },
          ],
          createdOnUtc: "2026-01-01T09:00:00Z",
          isLoading: false,
          isDeleting: false,
        },
      ],
      createdOnUtc: "2026-01-01T09:00:00Z",
      isLoading: false,
      isDeleting: false,
    },
    exerciseIndex: 0,
    setIndex,
    startedOnUtc: "2026-01-01T10:00:00Z",
    hasNext: true,
    isLastSet: false,
    isFirstSet: setIndex === 0,
    isLastExercise: false,
    isFirstExercise: true,
    completedExerciseIds: [],
    skippedExerciseIds: [],
    isLoading: false,
    isDeleting: false,
  };
}

describe("ongoingTrainingsQueryKeys", () => {
  it("builds stable keys for active and by-id queries", () => {
    expect(ongoingTrainingsQueryKeys.active).toEqual([
      "ongoingTrainings",
      "active",
    ]);
    expect(ongoingTrainingsQueryKeys.byId("ongoing-1")).toEqual([
      "ongoingTrainings",
      "ongoing-1",
    ]);
  });
});

describe("setActiveOngoingTrainingQueryDataByAction", () => {
  it("applies navigation and marks the session as loading", () => {
    const oldData = ongoingTraining(0);
    queryClient.setQueryData(ongoingTrainingsQueryKeys.active, oldData);

    setActiveOngoingTrainingQueryDataByAction({ action: "next" });

    expect(queryClient.getQueryData(ongoingTrainingsQueryKeys.active)).toEqual({
      ...oldData,
      setIndex: 1,
      isLoading: true,
    });
  });
});

describe("ensureActiveOngoingTraining", () => {
  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
    mockGetActiveOngoingTraining.mockResolvedValue({
      data: ongoingTrainingFixture(),
    });
  });

  it("fills the active ongoing training cache", async () => {
    await ensureActiveOngoingTraining();

    expect(queryClient.getQueryData(ongoingTrainingsQueryKeys.active)).toEqual(
      ongoingTrainingFixture(),
    );
  });
});