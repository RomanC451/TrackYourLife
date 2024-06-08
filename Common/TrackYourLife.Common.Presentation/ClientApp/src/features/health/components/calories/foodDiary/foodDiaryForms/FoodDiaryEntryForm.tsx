import React from "react";
import { UseFormReturn } from "react-hook-form";
import { Button } from "~/chadcn/ui/button";
import { Form } from "~/chadcn/ui/form";
import { Separator } from "~/chadcn/ui/separator";
import { screensEnum } from "~/constants/tailwindSizes";
import { useAppGeneralStateContext } from "~/contexts/AppGeneralContextProvider";

import { LinearProgress } from "@mui/material";

import { TFoodDiaryFormSchema } from "../../../../hooks/useAddFoodDiaryForm";
import { FoodElement } from "../../../../requests/getFoodListRequest";
import MealTypeFormField from "./MealTypeFormField";
import NutritionalInfoAccordion from "./NutritionalInfoAccordion";
import ServingsFormField from "./ServingsFormField";
import ServingSizeFormField from "./ServingSizeFormField";

type AddFoodDiaryEntryFormProps = {
  food: FoodElement;
  form: UseFormReturn<
    {
      nrOfServings: number;
      servingSizeIndex: number;
      mealType: string;
    },
    unknown,
    {
      nrOfServings: number;
      servingSizeIndex: number;
      mealType: string;
    }
  >;
  nutritionalMultiplier: number;
  onSubmit(formData: TFoodDiaryFormSchema): void;
  isPending: boolean;
  submitButtonText?: string;
};

const FoodDiaryEntryForm: React.FC<AddFoodDiaryEntryFormProps> = ({
  food,
  form,
  nutritionalMultiplier,
  onSubmit,
  isPending,
  submitButtonText,
}) => {
  const { screenSize } = useAppGeneralStateContext();

  return (
    <>
      <Form {...form}>
        <form
          onSubmit={form.handleSubmit(onSubmit)}
          className="flex w-full flex-col gap-2"
        >
          {screenSize.width >= screensEnum.sm ? (
            <div className="flex gap-3">
              <ServingsFormField form={form} />
              <ServingSizeFormField form={form} food={food} />
            </div>
          ) : (
            <>
              <ServingsFormField form={form} />
              <Separator />
              <ServingSizeFormField form={form} food={food} />
            </>
          )}
          <Separator />
          <div className="flex w-full justify-end">
            <MealTypeFormField
              form={form}
              className="w-full min-w-[130px]  sm:w-[50%]"
            />
          </div>
          <Separator />
          <NutritionalInfoAccordion
            food={food}
            nutritionalMultiplier={nutritionalMultiplier}
          />

          <div className=" flex w-[100%] justify-end">
            <Button
              variant="secondary"
              disabled={isPending}
              type="submit"
              className=" "
            >
              {submitButtonText}
            </Button>
          </div>
        </form>
      </Form>
      {isPending ? <LinearProgress color="inherit" /> : null}
    </>
  );
};

export default FoodDiaryEntryForm;
