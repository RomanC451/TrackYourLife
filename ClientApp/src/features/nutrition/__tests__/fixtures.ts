import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import type {
  FoodDto,
  GoalDto,
  IngredientDto,
  RecipeDto,
  ServingSizeDto,
} from "@/services/openapi";
import { GoalPeriod, GoalType } from "@/services/openapi";

export function servingSize(
  id: string,
  multiplier = 1,
  value = 100,
  unit = "g",
): ServingSizeDto {
  return {
    id,
    nutritionMultiplier: multiplier,
    value,
    unit,
    isLoading: false,
    isDeleting: false,
  } as ServingSizeDto;
}

export function food(
  id: string,
  name: string,
  brandName = "Brand",
): FoodDto {
  return {
    id,
    name,
    brandName,
    type: "generic",
    countryCode: "US",
    servingSizes: [servingSize("ss-1")],
    nutritionalContents: createEmptyNutritionalContent(),
    isLoading: false,
    isDeleting: false,
  };
}

export function ingredient(
  id: string,
  foodDto: FoodDto,
  quantity = 1,
): IngredientDto {
  return {
    id,
    food: foodDto,
    servingSize: foodDto.servingSizes[0],
    quantity,
    isLoading: false,
    isDeleting: false,
  };
}

export function recipe(
  id: string,
  name: string,
  overrides: Partial<RecipeDto> = {},
): RecipeDto {
  return {
    id,
    name,
    portions: 2,
    weight: 400,
    ingredients: [],
    servingSizes: [],
    nutritionalContents: createEmptyNutritionalContent(),
    isLoading: false,
    isDeleting: false,
    ...overrides,
  };
}

export function goal(type: GoalType, value: number): GoalDto {
  return {
    id: `${type}-goal`,
    type,
    value,
    period: GoalPeriod.Day,
    startDate: "2026-01-01",
    endDate: "2026-12-31",
    isLoading: false,
    isDeleting: false,
  };
}

export function macroGoals() {
  return {
    calories: goal(GoalType.Calories, 2000),
    proteins: goal(GoalType.Protein, 150),
    carbs: goal(GoalType.Carbohydrates, 220),
    fat: goal(GoalType.Fats, 70),
  };
}
