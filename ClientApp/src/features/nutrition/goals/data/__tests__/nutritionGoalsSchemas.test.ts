import { describe, expect, it } from "vitest";

import {
  ActivityLevel,
  FitnessGoal,
  Gender,
} from "@/services/openapi";

import {
  calculateNutritionGoalsFormSchema,
  updateNutritionGoalsFormSchema,
} from "../nutritionGoalsSchemas";

describe("calculateNutritionGoalsFormSchema", () => {
  const valid = {
    age: "30",
    weight: "75",
    height: "180",
    gender: Gender.Male,
    activityLevel: ActivityLevel.ModeratelyActive,
    fitnessGoal: FitnessGoal.Maintain,
  };

  it("parses string numeric fields into numbers", () => {
    const result = calculateNutritionGoalsFormSchema.safeParse(valid);

    expect(result.success).toBe(true);
    if (result.success) {
      expect(result.data.age).toBe(30);
      expect(result.data.weight).toBe(75);
      expect(result.data.height).toBe(180);
    }
  });

  it("requires all calculator fields", () => {
    const result = calculateNutritionGoalsFormSchema.safeParse({
      ...valid,
      age: "",
    });

    expect(result.success).toBe(false);
  });
});

describe("updateNutritionGoalsFormSchema", () => {
  it("accepts positive macro goals", () => {
    const result = updateNutritionGoalsFormSchema.safeParse({
      calories: 2000,
      proteins: 150,
      carbs: 200,
      fat: 70,
    });

    expect(result.success).toBe(true);
  });

  it("rejects zero or negative goals", () => {
    const result = updateNutritionGoalsFormSchema.safeParse({
      calories: 0,
      proteins: 150,
      carbs: 200,
      fat: 70,
    });

    expect(result.success).toBe(false);
  });
});
