import { useToggle } from "usehooks-ts";
import { RecipeDto } from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import { AddRecipeDiaryFormSchema } from "../../data/schemas";
import useAddRecipeDiaryMutation from "../../mutations/foodDiaries/useAddRecipeDiaryMutation";

export default function useAddRecipeDiaryEntryDialog(
  recipe: RecipeDto,
  date: DateOnly,
  onSuccess?: () => void,
) {
  const [dialogState, toggleDialogState] = useToggle(false);

  const { addRecipeDiaryMutation, isPending } = useAddRecipeDiaryMutation();

  function onSubmit(formData: AddRecipeDiaryFormSchema) {
    addRecipeDiaryMutation.mutate(
      {
        recipeId: recipe.id,
        quantity: formData.nrOfServings,
        entryDate: date,
        mealType: formData.mealType,
        recipe: recipe,
      },
      {
        onSuccess: () => {
          onSuccess?.();
          toggleDialogState();
        },
      },
    );
  }

  return {
    dialogState,
    toggleDialogState,
    onSubmit,
    isPending,
  };
}
