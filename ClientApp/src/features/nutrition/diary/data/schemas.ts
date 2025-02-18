import { z } from "zod";

import {
  ActivityLevel,
  FitnessGoal,
  Gender,
  MealTypes,
} from "@/services/openapi";

export const addFoodDiaryFormSchema = z.object({
  nrOfServings: z.number().min(0.1, {
    message: "The number of servings can't be empty.",
  }),
  servingSizeIndex: z.number(),
  mealType: z.nativeEnum(MealTypes, { required_error: "Select a meal." }),
});

export type AddFoodDiaryFormSchema = z.infer<typeof addFoodDiaryFormSchema>;

export const addRecipeDiaryFormSchema = z.object({
  nrOfServings: z.number().min(0.1, {
    message: "The number of servings can't be empty.",
  }),
  mealType: z.nativeEnum(MealTypes, { required_error: "Select a meal." }),
});

export type AddRecipeDiaryFormSchema = z.infer<typeof addRecipeDiaryFormSchema>;

export const createRecipeFormSchema = z.object({
  name: z.string().min(1, { message: "Name is required" }),
});

export type CreateRecipeFormSchema = z.infer<typeof createRecipeFormSchema>;

export const addIngredientFormSchema = z.object({
  nrOfServings: z
    .number()
    .min(0.1, { message: "The number of servings can't be empty." }),
  servingSizeIndex: z.number(),
});

export type AddIngredientFormSchema = z.infer<typeof addIngredientFormSchema>;

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
