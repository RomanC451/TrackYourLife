import { useForm } from "react-hook-form";

import { zodResolver } from "@hookform/resolvers/zod";
import {
  addFoodDiaryFormSchema,
  AddFoodDiaryFormSchema,
} from "../../data/schemas";

const useFoodEntryForm = (defaultValues?: AddFoodDiaryFormSchema) => {
  const form = useForm<AddFoodDiaryFormSchema>({
    resolver: zodResolver(addFoodDiaryFormSchema),
    defaultValues: defaultValues ?? {
      nrOfServings: 1,
      servingSizeIndex: 0,
    },
  });

  return { form };
};

export default useFoodEntryForm;
