import { useForm } from "react-hook-form";
import { z } from "zod";

import { zodResolver } from "@hookform/resolvers/zod";

export const foodDiaryFormSchema = z.object({
  nrOfServings: z.number().min(0.1, {
    message: "The number of servings can't be empty."
  }),
  servingSizeIndex: z.number(),
  mealType: z.string().min(1, { message: "Select a meal." })
});

export type TFoodDiaryFormSchema = z.infer<typeof foodDiaryFormSchema>;

const useFoodDiaryForm = (defaultValues?: TFoodDiaryFormSchema) => {
  const form = useForm<TFoodDiaryFormSchema>({
    resolver: zodResolver(foodDiaryFormSchema),
    defaultValues: defaultValues ?? {
      nrOfServings: 1,
      mealType: "",
      servingSizeIndex: 0
    }
  });

  return { form };
};

export default useFoodDiaryForm;
