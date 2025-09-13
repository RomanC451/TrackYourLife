import { z } from "zod";

import { MealTypes } from "@/services/openapi";

export const recipeDiaryFormSchema = z.object({
  id: z.string().optional(),
  recipeId: z.string(),
  mealType: z.nativeEnum(MealTypes, { required_error: "Select a meal." }),
  servingSizeId: z.string(),
  quantity: z.number(),
  entryDate: z.string(),
});

export type RecipeDiaryFormSchema = z.infer<typeof recipeDiaryFormSchema>;
