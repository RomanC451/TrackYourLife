import { LinearProgress } from "@mui/material";
import { UseFormReturn } from "react-hook-form";

import { Button } from "@/components/ui/button";
import { Form } from "@/components/ui/form";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { screensEnum } from "@/constants/tailwindSizes";
import { useAppGeneralStateContext } from "@/contexts/AppGeneralContextProvider";
import ServingsFormField from "@/features/nutrition/common/components/formFields/ServingsFormField";
import ServingSizeFormField from "@/features/nutrition/common/components/formFields/ServingSizeFormField";
import NutritionalInfoAccordion from "@/features/nutrition/common/components/NutritionalInfoAccordion";
import { LoadingState } from "@/hooks/useDelayedLoading";
import { FoodDto } from "@/services/openapi";

import { AddIngredientFormSchema } from "../../data/schemas";

type IngredientFormProps = {
  food: FoodDto;
  form: UseFormReturn<
    {
      nrOfServings: number;
      servingSizeIndex: number;
    },
    unknown,
    undefined
  >;
  nutritionalMultiplier: number;
  onSubmit(formData: AddIngredientFormSchema): void;
  isPending: LoadingState;
  submitButtonText?: string;
};

function IngredientForm({
  food,
  form,
  nutritionalMultiplier,
  onSubmit,
  isPending,
  submitButtonText,
}: IngredientFormProps) {
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
              <ServingsFormField control={form.control} />
              <ServingSizeFormField control={form.control} food={food} />
            </div>
          ) : (
            <>
              <ServingsFormField control={form.control} />
              <Separator />
              <ServingSizeFormField control={form.control} food={food} />
            </>
          )}
          <Separator />
          <NutritionalInfoAccordion
            nutritionalContents={food.nutritionalContents}
            nutritionalMultiplier={nutritionalMultiplier}
          />

          <div className="flex w-[100%] justify-end">
            <Button
              variant="default"
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

IngredientForm.Loading = function IngredientFormLoading() {
  const { screenSize } = useAppGeneralStateContext();

  return (
    <>
      {screenSize.width >= screensEnum.sm ? (
        <div className="flex gap-3">
          <div className="w-[180px]">
            <Skeleton className="mb-1 h-[19px] w-[125px]" />
            <Skeleton className="h-[40px] w-full" />
          </div>
          <div className="grow">
            <Skeleton className="mb-1 h-[19px] w-[100px]" />
            <Skeleton className="h-[40px] w-full" />
          </div>
        </div>
      ) : (
        <>
          <div className="w-full">
            <Skeleton className="mb-1 h-[19px] w-[125px]" />
            <Skeleton className="h-[40px] w-full" />
          </div>
          <Separator />
          <div className="full">
            <Skeleton className="mb-1 h-[19px] w-[100px]" />
            <Skeleton className="h-[40px] w-full" />
          </div>
        </>
      )}
      <Separator />
      {/* <div className="flex w-full justify-end">
          <div className="w-full min-w-[130px]  sm:w-[50%]">
            <Skeleton className="mb-1 h-[19px] w-[100px]" />
            <Skeleton className="h-[40px] w-full" />
          </div>
        </div> */}
      <Separator />
      <div className="flex items-center justify-between">
        <Skeleton className="h-[24px] w-[175px]" />
        <Skeleton className="h-[20px] w-[20px]" />
      </div>
      <Separator />
      <div className="flex w-full justify-end">
        <Skeleton className="h-[40px] w-[150px]" />
      </div>
    </>
  );
};

export default IngredientForm;
