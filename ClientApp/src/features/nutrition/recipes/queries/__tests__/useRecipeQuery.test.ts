import { beforeEach, describe, expect, it, vi } from "vitest";

const mockGetRecipesByUserId = vi.hoisted(() => vi.fn());
const mockGetRecipeById = vi.hoisted(() => vi.fn());

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockRecipesApi {
    getRecipesByUserId = mockGetRecipesByUserId;
    getRecipeById = mockGetRecipeById;
  }
  return { ...actual, RecipesApi: MockRecipesApi };
});

import * as recipesUtils from "@/features/nutrition/common/utils/recipes";
import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { queryClient } from "@/queryClient";
import { createQueryFnContext } from "@/test/queryFnContext";
import type {
  FoodDto,
  IngredientDto,
  RecipeDto,
  ServingSizeDto,
} from "@/services/openapi";

import {
  recipesQueryKeys,
  recipesQueryOptions,
  setRecipeQueryData,
} from "../useRecipeQuery";

function servingSize(id: string, multiplier = 1): ServingSizeDto {
  return {
    id,
    nutritionMultiplier: multiplier,
    isLoading: false,
    isDeleting: false,
  } as ServingSizeDto;
}

function food(name: string): FoodDto {
  const nutrition = createEmptyNutritionalContent();
  nutrition.protein = 10;
  nutrition.energy = { unit: "calories", value: 100 };

  return {
    id: `food-${name}`,
    name,
    brandName: "Brand",
    type: "generic",
    countryCode: "US",
    servingSizes: [],
    nutritionalContents: nutrition,
    isLoading: false,
    isDeleting: false,
  };
}

function ingredient(id: string, quantity: number): IngredientDto {
  return {
    id,
    food: food("Oats"),
    servingSize: servingSize("ss-1"),
    quantity,
    isLoading: false,
    isDeleting: false,
  };
}

function baseRecipe(): RecipeDto {
  return {
    id: "recipe-1",
    name: "Breakfast bowl",
    portions: 2,
    weight: 300,
    ingredients: [ingredient("ing-1", 1)],
    servingSizes: [],
    nutritionalContents: createEmptyNutritionalContent(),
    isLoading: false,
    isDeleting: false,
  };
}

describe("recipesQueryKeys", () => {
  it("builds recipe list and detail keys", () => {
    expect(recipesQueryKeys.all).toEqual(["recipes"]);
    expect(recipesQueryKeys.byId("recipe-1")).toEqual([
      "recipes",
      "recipe-1",
    ]);
  });
});

describe("setRecipeQueryData", () => {
  beforeEach(() => {
    queryClient.clear();
    queryClient.setQueryData(recipesQueryKeys.byId("recipe-1"), baseRecipe());
  });

  it("replaces the recipe when data is provided", () => {
    const replacement = { ...baseRecipe(), name: "Lunch bowl" };
    setRecipeQueryData({ recipeId: "recipe-1", data: replacement });

    expect(
      queryClient.getQueryData<RecipeDto>(recipesQueryKeys.byId("recipe-1"))
        ?.name,
    ).toBe("Lunch bowl");
  });

  it("updates name and portions", () => {
    setRecipeQueryData({
      recipeId: "recipe-1",
      name: "Updated bowl",
      portions: 4,
    });

    const cached = queryClient.getQueryData<RecipeDto>(
      recipesQueryKeys.byId("recipe-1"),
    );

    expect(cached?.name).toBe("Updated bowl");
    expect(cached?.portions).toBe(4);
  });

  it("updates an existing ingredient", () => {
    const updated = ingredient("ing-1", 3);
    setRecipeQueryData({
      recipeId: "recipe-1",
      updatedIngredient: updated,
    });

    const cached = queryClient.getQueryData<RecipeDto>(
      recipesQueryKeys.byId("recipe-1"),
    );

    expect(cached?.ingredients[0].quantity).toBe(3);
    expect(cached?.nutritionalContents.protein).toBe(30);
  });

  it("removes ingredients by id", () => {
    setRecipeQueryData({
      recipeId: "recipe-1",
      removedIngredientsIds: ["ing-1"],
    });

    const cached = queryClient.getQueryData<RecipeDto>(
      recipesQueryKeys.byId("recipe-1"),
    );

    expect(cached?.ingredients).toHaveLength(0);
  });

  it("delegates ingredient additions to addIngredientInRecipe", () => {
    const added = ingredient("ing-2", 2);
    const addIngredientSpy = vi.spyOn(recipesUtils, "addIngredientInRecipe");

    setRecipeQueryData({
      recipeId: "recipe-1",
      addedIngredient: added,
    });

    expect(addIngredientSpy).toHaveBeenCalledWith(
      expect.objectContaining({ id: "recipe-1" }),
      added,
    );
    addIngredientSpy.mockRestore();
  });
});

describe("recipesQueryOptions", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("fetches all recipes for the current user", async () => {
    const recipes = [baseRecipe()];
    mockGetRecipesByUserId.mockResolvedValue({ data: recipes });

    const result = await recipesQueryOptions.all.queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: recipesQueryKeys.all,
      }),
    );
    expect(result).toEqual(recipes);
    expect(mockGetRecipesByUserId).toHaveBeenCalled();
  });
});
