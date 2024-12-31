import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import {
  addRecipeDiaryFormSchema,
  AddRecipeDiaryFormSchema,
} from "../../data/schemas";

export default function useRecipeEntryForm(
  defaultValues?: AddRecipeDiaryFormSchema,
) {
  const form = useForm<AddRecipeDiaryFormSchema>({
    resolver: zodResolver(addRecipeDiaryFormSchema),
    defaultValues: defaultValues ?? {
      nrOfServings: 1,
    },
  });

  return { form };
}
