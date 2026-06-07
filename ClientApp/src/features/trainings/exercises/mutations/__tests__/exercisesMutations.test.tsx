import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { exercise } from "@/features/trainings/__tests__/fixtures";
import { queryClient } from "@/queryClient";
import { Difficulty, type ExerciseDto } from "@/services/openapi";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

import { exercisesQueryKeys } from "../../queries/exercisesQuery";

const { mockCreateExercise, mockUpdateExercise, mockDeleteExercise } =
  vi.hoisted(() => ({
    mockCreateExercise: vi.fn(),
    mockUpdateExercise: vi.fn(),
    mockDeleteExercise: vi.fn(),
  }));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockExercisesApi {
    createExercise = mockCreateExercise;
    updateExercise = mockUpdateExercise;
    deleteExercise = mockDeleteExercise;
  }
  return { ...actual, ExercisesApi: MockExercisesApi };
});

vi.mock("uuid", () => ({ v4: () => "new-exercise-id" }));

import useCreateExerciseMutation from "../useCreateExerciseMutation";
import useDeleteExerciseMutation from "../useDeleteExerciseMutation";
import useUpdateExerciseMutation from "../useUpdateExerciseMutation";

const formRequest = {
  id: "ex-1",
  name: "Bench press",
  muscleGroups: ["chest"],
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
};

describe("exercises mutations", () => {
  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
    queryClient.setQueryData(exercisesQueryKeys.all, [
      exercise("a", { name: "Alpha" }),
      exercise("b", { name: "Beta" }),
    ]);
    mockCreateExercise.mockResolvedValue({ data: undefined });
    mockUpdateExercise.mockResolvedValue({ data: undefined });
    mockDeleteExercise.mockResolvedValue({ data: undefined });
  });

  it("appends an optimistic exercise on create", async () => {
    const { result } = renderHook(() => useCreateExerciseMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        request: formRequest,
        setError: vi.fn(),
        id: undefined,
      });
    });

    const cached = queryClient.getQueryData<ExerciseDto[]>(exercisesQueryKeys.all)!;
    expect(cached).toHaveLength(3);
    expect(cached.find((e) => e.name === "Bench press")).toMatchObject({
      isLoading: true,
    });
  });

  it("marks the exercise as loading on update", async () => {
    const { result } = renderHook(() => useUpdateExerciseMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        id: "b",
        request: { ...formRequest, name: "Beta updated" },
        setError: vi.fn(),
      });
    });

    expect(
      queryClient.getQueryData<ExerciseDto[]>(exercisesQueryKeys.all)?.find((e) => e.id === "b"),
    ).toMatchObject({ name: "Beta updated", isLoading: true });
  });

  it("marks the exercise as deleting optimistically", async () => {
    const { result } = renderHook(() => useDeleteExerciseMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        id: "a",
        forceDelete: false,
        name: "Alpha",
      });
    });

    expect(
      queryClient.getQueryData<ExerciseDto[]>(exercisesQueryKeys.all)?.find((e) => e.id === "a"),
    ).toMatchObject({ isDeleting: true });
  });
});
