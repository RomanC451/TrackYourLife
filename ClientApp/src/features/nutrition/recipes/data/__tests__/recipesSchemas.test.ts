import { describe, expect, it } from "vitest";

import { recipeDetailsSchema } from "../recipesSchemas";

describe("recipeDetailsSchema", () => {
  it("accepts valid recipe details", () => {
    const result = recipeDetailsSchema.safeParse({
      name: "Overnight oats",
      portions: 2,
      weight: 400,
    });

    expect(result.success).toBe(true);
  });

  it("requires a non-empty name", () => {
    const result = recipeDetailsSchema.safeParse({
      name: "",
      portions: 2,
      weight: 400,
    });

    expect(result.success).toBe(false);
  });

  it("requires portions and weight of at least 1", () => {
    expect(
      recipeDetailsSchema.safeParse({
        name: "Soup",
        portions: 0,
        weight: 400,
      }).success,
    ).toBe(false);

    expect(
      recipeDetailsSchema.safeParse({
        name: "Soup",
        portions: 2,
        weight: 0,
      }).success,
    ).toBe(false);
  });
});
