import { describe, expect, it } from "vitest";

import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import {
  DiaryType,
  MealTypes,
  type NutritionDiaryDto,
} from "@/services/openapi";

import { applyNutritionDiariesMealUpdate } from "../useDiaryQuery";

function diaryEntry(id: string, name: string): NutritionDiaryDto {
  return {
    id,
    name,
    mealType: MealTypes.Breakfast,
    diaryType: DiaryType.FoodDiary,
    date: "2026-06-05",
    nutritionalContents: createEmptyNutritionalContent(),
    nutritionMultiplier: 1,
    quantity: 1,
    isLoading: false,
    isDeleting: false,
  };
}

const breakfast = MealTypes.Breakfast;

describe("applyNutritionDiariesMealUpdate", () => {
  const base = {
    diaries: {
      [breakfast]: [diaryEntry("a", "Oats"), diaryEntry("b", "Eggs")],
      [MealTypes.Lunch]: [],
    },
  };

  it("marks a diary entry as deleting", () => {
    const result = applyNutritionDiariesMealUpdate(base, {
      mealType: breakfast,
      deleteDiaryId: "a",
    });

    expect(result.diaries[breakfast][0].isDeleting).toBe(true);
    expect(result.diaries[breakfast][1].isDeleting).toBe(false);
  });

  it("appends a new diary entry", () => {
    const newEntry = diaryEntry("c", "Yogurt");
    const result = applyNutritionDiariesMealUpdate(base, {
      mealType: breakfast,
      newDiary: newEntry,
    });

    expect(result.diaries[breakfast]).toHaveLength(3);
    expect(result.diaries[breakfast][2]).toEqual(newEntry);
  });

  it("merges updates into an existing diary entry", () => {
    const result = applyNutritionDiariesMealUpdate(base, {
      mealType: breakfast,
      updatedDiary: { id: "b", name: "Scrambled eggs" },
    });

    expect(result.diaries[breakfast][1]).toMatchObject({
      id: "b",
      name: "Scrambled eggs",
    });
  });

  it("does not mutate unrelated meals", () => {
    const result = applyNutritionDiariesMealUpdate(base, {
      mealType: breakfast,
      deleteDiaryId: "a",
    });

    expect(result.diaries[MealTypes.Lunch]).toEqual([]);
  });
});
