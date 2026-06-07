import { describe, expect, it } from "vitest";

import type { FoodDto, IngredientDto, ServingSizeDto } from "@/services/openapi";

import { createEmptyNutritionalContent } from "../nutritionalContent";
import { getCalories } from "../ingredients";

function ingredient(
  multiplier: number,
  quantity: number,
  calories: number,
): IngredientDto {
  const nutrition = createEmptyNutritionalContent();
  nutrition.energy = { unit: "calories", value: calories };

  return {
    id: "ing-1",
    quantity,
    servingSize: {
      id: "ss-1",
      nutritionMultiplier: multiplier,
      isLoading: false,
      isDeleting: false,
    } as ServingSizeDto,
    food: {
      id: "food-1",
      name: "Oats",
      brandName: "Brand",
      type: "generic",
      countryCode: "US",
      servingSizes: [],
      nutritionalContents: nutrition,
      isLoading: false,
      isDeleting: false,
    } as FoodDto,
    isLoading: false,
    isDeleting: false,
  };
}

describe("getCalories", () => {
  it("multiplies serving size, quantity, and food energy", () => {
    expect(getCalories(ingredient(1, 2, 100))).toBe(200);
  });

  it("applies the nutrition multiplier", () => {
    expect(getCalories(ingredient(0.5, 2, 100))).toBe(100);
  });
});
