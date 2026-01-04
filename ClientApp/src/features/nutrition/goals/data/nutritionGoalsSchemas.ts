import z from "zod";

import { ActivityLevel, FitnessGoal, Gender } from "@/services/openapi";

export const calculateNutritionGoalsFormSchema = z.object({
  age: z.string().min(1, "Age is required").transform(Number),
  weight: z.string().min(1, "Weight is required").transform(Number),
  height: z.string().min(1, "Height is required").transform(Number),
  gender: z.nativeEnum(Gender, { required_error: "Please select a gender" }),
  activityLevel: z.nativeEnum(ActivityLevel, {
    required_error: "Please select an activity level",
  }),
  fitnessGoal: z.nativeEnum(FitnessGoal, {
    required_error: "Please select a goal",
  }),
});

export type CalculateNutritionGoalsFormSchema = z.infer<
  typeof calculateNutritionGoalsFormSchema
>;

export const updateNutritionGoalsFormSchema = z.object({
  calories: z.number().min(1, "Calories must be greater than 0"),
  proteins: z.number().min(1, "Protein must be greater than 0"),
  carbs: z.number().min(1, "Carbohydrates must be greater than 0"),
  fat: z.number().min(1, "Fats must be greater than 0"),
});

export type UpdateNutritionGoalsFormSchema = z.infer<
  typeof updateNutritionGoalsFormSchema
>;
