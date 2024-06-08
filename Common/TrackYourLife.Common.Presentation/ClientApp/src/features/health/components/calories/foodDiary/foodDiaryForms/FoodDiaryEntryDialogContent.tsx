import React, { PropsWithChildren } from "react";
import { DialogTitle } from "~/chadcn/ui/dialog";
import { Separator } from "~/chadcn/ui/separator";

import useFoodDiaryForm, {
  TFoodDiaryFormSchema,
} from "../../../../hooks/useAddFoodDiaryForm";
import { FoodElement } from "../../../../requests/index.tsold";
import getMacrosData from "../../../../utils/getMacrosData";
import FoodDiaryEntryForm from "./FoodDiaryEntryForm";
import MacroOverview from "./MacroOverview";
import MacrosGraph from "./MacrosGraph";

type FoodDiaryEntryDialogContentProps = PropsWithChildren & {
  food: FoodElement;
  onSubmit(formData: TFoodDiaryFormSchema): void;
  isPending: boolean;
  defaultValues?: TFoodDiaryFormSchema;
  submitButtonText?: string;
};

const FoodDiaryEntryDialogContent: React.FC<
  FoodDiaryEntryDialogContentProps
> = ({ food, onSubmit, isPending, defaultValues, submitButtonText }) => {
  const { form } = useFoodDiaryForm(defaultValues);

  const formValues = form.watch();

  const nutritionMultiplier =
    food.servingSizes[formValues?.servingSizeIndex ?? 0].nutritionMultiplier *
    Number(formValues.nrOfServings);

  const macrosOverviewData = getMacrosData(food, nutritionMultiplier);

  const nutritionalPercentages = [
    macrosOverviewData.carbohydrates.percentage,
    macrosOverviewData.fat.percentage,
    macrosOverviewData.protein.percentage,
  ];

  return (
    <>
      <DialogTitle className="text-left">
        {food.name} {food.brandName ? "- " + food.brandName : ""}
      </DialogTitle>
      <Separator />
      <div className="flex items-center justify-between">
        <MacrosGraph
          food={food}
          nutritionMultiplier={nutritionMultiplier}
          nutritionalPercentages={nutritionalPercentages}
        />
        <MacroOverview {...macrosOverviewData.carbohydrates} />
        <MacroOverview {...macrosOverviewData.fat} />
        <MacroOverview {...macrosOverviewData.protein} />
      </div>
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
};

export default FoodDiaryEntryDialogContent;
