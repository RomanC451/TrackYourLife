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
