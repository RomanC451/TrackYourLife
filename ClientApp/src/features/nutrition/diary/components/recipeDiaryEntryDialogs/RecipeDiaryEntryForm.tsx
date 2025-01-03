import { LinearProgress } from "@mui/material";
import { UseFormReturn } from "react-hook-form";

import { Button } from "@/components/ui/button";
import { Form } from "@/components/ui/form";
import { Separator } from "@/components/ui/separator";
import MealTypeFormField from "@/features/nutrition/common/components/formFields/MealTypeFormField";
import ServingsFormField from "@/features/nutrition/common/components/formFields/ServingsFormField";
import NutritionalInfoAccordion from "@/features/nutrition/common/components/NutritionalInfoAccordion";
import { LoadingState } from "@/hooks/useDelayedLoading";
import { MealTypes, RecipeDto } from "@/services/openapi";

import { AddRecipeDiaryFormSchema } from "../../data/schemas";

type RecipeDiaryEntryFormProps = {
  recipe: RecipeDto;
  form: UseFormReturn<
    {
      nrOfServings: number;
      mealType: MealTypes;
    },
    unknown,
    undefined
  >;
  onSubmit(formData: AddRecipeDiaryFormSchema): void;
  isPending: LoadingState;
  submitButtonText: string;
};

function RecipeDiaryEntryForm({
  recipe,
  form,
  onSubmit,
  isPending,
  submitButtonText,
}: RecipeDiaryEntryFormProps): JSX.Element {
  const formValues = form.watch();

  return (
    <>
      <Form {...form}>
        <form
          onSubmit={form.handleSubmit(onSubmit)}
          className="flex w-full flex-col gap-2"
        >
          <ServingsFormField control={form.control} />
          <Separator />
          <div className="flex w-full justify-end">
            <MealTypeFormField
              control={form.control}
              className="w-full min-w-[130px] sm:w-[50%]"
            />
          </div>
          <Separator />
          <NutritionalInfoAccordion
            nutritionalContents={recipe.nutritionalContents}
            nutritionalMultiplier={formValues.nrOfServings}
          />

          <div className="flex w-[100%] justify-end">
            <Button
              variant="secondary"
              disabled={!isPending.isLoaded}
              type="submit"
              className=" "
            >
              {submitButtonText}
            </Button>
          </div>
        </form>
      </Form>
      {isPending.isLoading ? <LinearProgress color="inherit" /> : null}
    </>
  );
}

export default RecipeDiaryEntryForm;
