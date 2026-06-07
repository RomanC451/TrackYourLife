import { beforeEach, describe, expect, it, vi } from "vitest";

import { queryClient } from "@/queryClient";
import type { RecipeDto } from "@/services/openapi";

import { recipesQueryKeys } from "../../../recipes/queries/useRecipeQuery";
import { getRecipesQueryData, setRecipesQueryData } from "../useRecipesQuery";

function recipe(id: string, name: string): RecipeDto {
  return {
    id,
    name,
    portions: 1,
    weight: 100,
    ingredients: [],
    servingSizes: [],
    nutritionalContents: {
      calcium: 0,
      carbohydrates: 0,
      cholesterol: 0,
      fat: 0,
      fiber: 0,
      iron: 0,
      monounsaturatedFat: 0,
      netCarbs: 0,
      polyunsaturatedFat: 0,
      potassium: 0,
      protein: 0,
      saturatedFat: 0,
      sodium: 0,
      sugar: 0,
      transFat: 0,
      vitaminA: 0,
      vitaminC: 0,
      energy: { unit: "calories", value: 0 },
    },
    isLoading: false,
    isDeleting: false,
  };
}

describe("setRecipesQueryData", () => {
  beforeEach(() => {
    queryClient.clear();
    queryClient.setQueryData(recipesQueryKeys.all, [
      recipe("r-1", "Oats"),
      recipe("r-2", "Soup"),
    ]);
  });

  it("replaces the full recipes list when data is provided", () => {
    setRecipesQueryData({ data: [recipe("r-3", "Salad")] });

    expect(getRecipesQueryData()).toEqual([recipe("r-3", "Salad")]);
  });

  it("filters recipes", () => {
    setRecipesQueryData({ filter: (entry) => entry.id !== "r-1" });

    expect(getRecipesQueryData()?.map((entry) => entry.id)).toEqual([
      "r-2",
    ]);
  });

  it("appends a new recipe", () => {
    setRecipesQueryData({ newRecipe: recipe("r-3", "Salad") });

    expect(getRecipesQueryData()?.map((entry) => entry.id)).toEqual([
      "r-1",
      "r-2",
      "r-3",
    ]);
  });

  it("merges updates into an existing recipe", () => {
    setRecipesQueryData({
      updatedRecipe: { id: "r-2", name: "Hearty soup" },
    });

    expect(getRecipesQueryData()?.[1].name).toBe("Hearty soup");
  });

  it("invalidates the recipes query when requested", () => {
    const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries");

    setRecipesQueryData({ invalidate: true });

    expect(invalidateSpy).toHaveBeenCalled();
    invalidateSpy.mockRestore();
  });
});
