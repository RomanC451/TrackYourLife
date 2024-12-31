import { DialogDescription, DialogTitle } from "~/chadcn/ui/dialog";
import { Separator } from "~/chadcn/ui/separator";
import { AddRecipeDiaryFormSchema } from "~/features/health/data/schemas";
import useRecipeEntryForm from "~/features/health/hooks/foodDiaries/useRecipeEntryForm";
import { LoadingState } from "~/hooks/useDelayedLoading";
import { RecipeDto } from "~/services/openapi";
import MacrosDialogHeader from "../../common/MacrosDialogHeader";
import RecipeDiaryEntryForm from "../foodDiaryForms/RecipeDiaryEntryForm";

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
      <Separator />
      <MacrosDialogHeader
        nutritionalContents={recipe.nutritionalContents}
        nutritionMultiplier={formValues.nrOfServings}
      />
      <Separator />
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
