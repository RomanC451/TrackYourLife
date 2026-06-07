import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { goal } from "@/features/nutrition/__tests__/fixtures";
import { queryClient } from "@/queryClient";
import { GoalType, type GoalDto } from "@/services/openapi";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

import { nutritionGoalsQueryKeys } from "../../queries/nutritionGoalsQuery";

const {
  mockAddGoal,
  mockUpdateGoal,
  mockUpdateNutritionGoals,
  mockCalculateNutritionGoals,
} = vi.hoisted(() => ({
  mockAddGoal: vi.fn(),
  mockUpdateGoal: vi.fn(),
  mockUpdateNutritionGoals: vi.fn(),
  mockCalculateNutritionGoals: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockGoalsApi {
    addGoal = mockAddGoal;
    updateGoal = mockUpdateGoal;
    updateNutritionGoals = mockUpdateNutritionGoals;
    calculateNutritionGoals = mockCalculateNutritionGoals;
  }
  return { ...actual, GoalsApi: MockGoalsApi };
});

import useCalculateNutritionGoalsMutation from "../useCalculateNutritionGoalsMutation";
import useUpdateCaloriesGoalMutation from "../useUpdateCaloriesGoalMutation";
import useUpdateGoalMutation from "../useUpdateGoalMutation";
import useUpdateNutritionGoalsMutation from "../useUpdateNutritionGoalsMutation";

describe("goals mutations", () => {
  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
    mockAddGoal.mockResolvedValue({ data: goal(GoalType.Calories, 2200) });
    mockUpdateGoal.mockResolvedValue({ data: goal(GoalType.Protein, 160) });
    mockUpdateNutritionGoals.mockResolvedValue({ data: undefined });
    mockCalculateNutritionGoals.mockResolvedValue({
      data: {
        calories: 2200,
        proteins: 160,
        carbs: 240,
        fat: 75,
      },
    });
    queryClient.setQueryData(nutritionGoalsQueryKeys.all, [
      goal(GoalType.Calories, 2000),
    ]);
  });

  describe("useUpdateCaloriesGoalMutation", () => {
    it("appends the new calories goal to the cache on success", async () => {
      const { result } = renderHook(() => useUpdateCaloriesGoalMutation(), {
        wrapper: createQueryClientWrapper(),
      });

      await act(async () => {
        await result.current.setCaloriesGoalMutation.mutateAsync({
          value: 2200,
        });
      });

      const cached = queryClient.getQueryData<GoalDto[]>(nutritionGoalsQueryKeys.all)!;
      expect(cached).toHaveLength(2);
      expect(cached[1]).toMatchObject({
        type: GoalType.Calories,
        value: 2200,
      });
    });
  });

  describe("useUpdateGoalMutation", () => {
    it("exposes the mutation and clears previous errors on mutate", async () => {
      const { result } = renderHook(() => useUpdateGoalMutation(), {
        wrapper: createQueryClientWrapper(),
      });

      await act(async () => {
        await result.current.updateGoalMutation.mutateAsync({
          id: "protein-goal",
          type: GoalType.Protein,
          value: 160,
        });
      });

      expect(mockUpdateGoal).toHaveBeenCalled();
      expect(result.current.error).toBeUndefined();
    });
  });

  describe("useUpdateNutritionGoalsMutation", () => {
    it("calls the bulk goals update API", async () => {
      const { result } = renderHook(() => useUpdateNutritionGoalsMutation(), {
        wrapper: createQueryClientWrapper(),
      });

      await act(async () => {
        await result.current.mutateAsync({
          calories: 2200,
          protein: 160,
          carbohydrates: 240,
          fats: 75,
        });
      });

      await waitFor(() =>
        expect(mockUpdateNutritionGoals).toHaveBeenCalledWith({
          calories: 2200,
          protein: 160,
          carbohydrates: 240,
          fats: 75,
        }),
      );
    });
  });

  describe("useCalculateNutritionGoalsMutation", () => {
    it("calls the calculator API", async () => {
      const { result } = renderHook(
        () => useCalculateNutritionGoalsMutation(),
        { wrapper: createQueryClientWrapper() },
      );

      await act(async () => {
        await result.current.mutateAsync({
          age: 30,
          weight: 75,
          height: 180,
          gender: "Male",
          activityLevel: "ModeratelyActive",
          fitnessGoal: "Maintain",
          force: false,
        });
      });

      expect(mockCalculateNutritionGoals).toHaveBeenCalled();
    });
  });
});
