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
import { IdResponse, RecipeDto } from "@/services/openapi";

import { RecipeDiaryFormSchema } from "../../data/recipeDiarySchema";
import { AddRecipeDiaryMutationVariables } from "../../mutations/useAddRecipeDiaryMutation";
import { UpdateRecipeDiaryMutationVariables } from "../../mutations/useUpdateRecipeDiaryMutation";
import { RecipeDiaryForm } from "./RecipeDiaryForm";
import useRecipeDiaryDialog from "./useRecipeDiaryDialog";

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
    title: "Add recipe diary",
    description: "Add a new recipe diary",
    submitButtonText: "Add",
  },
  edit: {
    title: "Edit recipe diary",
    description: "Edit the details of this recipe diary",
    submitButtonText: "Save",
  },
};

export function RecipeDiaryDialog({
  dialogType,
  mutation,
  defaultValues,
  onSuccess,
  onClose,
  recipe,
}: {
  dialogType: DialogType;
  mutation:
    | UseCustomMutationResult<
        IdResponse,
        Error | undefined,
        AddRecipeDiaryMutationVariables,
        unknown
      >
    | UseCustomMutationResult<
        void,
        Error | undefined,
        UpdateRecipeDiaryMutationVariables,
        unknown
      >;
  defaultValues: RecipeDiaryFormSchema;
  onSuccess: () => void;
  onClose: () => void;
  recipe: RecipeDto;
}) {
  const { handleCustomSubmit, form } = useRecipeDiaryDialog({
    onSuccess: () => {
      form.reset(defaultValues);
      onSuccess?.();
    },
    mutation: mutation,
    defaultValues: defaultValues,
  });

  const [nutritionMultiplier, setNutritionMultiplier] = useState(
    recipe.servingSizes.find((ss) => ss.id === defaultValues.servingSizeId)
      ?.nutritionMultiplier ?? 1,
  );

  useEffect(() => {
    const subscription = form.watch((values, { name }) => {
      if (name !== "servingSizeId" && name !== "quantity") {
        return;
      }

      const servingSize =
        recipe.servingSizes.find((ss) => ss.id === values.servingSizeId) ??
        recipe.servingSizes[0];

      setNutritionMultiplier(
        servingSize.nutritionMultiplier * Number(values.quantity),
      );
    });

    return () => subscription.unsubscribe();
  }, [form, recipe.servingSizes]);

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
          nutritionalContents={recipe.nutritionalContents}
          nutritionMultiplier={nutritionMultiplier}
        />
        <RecipeDiaryForm
          form={form}
          handleCustomSubmit={handleCustomSubmit}
          submitButtonText={dialogTexts[dialogType].submitButtonText}
          pendingState={mutation.pendingState}
          recipe={recipe}
        />
      </DialogContent>
    </Dialog>
  );
}
