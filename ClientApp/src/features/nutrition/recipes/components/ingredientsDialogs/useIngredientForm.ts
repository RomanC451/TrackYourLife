import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";

import {
  addIngredientFormSchema,
  AddIngredientFormSchema,
} from "../../data/schemas";

function useIngredientForm(defaultValues?: AddIngredientFormSchema) {
  const form = useForm<AddIngredientFormSchema>({
    resolver: zodResolver(addIngredientFormSchema),
    defaultValues: defaultValues ?? {
      nrOfServings: 1,
      servingSizeIndex: 0,
    },
  });
  return { form };
}

export default useIngredientForm;
