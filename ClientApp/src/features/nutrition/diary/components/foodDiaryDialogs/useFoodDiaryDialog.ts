import useCustomForm from "@/components/forms/useCustomForm";
import { UseCustomMutationResult } from "@/hooks/useCustomMutation";

import {
  FoodDiaryFormSchema,
  foodDiaryFormSchema,
} from "../../data/foodDiarySchemas";
import { AddFoodDiaryMutationVariables } from "../../mutations/useAddFoodDiaryMutation";
import { UpdateFoodDiaryMutationVariables } from "../../mutations/useUpdateFoodDiaryMutation";

type useFoodDiaryDialogProps<TResponse> = {
  onSuccess: () => void;
  defaultValues: FoodDiaryFormSchema;
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
};

function useFoodDiaryDialog<TResponse>({
  onSuccess,
  mutation,
  defaultValues,
}: useFoodDiaryDialogProps<TResponse>) {
  const { form, handleCustomSubmit } = useCustomForm({
    formSchema: foodDiaryFormSchema,
    defaultValues: defaultValues,
    onSubmit: async (formData) => {
      await mutation.mutateAsync({
        id: formData.id!,
        foodId: formData.foodId,
        mealType: formData.mealType,
        servingSizeId: formData.servingSizeId.toString(),
        quantity: formData.quantity,
        entryDate: formData.entryDate,
      });
      onSuccess?.();
    },
  });

  return {
    form,
    handleCustomSubmit,
  };
}

export default useFoodDiaryDialog;
