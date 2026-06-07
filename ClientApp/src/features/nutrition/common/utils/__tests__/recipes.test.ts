import { describe, expect, it } from "vitest";

import type {
  FoodDto,
  IngredientDto,
  NutritionalContent,
  RecipeDto,
  ServingSizeDto,
} from "@/services/openapi";

import { createEmptyNutritionalContent } from "../nutritionalContent";
import {
  addIngredientInRecipe,
  removeIngredientFromRecipe,
  updateIngredientInRecipe,
} from "../recipes";

function servingSize(id: string, multiplier = 1): ServingSizeDto {
  return {
    id,
    nutritionMultiplier: multiplier,
    isLoading: false,
    isDeleting: false,
  } as ServingSizeDto;
}

function food(
  name: string,
  brandName: string,
  nutritionalContents: NutritionalContent,
): FoodDto {
  return {
    id: `food-${name}`,
    name,
    brandName,
    type: "generic",
    countryCode: "US",
    servingSizes: [],
    nutritionalContents,
    isLoading: false,
    isDeleting: false,
  };
}

function ingredient(
  id: string,
  foodDto: FoodDto,
  serving: ServingSizeDto,
  quantity: number,
): IngredientDto {
  return {
    id,
    food: foodDto,
    servingSize: serving,
    quantity,
    isLoading: false,
    isDeleting: false,
  };
}

function recipe(
  ingredients: IngredientDto[],
  nutritionalContents = createEmptyNutritionalContent(),
): RecipeDto {
  return {
    id: "recipe-1",
    name: "Test recipe",
    portions: 2,
    weight: 500,
    ingredients,
    servingSizes: [],
    nutritionalContents,
    isLoading: false,
    isDeleting: false,
  };
}

const oatsNutrition = createEmptyNutritionalContent();
oatsNutrition.protein = 10;
oatsNutrition.carbohydrates = 50;
oatsNutrition.energy = { unit: "calories", value: 200 };

const eggNutrition = createEmptyNutritionalContent();
eggNutrition.protein = 6;
eggNutrition.fat = 5;
eggNutrition.energy = { unit: "calories", value: 70 };

describe("addIngredientInRecipe", () => {
  it("appends a new ingredient and updates nutritional totals", () => {
    const oats = food("Oats", "BrandA", oatsNutrition);
    const added = ingredient("ing-1", oats, servingSize("ss-1", 1), 2);

    const result = addIngredientInRecipe(recipe([]), added);

    expect(result?.ingredients).toHaveLength(1);
    expect(result?.nutritionalContents.protein).toBe(20);
    expect(result?.nutritionalContents.energy.value).toBe(400);
    expect(result?.isLoading).toBe(true);
  });

  it("merges quantity when the same food and serving size already exists", () => {
    const oats = food("Oats", "BrandA", oatsNutrition);
    const existing = ingredient("ing-1", oats, servingSize("ss-1", 1), 1);
    const added = ingredient("ing-2", oats, servingSize("ss-1", 1), 2);
    const base = addIngredientInRecipe(recipe([]), existing)!;

    const result = addIngredientInRecipe(base, added);

    expect(result?.ingredients).toHaveLength(1);
    expect(result?.ingredients[0].quantity).toBe(3);
    expect(result?.ingredients[0].id).toBe("ing-1");
    expect(result?.nutritionalContents.protein).toBe(30);
  });

  it("returns undefined when the same food has a different serving size", () => {
    const oats = food("Oats", "BrandA", oatsNutrition);
    const existing = ingredient("ing-1", oats, servingSize("ss-1", 1), 1);
    const added = ingredient("ing-2", oats, servingSize("ss-2", 0.5), 1);

    const result = addIngredientInRecipe(recipe([existing]), added);

    expect(result).toBeUndefined();
  });
});

describe("updateIngredientInRecipe", () => {
  it("replaces nutrition for the matching ingredient", () => {
    const oats = food("Oats", "BrandA", oatsNutrition);
    const eggs = food("Eggs", "BrandB", eggNutrition);
    const existingOats = ingredient("ing-1", oats, servingSize("ss-1", 1), 1);
    const existingEggs = ingredient("ing-2", eggs, servingSize("ss-2", 1), 1);
    const base = addIngredientInRecipe(
      addIngredientInRecipe(recipe([]), existingOats)!,
      existingEggs,
    )!;

    const updatedOats = ingredient("ing-1", oats, servingSize("ss-1", 1), 3);
    const result = updateIngredientInRecipe(base, updatedOats);

    expect(result.ingredients[0].quantity).toBe(3);
    expect(result.nutritionalContents.protein).toBe(36);
    expect(result.nutritionalContents.energy.value).toBe(670);
  });
});

describe("removeIngredientFromRecipe", () => {
  it("removes ingredients and subtracts their nutrition", () => {
    const oats = food("Oats", "BrandA", oatsNutrition);
    const eggs = food("Eggs", "BrandB", eggNutrition);
    const existingOats = ingredient("ing-1", oats, servingSize("ss-1", 1), 2);
    const existingEggs = ingredient("ing-2", eggs, servingSize("ss-2", 1), 1);
    const base = addIngredientInRecipe(
      addIngredientInRecipe(recipe([]), existingOats)!,
      existingEggs,
    )!;

    const result = removeIngredientFromRecipe(base, ["ing-1"]);

    expect(result.ingredients).toHaveLength(1);
    expect(result.ingredients[0].id).toBe("ing-2");
    expect(result.nutritionalContents.protein).toBe(6);
    expect(result.nutritionalContents.energy.value).toBe(70);
  });
});
