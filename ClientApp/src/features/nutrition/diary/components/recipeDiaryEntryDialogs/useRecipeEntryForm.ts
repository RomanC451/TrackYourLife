import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { useReadLocalStorage } from "usehooks-ts";

import { MealTypes } from "@/services/openapi";

import {
  addRecipeDiaryFormSchema,
  AddRecipeDiaryFormSchema,
} from "../../data/schemas";

export default function useRecipeEntryForm(
  defaultValues?: AddRecipeDiaryFormSchema,
) {
  const mealType = useReadLocalStorage<MealTypes>("lastMealType");

  const form = useForm<AddRecipeDiaryFormSchema>({
    resolver: zodResolver(addRecipeDiaryFormSchema),
    defaultValues: defaultValues ?? {
      nrOfServings: 1,
      mealType: mealType ?? undefined,
    },
  });

  return { form };
}
