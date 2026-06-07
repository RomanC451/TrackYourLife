import { describe, expect, it } from "vitest";

import { MealTypes } from "@/services/openapi";

import { foodDiaryFormSchema } from "../foodDiarySchemas";

describe("foodDiaryFormSchema", () => {
  const valid = {
    foodId: "food-1",
    mealType: MealTypes.Breakfast,
    servingSizeId: "ss-1",
    quantity: 1,
    entryDate: "2026-06-05",
  };

  it("accepts a valid food diary form", () => {
    expect(foodDiaryFormSchema.safeParse(valid).success).toBe(true);
  });

  it("treats an empty meal type as missing", () => {
    const result = foodDiaryFormSchema.safeParse({ ...valid, mealType: "" });

    expect(result.success).toBe(false);
  });

  it("rejects quantities below 0.1", () => {
    const result = foodDiaryFormSchema.safeParse({ ...valid, quantity: 0 });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toBe(
        "The quantity can't be empty.",
      );
    }
  });
});
