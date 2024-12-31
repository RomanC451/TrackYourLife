import { DialogDescription, DialogTitle } from "~/chadcn/ui/dialog";
import { Separator } from "~/chadcn/ui/separator";

import { Skeleton } from "~/chadcn/ui/skeleton";
import { AddFoodDiaryFormSchema } from "~/features/health/data/schemas";
import { LoadingState } from "~/hooks/useDelayedLoading";
import { FoodDto } from "~/services/openapi";
import useFoodEntryForm from "../../../hooks/foodDiaries/useFoodEntryForm";
import MacrosDialogHeader from "../../common/MacrosDialogHeader";
import FoodDiaryEntryForm from "../foodDiaryForms/FoodDiaryEntryForm";

type FoodDiaryEntryDialogContentProps = {
  food: FoodDto;
  onSubmit: (formData: AddFoodDiaryFormSchema) => void;
  isPending: LoadingState;
  defaultValues?: AddFoodDiaryFormSchema;
  submitButtonText?: string;
};

function FoodDiaryEntryDialogContent({
  food,
  onSubmit,
  isPending,
  defaultValues,
  submitButtonText,
}: FoodDiaryEntryDialogContentProps) {
  const { form } = useFoodEntryForm(defaultValues);

  const formValues = form.watch();

  const nutritionMultiplier =
    food.servingSizes[formValues?.servingSizeIndex ?? 0].nutritionMultiplier *
    Number(formValues.nrOfServings);

  return (
    <>
      <DialogDescription hidden>Edit food diary entry</DialogDescription>
      <DialogTitle className="text-left">
        {food.name} {food.brandName ? "- " + food.brandName : ""}
      </DialogTitle>
      <Separator />
      <MacrosDialogHeader
        nutritionalContents={food.nutritionalContents}
        nutritionMultiplier={nutritionMultiplier}
      />
      <Separator />
      <FoodDiaryEntryForm
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

FoodDiaryEntryDialogContent.Loading = function () {
  return (
    <>
      <DialogTitle>
        <Skeleton className="h-[18px] w-[250px]" />
      </DialogTitle>
      <Separator />
      <MacrosDialogHeader.Loading />
      <Separator />
      <FoodDiaryEntryForm.Loading />
    </>
  );
};

export default FoodDiaryEntryDialogContent;
