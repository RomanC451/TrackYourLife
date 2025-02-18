import { DialogDescription, DialogTitle } from "@/components/ui/dialog";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { LoadingState } from "@/hooks/useDelayedLoading";
import { FoodDto } from "@/services/openapi";

import MacrosDialogHeader from "../../../common/components/macros/MacrosDialogHeader";
import { AddFoodDiaryFormSchema } from "../../data/schemas";
import FoodDiaryEntryForm from "./FoodDiaryEntryForm";
import useFoodEntryForm from "./useFoodEntryForm";

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
      <Separator className="my-2" />
      <MacrosDialogHeader
        nutritionalContents={food.nutritionalContents}
        nutritionMultiplier={nutritionMultiplier}
      />
      <Separator className="my-2" />

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
