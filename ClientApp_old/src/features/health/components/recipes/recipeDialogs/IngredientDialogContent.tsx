import { DialogDescription, DialogTitle } from "~/chadcn/ui/dialog";
import { Separator } from "~/chadcn/ui/separator";
import { Skeleton } from "~/chadcn/ui/skeleton";
import { AddIngredientFormSchema } from "~/features/health/data/schemas";
import useIngredientForm from "~/features/health/hooks/recipes/useIngredientForm";
import { LoadingState } from "~/hooks/useDelayedLoading";
import { FoodDto } from "~/services/openapi";
import MacrosDialogHeader from "../../common/MacrosDialogHeader";
import IngredientForm from "../recipeForms/IngredientForm";

type IngredientDialogContentProps = {
  food: FoodDto;
  onSubmit: (formData: AddIngredientFormSchema) => void;
  isPending: LoadingState;
  defaultValues?: AddIngredientFormSchema;
  submitButtonText?: string;
};

function IngredientDialogContent({
  food,
  onSubmit,
  isPending,
  defaultValues,
  submitButtonText,
}: IngredientDialogContentProps): JSX.Element {
  const { form } = useIngredientForm(defaultValues);

  const formValues = form.watch();

  const nutritionMultiplier =
    food.servingSizes[formValues?.servingSizeIndex ?? 0].nutritionMultiplier *
    Number(formValues.nrOfServings);

  return (
    <>
      <DialogTitle className="text-left">
        {food.name} {food.brandName ? "- " + food.brandName : ""}
      </DialogTitle>
      <DialogDescription hidden>Edit ingredient</DialogDescription>
      <Separator />
      <MacrosDialogHeader
        nutritionalContents={food.nutritionalContents}
        nutritionMultiplier={nutritionMultiplier}
      />
      <Separator />
      <IngredientForm
        food={food}
        form={form}
        nutritionalMultiplier={nutritionMultiplier}
        onSubmit={onSubmit}
        isPending={isPending}
        submitButtonText={submitButtonText}
      />
    </>
  );
}

IngredientDialogContent.Loading = function () {
  return (
    <>
      <DialogTitle>
        <Skeleton className="h-[18px] w-[250px]" />
      </DialogTitle>
      <Separator />
      <MacrosDialogHeader.Loading />
      <Separator />
      <IngredientForm.Loading />
    </>
  );
};

export default IngredientDialogContent;
