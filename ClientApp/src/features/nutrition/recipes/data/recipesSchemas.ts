import z from "zod";

export const recipeDetailsSchema = z.object({
  name: z.string().min(1, { message: "Name is required" }),
  portions: z.number().min(1, { message: "Portions must be at least 1" }),
  weight: z.number().min(1, { message: "Weight must be at least 1" }),
});

export type RecipeDetailsSchema = z.infer<typeof recipeDetailsSchema>;
