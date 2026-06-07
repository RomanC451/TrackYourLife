import { describe, expect, it } from "vitest";

import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";

import { emptyRecipe } from "../useRecipeDialog";

describe("emptyRecipe", () => {
  it("provides default values for a new recipe form", () => {
    expect(emptyRecipe).toEqual({
      name: "",
      portions: 1,
      weight: 100,
      isLoading: false,
      isDeleting: false,
      id: "",
      ingredients: [],
      nutritionalContents: createEmptyNutritionalContent(),
      servingSizes: [],
    });
  });
});
