import { UseFormReturn } from "react-hook-form";

import LinearProgress from "@/components/linear-progress";
import { Button } from "@/components/ui/button";
import { Form } from "@/components/ui/form";
import { Separator } from "@/components/ui/separator";
import MealTypeFormField from "@/features/nutrition/common/components/formFields/MealTypeFormField";
import QuantityFormField from "@/features/nutrition/common/components/formFields/QuantityFormField";
import ServingSizeFormField from "@/features/nutrition/common/components/formFields/ServingSizeFormField";
import NutritionalInfoAccordion from "@/features/nutrition/common/components/NutritionalInfoAccordion";
import { PendingState } from "@/hooks/useCustomQuery";
import { RecipeDto } from "@/services/openapi";

import { RecipeDiaryFormSchema } from "../../data/recipeDiarySchema";

export function RecipeDiaryForm({
  form,
  handleCustomSubmit,
  submitButtonText,
  pendingState,
  recipe,
}: {
  form: UseFormReturn<RecipeDiaryFormSchema>;
  handleCustomSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  submitButtonText: string;
  pendingState: PendingState;
  recipe: RecipeDto;
}) {
  const servingSize = Object.values(recipe.servingSizes).find(
    (ss) => ss.id == form.getValues("servingSizeId"),
  )!;

  return (
    <>
      <Form {...form}>
        <form
          onSubmit={handleCustomSubmit}
          className="flex w-full flex-col gap-2"
        >
          <div className="flex flex-col gap-2 @sm/dialog:flex-row">
            <QuantityFormField name="quantity" label="Quantity" />
            <Separator className="@sm/dialog:hidden" />
            <ServingSizeFormField
              name="servingSizeId"
              servingSizes={recipe.servingSizes}
            />
          </div>
          <Separator />
          <div className="flex w-full justify-end">
            <MealTypeFormField className="w-full @sm/dialog:min-w-48" />
          </div>
          <Separator />
          <NutritionalInfoAccordion
            nutritionalContents={recipe.nutritionalContents}
            nutritionalMultiplier={servingSize.nutritionMultiplier}
          />

          <div className="flex w-full justify-end">
            <Button
              variant="secondary"
              type="submit"
              className=" "
              disabled={pendingState.isPending}
            >
              {submitButtonText}
            </Button>
          </div>
        </form>
      </Form>
      {pendingState.isDelayedPending ? (
        <LinearProgress color="inherit" />
      ) : null}
    </>
  );
}
