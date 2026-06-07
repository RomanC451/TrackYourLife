import { beforeEach, describe, expect, it, vi } from "vitest";

import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { DiaryType, GetNutritionDiariesByDateResponse, MealTypes } from "@/services/openapi";
import { queryClient } from "@/queryClient";
import { createQueryFnContext } from "@/test/queryFnContext";

import {
  applyNutritionDiariesMealUpdate,
  foodDiariesQueryKeys,
  foodDiariesQueryOptions,
  nutritionDiariesQueryKeys,
  nutritionDiariesQueryOptions,
  recipeDiariesQueryKeys,
  recipeDiariesQueryOptions,
  setNutritionDiariesQueryData,
} from "../useDiaryQuery";

const mockGetFoodDiaryById = vi.hoisted(() => vi.fn());
const mockGetNutritionDiariesByDate = vi.hoisted(() => vi.fn());
const mockGetNutritionOverviewByPeriod = vi.hoisted(() => vi.fn());
const mockGetRecipeDiaryById = vi.hoisted(() => vi.fn());

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockFoodDiariesApi {
    getFoodDiaryById = mockGetFoodDiaryById;
  }
  class MockNutritionDiariesApi {
    getNutritionDiariesByDate = mockGetNutritionDiariesByDate;
    getNutritionOverviewByPeriod = mockGetNutritionOverviewByPeriod;
  }
  class MockRecipeDiariesApi {
    getRecipeDiaryById = mockGetRecipeDiaryById;
  }
  return {
    ...actual,
    FoodDiariesApi: MockFoodDiariesApi,
    NutritionDiariesApi: MockNutritionDiariesApi,
    RecipeDiariesApi: MockRecipeDiariesApi,
  };
});

describe("diary query keys", () => {
  it("builds food diary keys", () => {
    expect(foodDiariesQueryKeys.all).toEqual(["foodDiaries"]);
    expect(foodDiariesQueryKeys.byId("fd-1")).toEqual([
      "foodDiaries",
      "fd-1",
    ]);
  });

  it("builds nutrition diary keys", () => {
    expect(nutritionDiariesQueryKeys.byDate("2026-06-05")).toEqual([
      "nutritionDiaries",
      "2026-06-05",
    ]);
    expect(
      nutritionDiariesQueryKeys.overview("2026-06-01", "2026-06-07"),
    ).toEqual([
      "nutritionDiaries",
      "overview",
      "2026-06-01",
      "2026-06-07",
    ]);
  });

  it("builds recipe diary keys", () => {
    expect(recipeDiariesQueryKeys.byId("rd-1")).toEqual([
      "recipeDiaries",
      "rd-1",
    ]);
  });
});

describe("applyNutritionDiariesMealUpdate", () => {
  const baseData = {
    diaries: {
      [MealTypes.Breakfast]: [
        {
          id: "diary-1",
          name: "Oats",
          mealType: MealTypes.Breakfast,
          diaryType: DiaryType.FoodDiary,
          date: "2026-06-05",
          nutritionalContents: createEmptyNutritionalContent(),
          nutritionMultiplier: 1,
          quantity: 1,
          isLoading: false,
          isDeleting: false,
        },
      ],
      [MealTypes.Lunch]: [],
      [MealTypes.Dinner]: [],
      [MealTypes.Snacks]: [],
    },
  };

  it("marks an entry as deleting", () => {
    const result = applyNutritionDiariesMealUpdate(baseData, {
      mealType: MealTypes.Breakfast,
      deleteDiaryId: "diary-1",
    });

    expect(result.diaries[MealTypes.Breakfast][0].isDeleting).toBe(true);
  });

  it("appends a new diary entry", () => {
    const newEntry = {
      ...baseData.diaries[MealTypes.Breakfast][0],
      id: "diary-2",
      name: "Banana",
    };

    const result = applyNutritionDiariesMealUpdate(baseData, {
      mealType: MealTypes.Breakfast,
      newDiary: newEntry,
    });

    expect(result.diaries[MealTypes.Breakfast]).toHaveLength(2);
    expect(result.diaries[MealTypes.Breakfast][1].name).toBe("Banana");
  });

  it("updates an existing diary entry", () => {
    const result = applyNutritionDiariesMealUpdate(baseData, {
      mealType: MealTypes.Breakfast,
      updatedDiary: { id: "diary-1", quantity: 3 },
    });

    expect(result.diaries[MealTypes.Breakfast][0].quantity).toBe(3);
  });
});

describe("setNutritionDiariesQueryData", () => {
  const baseData = {
    diaries: {
      [MealTypes.Breakfast]: [
        {
          id: "diary-1",
          name: "Oats",
          mealType: MealTypes.Breakfast,
          diaryType: DiaryType.FoodDiary,
          date: "2026-06-05",
          nutritionalContents: createEmptyNutritionalContent(),
          nutritionMultiplier: 1,
          quantity: 1,
          isLoading: false,
          isDeleting: false,
        },
      ],
      [MealTypes.Lunch]: [],
      [MealTypes.Dinner]: [],
      [MealTypes.Snacks]: [],
    },
  };

  beforeEach(() => {
    queryClient.clear();
    queryClient.setQueryData(
      nutritionDiariesQueryKeys.byDate("2026-06-05"),
      baseData,
    );
  });

  it("updates the nutrition diaries cache for a date", () => {
    setNutritionDiariesQueryData({
      date: "2026-06-05",
      mealType: MealTypes.Breakfast,
      updatedDiary: { id: "diary-1", quantity: 4 },
    });

    const cached = queryClient.getQueryData<GetNutritionDiariesByDateResponse>(
      nutritionDiariesQueryKeys.byDate("2026-06-05"),
    )!;
    expect(cached.diaries[MealTypes.Breakfast][0].quantity).toBe(4);
  });
});

describe("diary query options", () => {
  it("fetches a food diary by id", async () => {
    mockGetFoodDiaryById.mockResolvedValue({ data: { id: "fd-1" } });

    const result = await foodDiariesQueryOptions.byId("fd-1").queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: foodDiariesQueryKeys.byId("fd-1"),
      }),
    );

    expect(result).toEqual({ id: "fd-1" });
  });

  it("fetches nutrition diaries by date", async () => {
    mockGetNutritionDiariesByDate.mockResolvedValue({
      data: { diaries: {} },
    });

    const result = await nutritionDiariesQueryOptions.byDate("2026-06-05").queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: nutritionDiariesQueryKeys.byDate("2026-06-05"),
      }),
    );

    expect(result).toEqual({ diaries: {} });
  });

  it("keeps previous overview data as placeholder", () => {
    const options = nutritionDiariesQueryOptions.overview(
      "2026-06-01",
      "2026-06-07",
    );
    const placeholderData = options.placeholderData as unknown as (
      previous: { totalCalories: number },
    ) => { totalCalories: number };
    const previous = { totalCalories: 100 };

    expect(placeholderData(previous)).toBe(previous);
  });

  it("fetches a recipe diary by id", async () => {
    mockGetRecipeDiaryById.mockResolvedValue({ data: { id: "rd-1" } });

    const result = await recipeDiariesQueryOptions.byId("rd-1").queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: recipeDiariesQueryKeys.byId("rd-1"),
      }),
    );

    expect(result).toEqual({ id: "rd-1" });
  });

  it("fetches nutrition overview data for a date range", async () => {
    mockGetNutritionOverviewByPeriod.mockResolvedValue({
      data: { totalCalories: 1800 },
    });

    const result = await nutritionDiariesQueryOptions
      .overview("2026-06-01", "2026-06-07")
      .queryFn!(
        createQueryFnContext({
          client: queryClient,
          queryKey: nutritionDiariesQueryKeys.overview(
            "2026-06-01",
            "2026-06-07",
          ),
        }),
      );

    expect(result).toEqual({ totalCalories: 1800 });
    expect(mockGetNutritionOverviewByPeriod).toHaveBeenCalledWith(
      "2026-06-01",
      "2026-06-07",
    );
  });
});
