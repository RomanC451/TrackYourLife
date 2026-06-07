import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { training, workoutPlan } from "@/features/trainings/__tests__/fixtures";
import { queryClient } from "@/queryClient";
import type { WorkoutPlanDto } from "@/services/openapi";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

import { trainingsQueryKeys } from "../../../trainings/queries/trainingsQueries";
import { workoutPlansQueryKeys } from "../../queries/workoutPlansQueries";

const {
  mockCreateWorkoutPlan,
  mockUpdateWorkoutPlan,
  mockDeleteWorkoutPlan,
  mockAddGoal,
} = vi.hoisted(() => ({
  mockCreateWorkoutPlan: vi.fn(),
  mockUpdateWorkoutPlan: vi.fn(),
  mockDeleteWorkoutPlan: vi.fn(),
  mockAddGoal: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockWorkoutPlansApi {
    createWorkoutPlan = mockCreateWorkoutPlan;
    updateWorkoutPlan = mockUpdateWorkoutPlan;
    deleteWorkoutPlan = mockDeleteWorkoutPlan;
  }
  class MockGoalsApi {
    addGoal = mockAddGoal;
    updateGoal = vi.fn();
  }
  return {
    ...actual,
    WorkoutPlansApi: MockWorkoutPlansApi,
    GoalsApi: MockGoalsApi,
  };
});

import useCreateWorkoutPlanMutation from "../useCreateWorkoutPlanMutation";
import useDeleteWorkoutPlanMutation from "../useDeleteWorkoutPlanMutation";
import useSetWorkoutsWeeklyGoalMutation from "../useSetWorkoutsWeeklyGoalMutation";
import useUpdateWorkoutPlanMutation from "../useUpdateWorkoutPlanMutation";

describe("workout plan mutations", () => {
  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
    queryClient.setQueryData(workoutPlansQueryKeys.all, [
      workoutPlan("plan-a"),
      workoutPlan("plan-b"),
    ]);
    queryClient.setQueryData(trainingsQueryKeys.all, [
      training("workout-1", { name: "Workout 1" }),
    ]);
    mockCreateWorkoutPlan.mockResolvedValue({ data: undefined });
    mockUpdateWorkoutPlan.mockResolvedValue({ data: undefined });
    mockDeleteWorkoutPlan.mockResolvedValue({ data: undefined });
    mockAddGoal.mockResolvedValue({ data: undefined });
  });

  it("appends an optimistic plan on create", async () => {
    const { result } = renderHook(() => useCreateWorkoutPlanMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        request: {
          name: "New plan",
          trainingIds: ["workout-1"],
          isActive: true,
        },
      });
    });

    const cached = queryClient.getQueryData<WorkoutPlanDto[]>(workoutPlansQueryKeys.all)!;
    expect(cached.some((plan) => plan.name === "New plan")).toBe(true);
  });

  it("calls the API to update a workout plan", async () => {
    const { result } = renderHook(() => useUpdateWorkoutPlanMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        id: "plan-b",
        request: {
          name: "Updated plan",
          trainingIds: ["workout-1"],
          isActive: true,
        },
      });
    });

    expect(mockUpdateWorkoutPlan).toHaveBeenCalledWith("plan-b", {
      name: "Updated plan",
      trainingIds: ["workout-1"],
      isActive: true,
    });
  });

  it("removes the plan from cache on delete", async () => {
    const { result } = renderHook(() => useDeleteWorkoutPlanMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ id: "plan-a" });
    });

    expect(
      queryClient.getQueryData<WorkoutPlanDto[]>(workoutPlansQueryKeys.all)?.map((p) => p.id),
    ).toEqual(["plan-b"]);
  });

  it("stores the weekly workouts goal in cache", async () => {
    const { result } = renderHook(() => useSetWorkoutsWeeklyGoalMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ value: 4 });
    });

    expect(mockAddGoal).toHaveBeenCalledWith(
      expect.objectContaining({
        value: 4,
        type: "Workouts",
      }),
    );
  });
});
