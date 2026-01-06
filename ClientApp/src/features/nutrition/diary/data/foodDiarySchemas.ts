import { z } from "zod";

import { MealTypes } from "@/services/openapi";

export const foodDiaryFormSchema = z.object({
  id: z.string().optional(),
  foodId: z.string(),
  mealType: z.preprocess(
    (val) => (val === "" ? undefined : val),
    z.nativeEnum(MealTypes, { required_error: "Select a meal." }),
  ),
  servingSizeId: z.string(),
  quantity: z.number().min(0.1, {
    message: "The quantity can't be empty.",
  }),
  entryDate: z.string(),
});

export type FoodDiaryFormSchema = z.infer<typeof foodDiaryFormSchema>;
