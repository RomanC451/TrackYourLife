import { z } from "zod";

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
