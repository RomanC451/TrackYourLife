import { act, renderHook } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import type { IngredientDto } from "@/services/openapi";

import { useIngredientsSelection } from "../useIngredientsSelection";

function ingredient(id: string): IngredientDto {
  return {
    id,
    quantity: 1,
    servingSize: {
      id: "ss-1",
      value: 100,
      unit: "g",
      nutritionMultiplier: 1,
      isLoading: false,
      isDeleting: false,
    },
    food: {
      id: `food-${id}`,
      name: "Oats",
      brandName: "Brand",
      type: "generic",
      countryCode: "US",
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
    },
    isLoading: false,
    isDeleting: false,
  };
}

const ingredients = [ingredient("ing-1"), ingredient("ing-2")];

describe("useIngredientsSelection", () => {
  it("starts with no selected ingredients", () => {
    const { result } = renderHook(() => useIngredientsSelection(ingredients));

    expect(result.current.selectedIds).toEqual([]);
    expect(result.current.isAllSelected).toBe(false);
  });

  it("toggles ingredient selection", () => {
    const { result } = renderHook(() => useIngredientsSelection(ingredients));

    act(() => result.current.toggle("ing-1"));
    expect(result.current.selectedIds).toEqual(["ing-1"]);

    act(() => result.current.toggle("ing-1"));
    expect(result.current.selectedIds).toEqual([]);
  });

  it("selects and clears all ingredients", () => {
    const { result } = renderHook(() => useIngredientsSelection(ingredients));

    act(() => result.current.handleSelectAll(true));
    expect(result.current.selectedIds).toEqual(["ing-1", "ing-2"]);
    expect(result.current.isAllSelected).toBe(true);

    act(() => result.current.clearSelection());
    expect(result.current.selectedIds).toEqual([]);
    expect(result.current.isAllSelected).toBe(false);
  });

  it("returns selected ingredient objects", () => {
    const { result } = renderHook(() => useIngredientsSelection(ingredients));

    act(() => result.current.toggle("ing-2"));

    expect(result.current.selectedIngredients.map((entry) => entry.id)).toEqual([
      "ing-2",
    ]);
  });

  it("reports not all selected when the ingredient list is empty", () => {
    const { result } = renderHook(() => useIngredientsSelection([]));

    expect(result.current.isAllSelected).toBe(false);
  });

  it("clears the selection when select all is turned off", () => {
    const { result } = renderHook(() => useIngredientsSelection(ingredients));

    act(() => result.current.handleSelectAll(true));
    expect(result.current.selectedIds).toEqual(["ing-1", "ing-2"]);

    act(() => result.current.handleSelectAll(false));
    expect(result.current.selectedIds).toEqual([]);
  });
});
