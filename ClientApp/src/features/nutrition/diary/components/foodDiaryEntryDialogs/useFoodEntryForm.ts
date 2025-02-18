import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { useReadLocalStorage } from "usehooks-ts";

import { MealTypes } from "@/services/openapi";

import {
  addFoodDiaryFormSchema,
  AddFoodDiaryFormSchema,
} from "../../data/schemas";

const useFoodEntryForm = (defaultValues?: AddFoodDiaryFormSchema) => {
  const mealType = useReadLocalStorage<MealTypes>("lastMealType");

  const form = useForm<AddFoodDiaryFormSchema>({
    resolver: zodResolver(addFoodDiaryFormSchema),
    defaultValues: defaultValues ?? {
      nrOfServings: 1,
      servingSizeIndex: 0,
      mealType: mealType ?? undefined,
    },
  });

  return { form };
};

export default useFoodEntryForm;
