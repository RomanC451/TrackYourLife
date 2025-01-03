import { z } from "zod";

import { MealTypes } from "@/services/openapi";

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
