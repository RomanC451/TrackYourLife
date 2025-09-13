import { UseFormReturn } from "react-hook-form";

import { Button } from "@/components/ui/button";
import { Form } from "@/components/ui/form";
import { Separator } from "@/components/ui/separator";
import MealTypeFormField from "@/features/nutrition/common/components/formFields/MealTypeFormField";
import QuantityFormField from "@/features/nutrition/common/components/formFields/QuantityFormField";
import ServingSizeFormField from "@/features/nutrition/common/components/formFields/ServingSizeFormField";
import NutritionalInfoAccordion from "@/features/nutrition/common/components/NutritionalInfoAccordion";
import { PendingState } from "@/hooks/useCustomQuery";
import { FoodDto } from "@/services/openapi";

import { FoodDiaryFormSchema } from "../../data/foodDiarySchemas";

function FoodDiaryForm({
  form,
  handleCustomSubmit,
  submitButtonText,
  pendingState,
  food,
}: {
  form: UseFormReturn<FoodDiaryFormSchema>;
  handleCustomSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  submitButtonText: string;
  pendingState: PendingState;
  food: FoodDto;
}) {
  const servingSize = Object.values(food.servingSizes).find(
    (ss) => ss.id == form.getValues("servingSizeId"),
  )!;

  return (
    <Form {...form}>
      <form onSubmit={handleCustomSubmit} className="space-y-4 pt-2">
        <div className="flex flex-col gap-2 @sm/dialog:flex-row">
          <QuantityFormField name="quantity" label="Quantity" />
          <Separator className="@sm/dialog:hidden" />
          <ServingSizeFormField
            name="servingSizeId"
            servingSizes={food.servingSizes}
          />
        </div>
        <Separator />
        <div className="flex w-full justify-end">
          <MealTypeFormField className="w-full min-w-[130px] sm:w-[50%]" />
        </div>
        <Separator />
        <NutritionalInfoAccordion
          nutritionalContents={food.nutritionalContents}
          nutritionalMultiplier={servingSize.nutritionMultiplier}
        />

        <div className="flex w-[100%] justify-end">
          <Button
            type="submit"
            variant="default"
            disabled={!!pendingState.isPending}
            className=""
          >
            {submitButtonText}
          </Button>
        </div>
      </form>
    </Form>
  );
}

export default FoodDiaryForm;
