import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { food, recipe } from "@/features/nutrition/__tests__/fixtures";
import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { queryClient } from "@/queryClient";
import {
  DiaryType,
  GetNutritionDiariesByDateResponse,
  MealTypes,
  type NutritionDiaryDto,
} from "@/services/openapi";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

import {
  nutritionDiariesQueryKeys,
} from "../../queries/useDiaryQuery";

const {
  mockAddFoodDiary,
  mockUpdateFoodDiary,
  mockDeleteFoodDiary,
  mockAddRecipeDiary,
  mockDeleteRecipeDiary,
  mockUpdateRecipeDiary,
} = vi.hoisted(() => ({
  mockAddFoodDiary: vi.fn(),
  mockUpdateFoodDiary: vi.fn(),
  mockDeleteFoodDiary: vi.fn(),
  mockAddRecipeDiary: vi.fn(),
  mockDeleteRecipeDiary: vi.fn(),
  mockUpdateRecipeDiary: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockFoodDiariesApi {
    addFoodDiary = mockAddFoodDiary;
    updateFoodDiary = mockUpdateFoodDiary;
    deleteFoodDiary = mockDeleteFoodDiary;
  }
  class MockRecipeDiariesApi {
    addRecipeDiary = mockAddRecipeDiary;
    deleteRecipeDiary = mockDeleteRecipeDiary;
    updateRecipeDiary = mockUpdateRecipeDiary;
  }
  return {
    ...actual,
    FoodDiariesApi: MockFoodDiariesApi,
    RecipeDiariesApi: MockRecipeDiariesApi,
  };
});

vi.mock("@/services/openapi/api", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi/api")>();
  class MockFoodDiariesApi {
    addFoodDiary = mockAddFoodDiary;
    updateFoodDiary = mockUpdateFoodDiary;
    deleteFoodDiary = mockDeleteFoodDiary;
  }
  class MockRecipeDiariesApi {
    addRecipeDiary = mockAddRecipeDiary;
    deleteRecipeDiary = mockDeleteRecipeDiary;
    updateRecipeDiary = mockUpdateRecipeDiary;
  }
  return {
    ...actual,
    FoodDiariesApi: MockFoodDiariesApi,
    RecipeDiariesApi: MockRecipeDiariesApi,
  };
});

vi.mock("@/features/nutrition/common/queries/useFoodSearchQuery", () => ({
  invalidateFoodSearchQuery: vi.fn(),
}));

import useAddFoodDiaryMutation from "../useAddFoodDiaryMutation";
import useAddRecipeDiaryMutation from "../useAddRecipeDiaryMutation";
import useDeleteNutritionDiaryMutation from "../useDeleteNutritionDiaryMutation";
import useUpdateFoodDiaryMutation from "../useUpdateFoodDiaryMutation";
import useUpdateRecipeDiaryMutation from "../useUpdateRecipeDiaryMutation";

function diaryEntry(id: string): NutritionDiaryDto {
  const nutrition = createEmptyNutritionalContent();
  nutrition.energy = { unit: "calories", value: 200 };

  return {
    id,
    name: "Oats",
    mealType: MealTypes.Breakfast,
    diaryType: DiaryType.FoodDiary,
    date: "2026-06-05",
    nutritionalContents: nutrition,
    nutritionMultiplier: 1,
    quantity: 1,
    isLoading: false,
    isDeleting: false,
  };
}

describe("diary mutations", () => {
  const oats = food("food-1", "Oats");
  oats.nutritionalContents = createEmptyNutritionalContent();
  oats.nutritionalContents.energy = { unit: "calories", value: 200 };
  oats.nutritionalContents.protein = 8;

  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
    mockAddFoodDiary.mockResolvedValue({ data: { id: "diary-new" } });
    mockUpdateFoodDiary.mockResolvedValue(undefined);
    mockDeleteFoodDiary.mockResolvedValue(undefined);
    mockAddRecipeDiary.mockResolvedValue({ data: { id: "recipe-diary-new" } });
    mockDeleteRecipeDiary.mockResolvedValue(undefined);
    mockUpdateRecipeDiary.mockResolvedValue(undefined);

    queryClient.setQueryData(nutritionDiariesQueryKeys.byDate("2026-06-05"), {
      diaries: {
        [MealTypes.Breakfast]: [diaryEntry("diary-1")],
        [MealTypes.Lunch]: [],
        [MealTypes.Dinner]: [],
        [MealTypes.Snacks]: [],
      },
    });
    queryClient.setQueryData(
      nutritionDiariesQueryKeys.overview("2026-06-05", "2026-06-05"),
      createEmptyNutritionalContent(),
    );
  });

  describe("useAddFoodDiaryMutation", () => {
    it("appends a new diary entry to the meal cache on success", async () => {
      const { result } = renderHook(() => useAddFoodDiaryMutation(oats), {
        wrapper: createQueryClientWrapper(),
      });

      await act(async () => {
        await result.current.mutateAsync({
          foodId: "food-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 2,
          entryDate: "2026-06-05",
        });
      });

      const cached = queryClient.getQueryData<GetNutritionDiariesByDateResponse>(
        nutritionDiariesQueryKeys.byDate("2026-06-05"),
      )!;
      expect(cached.diaries[MealTypes.Breakfast]).toHaveLength(2);
      expect(cached.diaries[MealTypes.Breakfast][1]).toMatchObject({
        id: "diary-new",
        name: "Oats",
        quantity: 2,
        isLoading: true,
      });
    });
  });

  describe("useUpdateFoodDiaryMutation", () => {
    it("updates an existing diary entry in the meal cache on success", async () => {
      const { result } = renderHook(() => useUpdateFoodDiaryMutation(oats), {
        wrapper: createQueryClientWrapper(),
      });

      await act(async () => {
        await result.current.mutateAsync({
          id: "diary-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 3,
          entryDate: "2026-06-05",
        });
      });

      const cached = queryClient.getQueryData<GetNutritionDiariesByDateResponse>(
        nutritionDiariesQueryKeys.byDate("2026-06-05"),
      )!;
      expect(cached.diaries[MealTypes.Breakfast][0]).toMatchObject({
        id: "diary-1",
        quantity: 3,
      });
      expect(mockUpdateFoodDiary).toHaveBeenCalledWith("diary-1", {
        mealType: MealTypes.Breakfast,
        servingSizeId: "ss-1",
        quantity: 3,
        entryDate: "2026-06-05",
      });
    });
  });

  describe("useAddRecipeDiaryMutation", () => {
    it("appends a recipe diary entry on success", async () => {
      const breakfastBowl = recipe("recipe-1", "Breakfast bowl", { portions: 2 });
      breakfastBowl.nutritionalContents = createEmptyNutritionalContent();
      breakfastBowl.nutritionalContents.energy = {
        unit: "calories",
        value: 400,
      };

      const { result } = renderHook(
        () => useAddRecipeDiaryMutation(breakfastBowl),
        { wrapper: createQueryClientWrapper() },
      );

      await act(async () => {
        await result.current.mutateAsync({
          recipeId: "recipe-1",
          mealType: MealTypes.Lunch,
          servingSizeId: "ss-1",
          quantity: 2,
          entryDate: "2026-06-05",
        });
      });

      const cached = queryClient.getQueryData<GetNutritionDiariesByDateResponse>(
        nutritionDiariesQueryKeys.byDate("2026-06-05"),
      )!;
      expect(cached.diaries[MealTypes.Lunch]).toHaveLength(1);
      expect(cached.diaries[MealTypes.Lunch][0]).toMatchObject({
        id: "recipe-diary-new",
        name: "Breakfast bowl",
        quantity: 2,
        diaryType: DiaryType.RecipeDiary,
        isLoading: true,
      });
    });
  });

  describe("useDeleteNutritionDiaryMutation", () => {
    it("marks a food diary entry as deleting on success", async () => {
      const entry = diaryEntry("diary-1");
      const { result } = renderHook(() => useDeleteNutritionDiaryMutation(), {
        wrapper: createQueryClientWrapper(),
      });

      await act(async () => {
        await result.current.mutateAsync(entry);
      });

      const cached = queryClient.getQueryData<GetNutritionDiariesByDateResponse>(
        nutritionDiariesQueryKeys.byDate("2026-06-05"),
      )!;
      expect(cached.diaries[MealTypes.Breakfast][0].isDeleting).toBe(true);
    });

    it("calls the recipe diary API for recipe entries", async () => {
      const entry = {
        ...diaryEntry("recipe-diary-1"),
        diaryType: DiaryType.RecipeDiary,
      };
      const { result } = renderHook(() => useDeleteNutritionDiaryMutation(), {
        wrapper: createQueryClientWrapper(),
      });

      await act(async () => {
        await result.current.mutateAsync(entry);
      });

      expect(mockDeleteRecipeDiary).toHaveBeenCalledWith("recipe-diary-1");
      expect(mockDeleteFoodDiary).not.toHaveBeenCalled();
    });
  });

  describe("useUpdateRecipeDiaryMutation", () => {
    it("updates a recipe diary entry in cache on success", async () => {
      const recipeDiary = {
        ...diaryEntry("recipe-diary-1"),
        diaryType: DiaryType.RecipeDiary,
        quantity: 1,
      };
      queryClient.setQueryData(nutritionDiariesQueryKeys.byDate("2026-06-05"), {
        diaries: {
          [MealTypes.Breakfast]: [recipeDiary],
          [MealTypes.Lunch]: [],
          [MealTypes.Dinner]: [],
          [MealTypes.Snacks]: [],
        },
      });

      const { result } = renderHook(() => useUpdateRecipeDiaryMutation(), {
        wrapper: createQueryClientWrapper(),
      });

      await act(async () => {
        await result.current.mutateAsync({
          id: "recipe-diary-1",
          mealType: MealTypes.Breakfast,
          servingSizeId: "ss-1",
          quantity: 3,
          entryDate: "2026-06-05",
        });
      });

      const cached = queryClient.getQueryData<GetNutritionDiariesByDateResponse>(
        nutritionDiariesQueryKeys.byDate("2026-06-05"),
      )!;
      expect(cached.diaries[MealTypes.Breakfast][0].quantity).toBe(3);
      expect(mockUpdateRecipeDiary).toHaveBeenCalledWith("recipe-diary-1", {
        mealType: MealTypes.Breakfast,
        servingSizeId: "ss-1",
        quantity: 3,
        entryDate: "2026-06-05",
      });
    });
  });
});
