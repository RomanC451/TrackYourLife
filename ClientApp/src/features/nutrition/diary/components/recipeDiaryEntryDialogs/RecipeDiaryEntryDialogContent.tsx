import { DialogDescription, DialogTitle } from "@/components/ui/dialog";
import { Separator } from "@/components/ui/separator";
import MacrosDialogHeader from "@/features/nutrition/common/components/macros/MacrosDialogHeader";
import { LoadingState } from "@/hooks/useDelayedLoading";
import { RecipeDto } from "@/services/openapi";

import { AddRecipeDiaryFormSchema } from "../../data/schemas";
import RecipeDiaryEntryForm from "./RecipeDiaryEntryForm";
import useRecipeEntryForm from "./useRecipeEntryForm";

type RecipeDiaryEntryDialogContentProps = {
  recipe: RecipeDto;
  onSubmit: (formData: AddRecipeDiaryFormSchema) => void;
  isPending: LoadingState;
  defaultValues?: AddRecipeDiaryFormSchema;
  submitButtonText: string;
};

function RecipeDiaryEntryDialogContent({
  recipe,
  onSubmit,
  isPending,
  defaultValues,
  submitButtonText,
}: RecipeDiaryEntryDialogContentProps): JSX.Element {
  const { form } = useRecipeEntryForm(defaultValues);

  const formValues = form.watch();

  return (
    <>
      <DialogTitle className="text-left">{recipe.name}</DialogTitle>
      <DialogDescription hidden>Edit recipe diary entry</DialogDescription>
      <Separator className="my-2" />
      <MacrosDialogHeader
        nutritionalContents={recipe.nutritionalContents}
        nutritionMultiplier={formValues.nrOfServings}
      />
      <Separator className="my-2" />
      <RecipeDiaryEntryForm
        recipe={recipe}
        form={form}
        onSubmit={onSubmit}
        isPending={isPending}
        submitButtonText={submitButtonText}
      />
    </>
  );
}

export default RecipeDiaryEntryDialogContent;
