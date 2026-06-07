import { describe, expect, it } from "vitest";

import { ingredientsApiErrors } from "../ingredientsApiErrors";

describe("ingredientsApiErrors", () => {
  it("exposes the different serving size validation code", () => {
    expect(ingredientsApiErrors.Ingredient.DifferentServingSize).toBe(
      "Ingredient.DifferentServingSize",
    );
  });
});
