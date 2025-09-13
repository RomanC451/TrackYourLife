import { useEffect, useState } from "react";

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import MacrosDialogHeader from "@/features/nutrition/common/components/macros/MacrosDialogHeader";
import { UseCustomMutationResult } from "@/hooks/useCustomMutation";
import { FoodDto } from "@/services/openapi";

import { FoodDiaryFormSchema } from "../../data/foodDiarySchemas";
import { AddFoodDiaryMutationVariables } from "../../mutations/useAddFoodDiaryMutation";
import { UpdateFoodDiaryMutationVariables } from "../../mutations/useUpdateFoodDiaryMutation";
import FoodDiaryForm from "./FoodDiaryForm";
import useFoodDiaryDialog from "./useFoodDiaryDialog";

type DialogType = "create" | "edit";

const dialogTexts: Record<
  DialogType,
  {
    title: string;
    description: string;
    submitButtonText: string;
  }
> = {
  create: {
    title: "Add food diary",
    description: "Add a new food diary",
    submitButtonText: "Add",
  },
  edit: {
    title: "Edit food diary",
    description: "Edit the details of this food diary",
    submitButtonText: "Save",
  },
};

export function FoodDiaryDialog<TResponse>({
  dialogType,
  mutation,
  defaultValues,
  onSuccess,
  onClose,
  food,
}: {
  dialogType: "create" | "edit";
  mutation:
    | UseCustomMutationResult<
        TResponse,
        Error | undefined,
        AddFoodDiaryMutationVariables,
        unknown
      >
    | UseCustomMutationResult<
        TResponse,
        Error | undefined,
        UpdateFoodDiaryMutationVariables,
        unknown
      >;
  defaultValues: FoodDiaryFormSchema;
  onSuccess?: () => void;
  onClose?: () => void;
  food: FoodDto;
}) {
  const { handleCustomSubmit, form } = useFoodDiaryDialog({
    onSuccess: () => {
      form.reset(defaultValues);
      onSuccess?.();
    },
    mutation: mutation,
    defaultValues: defaultValues,
  });

  const [nutritionMultiplier, setNutritionMultiplier] = useState(1);

  useEffect(() => {
    const subscription = form.watch((values, { name }) => {
      if (name !== "servingSizeId" && name !== "quantity") {
        return;
      }

      const servingSize =
        food.servingSizes.find((ss) => ss.id === values.servingSizeId) ??
        food.servingSizes[0];

      setNutritionMultiplier(
        servingSize.nutritionMultiplier * Number(values.quantity),
      );
    });

    return () => subscription.unsubscribe();
  }, [form, food.servingSizes]);

  return (
    <Dialog
      onOpenChange={(state) => {
        if (!state) {
          onClose?.();
        }
      }}
      defaultOpen={true}
    >
      <DialogContent className="p-6" withoutOverlay>
        <DialogHeader>
          <DialogTitle className="mb-2">
            {dialogTexts[dialogType].title}
          </DialogTitle>
          <DialogDescription hidden>
            {dialogTexts[dialogType].description}
          </DialogDescription>
        </DialogHeader>
        <MacrosDialogHeader
          nutritionalContents={food.nutritionalContents}
          nutritionMultiplier={nutritionMultiplier}
        />
        <FoodDiaryForm
          form={form}
          handleCustomSubmit={handleCustomSubmit}
          submitButtonText={dialogTexts[dialogType].submitButtonText}
          pendingState={mutation.pendingState}
          food={food}
        />
      </DialogContent>
    </Dialog>
  );
}
