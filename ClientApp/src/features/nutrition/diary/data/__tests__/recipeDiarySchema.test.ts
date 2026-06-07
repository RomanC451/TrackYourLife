import { describe, expect, it } from "vitest";

import { MealTypes } from "@/services/openapi";

import { recipeDiaryFormSchema } from "../recipeDiarySchema";

describe("recipeDiaryFormSchema", () => {
  const valid = {
    recipeId: "recipe-1",
    mealType: MealTypes.Lunch,
    servingSizeId: "ss-1",
    quantity: 2,
    entryDate: "2026-06-05",
  };

  it("accepts a valid recipe diary form", () => {
    expect(recipeDiaryFormSchema.safeParse(valid).success).toBe(true);
  });

  it("treats an empty meal type as missing", () => {
    const result = recipeDiaryFormSchema.safeParse({ ...valid, mealType: "" });

    expect(result.success).toBe(false);
  });

  it("requires a recipe id", () => {
    const result = recipeDiaryFormSchema.safeParse({
      ...valid,
      recipeId: undefined,
    });

    expect(result.success).toBe(false);
  });
});
