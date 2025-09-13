import { UseFormReturn } from "react-hook-form";

import { Badge } from "@/components/ui/badge";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Form } from "@/components/ui/form";
import { MutationPendingState } from "@/hooks/useCustomMutation";
import { FoodDto } from "@/services/openapi";

import QuantityFormField from "../../../common/components/formFields/QuantityFormField";
import ServingSizeFormField from "../../../common/components/formFields/ServingSizeFormField";
import MacrosDialogHeader from "../../../common/components/macros/MacrosDialogHeader";
import NutritionalInfoAccordion from "../../../common/components/NutritionalInfoAccordion";
import { IngredientSchema } from "../../data/ingredientsSchemas";

function IngredientForm({
  form,
  food,
  handleCustomSubmit,
  pendingState,
  submitButtonText,
}: {
  form: UseFormReturn<IngredientSchema>;
  food: FoodDto;
  handleCustomSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  pendingState: MutationPendingState;
  submitButtonText: string;
}) {
  const formValues = form.watch();

  const servingSize = Object.values(food.servingSizes).find(
    (servingSize) => servingSize.id === formValues.servingSizeId,
  );

  const nutritionalMultiplier =
    (servingSize?.nutritionMultiplier ?? 0) * Number(formValues.quantity);

  return (
    <div className="space-y-4">
      <Badge className="text-sm">
        {food.name} - {food.brandName}
      </Badge>
      <MacrosDialogHeader
        nutritionalContents={food.nutritionalContents}
        nutritionMultiplier={nutritionalMultiplier}
      />
      <Form {...form}>
        <form
          onSubmit={handleCustomSubmit}
          className="flex w-full flex-col gap-2"
        >
          <div className="grid grid-cols-3 gap-4">
            <ServingSizeFormField<IngredientSchema>
              servingSizes={food.servingSizes}
              name="servingSizeId"
              className="col-span-3"
            />
            <QuantityFormField<IngredientSchema>
              name="quantity"
              label="Quantity"
              className="col-span-3 @sm/dialog:col-span-2"
            />
          </div>

          <NutritionalInfoAccordion
            nutritionalContents={food.nutritionalContents}
            nutritionalMultiplier={nutritionalMultiplier}
          />

          <div className="flex w-[100%] justify-end">
            <ButtonWithLoading
              type="submit"
              variant="default"
              disabled={pendingState.isPending}
              className=" "
              isLoading={pendingState.isPending}
            >
              {submitButtonText}
            </ButtonWithLoading>
          </div>
        </form>
      </Form>
    </div>
  );
}

export default IngredientForm;
