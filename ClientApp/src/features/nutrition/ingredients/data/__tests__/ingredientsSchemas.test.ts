import { describe, expect, it } from "vitest";

import { ingredientSchema } from "../ingredientsSchemas";

describe("ingredientSchema", () => {
  it("accepts a valid ingredient form", () => {
    const result = ingredientSchema.safeParse({
      quantity: 1,
      servingSizeId: "ss-1",
      foodId: "food-1",
    });

    expect(result.success).toBe(true);
  });

  it("rejects quantities below 0.01", () => {
    const result = ingredientSchema.safeParse({
      quantity: 0,
      servingSizeId: "ss-1",
      foodId: "food-1",
    });

    expect(result.success).toBe(false);
  });

  it("requires food and serving size ids", () => {
    const result = ingredientSchema.safeParse({
      quantity: 1,
      servingSizeId: "",
      foodId: "",
    });

    expect(result.success).toBe(false);
  });
});
