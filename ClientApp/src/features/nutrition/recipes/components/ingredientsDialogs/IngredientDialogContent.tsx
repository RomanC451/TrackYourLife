import { DialogDescription, DialogTitle } from "@/components/ui/dialog";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import MacrosDialogHeader from "@/features/nutrition/common/components/macros/MacrosDialogHeader";
import { LoadingState } from "@/hooks/useDelayedLoading";
import { FoodDto } from "@/services/openapi";

import { AddIngredientFormSchema } from "../../data/schemas";
import IngredientForm from "../ingredientsForms/IngredientForm";
import useIngredientForm from "./useIngredientForm";

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
}: IngredientDialogContentProps) {
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
