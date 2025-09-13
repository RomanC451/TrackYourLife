import { z } from "zod";

export const ingredientSchema = z.object({
  quantity: z.number().min(0.01, { message: "Quantity can't be empty." }),
  servingSizeId: z.string().min(1, { message: "Serving size is required." }),
  foodId: z.string().min(1, { message: "Food is required." }),
});

export type IngredientSchema = z.infer<typeof ingredientSchema>;
