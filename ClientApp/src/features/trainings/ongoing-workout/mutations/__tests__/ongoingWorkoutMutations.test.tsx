import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { queryClient } from "@/queryClient";
import { Difficulty, type OngoingTrainingDto } from "@/services/openapi";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

import { ongoingTrainingsQueryKeys } from "../../queries/ongoingTrainingsQuery";

const {
  mockNextOngoingTraining,
  mockPreviousOngoingTraining,
  mockDeleteOngoingTraining,
  mockSkipExercise,
  mockFinishOngoingTraining,
  mockCreateOngoingTraining,
  mockAdjustExerciseSets,
  mockJumpToExercise,
  mockDeleteExerciseHistory,
} = vi.hoisted(() => ({
  mockNextOngoingTraining: vi.fn(),
  mockPreviousOngoingTraining: vi.fn(),
  mockDeleteOngoingTraining: vi.fn(),
  mockSkipExercise: vi.fn(),
  mockFinishOngoingTraining: vi.fn(),
  mockCreateOngoingTraining: vi.fn(),
  mockAdjustExerciseSets: vi.fn(),
  mockJumpToExercise: vi.fn(),
  mockDeleteExerciseHistory: vi.fn(),
}));

vi.mock("@/services/openapi/api", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi/api")>();
  class MockOngoingTrainingsApi {
    nextOngoingTraining = mockNextOngoingTraining;
    previousOngoingTraining = mockPreviousOngoingTraining;
    deleteOngoingTraining = mockDeleteOngoingTraining;
    skipExercise = mockSkipExercise;
    finishOngoingTraining = mockFinishOngoingTraining;
    createOngoingTraining = mockCreateOngoingTraining;
    adjustExerciseSets = mockAdjustExerciseSets;
    jumpToExercise = mockJumpToExercise;
  }
  class MockExercisesHistoriesApi {
    deleteExerciseHistory = mockDeleteExerciseHistory;
  }
  return {
    ...actual,
    OngoingTrainingsApi: MockOngoingTrainingsApi,
    ExercisesHistoriesApi: MockExercisesHistoriesApi,
  };
});

vi.mock("sonner", () => ({
  toast: { error: vi.fn() },
}));

import useAdjustExerciseMutation from "../useAdjustExerciseMutation";
import useCreateOngoingTrainingMutation from "../useCreateOngoingTrainingMutation";
import useDeleteExerciseHistoryMutation from "../useDeleteExerciseHistoryMutation";
import useDeleteOngoingTrainingMutation from "../useDeleteOngoingTrainingMutation";
import useFinishOngoingTrainingMutation from "../useFinishOngoingTrainingMutation";
import useJumpToExerciseMutation from "../useJumpToExerciseMutation";
import useNextOngoingTrainingMutation from "../useNextOngoingTrainingMutation";
import usePreviousOngoingTrainingMutation from "../usePreviousOngoingTrainingMutation";
import useSkipExerciseMutation from "../useSkipExerciseMutation";

function ongoingSession(): OngoingTrainingDto {
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
            {
              id: "set-2",
              name: "Set 2",
              orderIndex: 1,
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
    setIndex: 0,
    startedOnUtc: "2026-01-01T10:00:00Z",
    hasNext: true,
    isLastSet: false,
    isFirstSet: true,
    isLastExercise: false,
    isFirstExercise: true,
    completedExerciseIds: [],
    skippedExerciseIds: [],
    isLoading: false,
    isDeleting: false,
  };
}

describe("ongoing workout mutations", () => {
  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
    queryClient.setQueryData(
      ongoingTrainingsQueryKeys.active,
      ongoingSession(),
    );
    mockNextOngoingTraining.mockResolvedValue({ data: undefined });
    mockPreviousOngoingTraining.mockResolvedValue({ data: undefined });
    mockDeleteOngoingTraining.mockResolvedValue({ data: undefined });
    mockSkipExercise.mockResolvedValue({ data: undefined });
    mockFinishOngoingTraining.mockResolvedValue({ data: undefined });
    mockCreateOngoingTraining.mockResolvedValue({ data: undefined });
    mockAdjustExerciseSets.mockResolvedValue({ data: undefined });
    mockJumpToExercise.mockResolvedValue({ data: undefined });
    mockDeleteExerciseHistory.mockResolvedValue({ data: undefined });
  });

  it("advances the active session optimistically on next", async () => {
    const { result } = renderHook(() => useNextOngoingTrainingMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        ongoingTraining: ongoingSession(),
      });
    });

    expect(queryClient.getQueryData(ongoingTrainingsQueryKeys.active)).toMatchObject({
      setIndex: 1,
      isLoading: true,
    });
  });

  it("moves back optimistically on previous", async () => {
    queryClient.setQueryData(ongoingTrainingsQueryKeys.active, {
      ...ongoingSession(),
      setIndex: 1,
      isFirstSet: false,
    });

    const { result } = renderHook(() => usePreviousOngoingTrainingMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        ongoingTraining: ongoingSession(),
      });
    });

    expect(queryClient.getQueryData(ongoingTrainingsQueryKeys.active)).toMatchObject({
      setIndex: 0,
      isLoading: true,
    });
  });

  it("clears the active session on delete", async () => {
    const { result } = renderHook(() => useDeleteOngoingTrainingMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ ongoingTrainingId: "ongoing-1" });
    });

    expect(queryClient.getQueryData(ongoingTrainingsQueryKeys.active)).toBeNull();
  });

  it("skips to the next exercise optimistically", async () => {
    const { result } = renderHook(() => useSkipExerciseMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        ongoingTraining: ongoingSession(),
      });
    });

    expect(queryClient.getQueryData(ongoingTrainingsQueryKeys.active)).toMatchObject({
      exerciseIndex: 0,
      setIndex: 0,
      isLoading: true,
    });
  });

  it("finishes an ongoing training and removes by-id cache", async () => {
    queryClient.setQueryData(
      ongoingTrainingsQueryKeys.byId("ongoing-1"),
      ongoingSession(),
    );

    const { result } = renderHook(() => useFinishOngoingTrainingMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        ongoingTraining: ongoingSession(),
        caloriesBurned: 350,
      });
    });

    expect(mockFinishOngoingTraining).toHaveBeenCalledWith("ongoing-1", {
      caloriesBurned: 350,
    });
    expect(
      queryClient.getQueryData(ongoingTrainingsQueryKeys.byId("ongoing-1")),
    ).toBeUndefined();
  });

  it("creates an ongoing training", async () => {
    const { result } = renderHook(() => useCreateOngoingTrainingMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ trainingId: "training-1" });
    });

    expect(mockCreateOngoingTraining).toHaveBeenCalledWith({
      trainingId: "training-1",
    });
  });

  it("adjusts exercise sets", async () => {
    const { result } = renderHook(() => useAdjustExerciseMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        ongoingTrainingId: "ongoing-1",
        exerciseId: "ex-1",
        newSets: [
          {
            id: "set-1",
            name: "Set 1",
            orderIndex: 0,
            count1: 12,
            unit1: "reps",
          },
        ],
      });
    });

    expect(mockAdjustExerciseSets).toHaveBeenCalledWith("ongoing-1", {
      exerciseId: "ex-1",
      newExerciseSets: [
        {
          id: "set-1",
          name: "Set 1",
          orderIndex: 0,
          count1: 12,
          unit1: "reps",
        },
      ],
    });
  });

  it("jumps to another exercise", async () => {
    const { result } = renderHook(() => useJumpToExerciseMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        ongoingTraining: ongoingSession(),
        exerciseIndex: 1,
      });
    });

    expect(mockJumpToExercise).toHaveBeenCalledWith({
      ongoingTrainingId: "ongoing-1",
      exerciseIndex: 1,
    });
  });

  it("deletes exercise history and invalidates cache", async () => {
    const { result } = renderHook(() => useDeleteExerciseHistoryMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        id: "hist-1",
        exerciseId: "ex-1",
      });
    });

    expect(mockDeleteExerciseHistory).toHaveBeenCalledWith("hist-1");
  });
});
