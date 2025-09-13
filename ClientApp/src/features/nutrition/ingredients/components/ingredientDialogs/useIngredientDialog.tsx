import { UseMutationResult } from "@tanstack/react-query";

import useCustomForm from "@/components/forms/useCustomForm";
import { ApiError } from "@/services/openapi/apiSettings";

import {
  ingredientSchema,
  IngredientSchema,
} from "../../data/ingredientsSchemas";

function useIngredientDialog({
  onSuccess,
  mutation,
  defaultValues,
}: {
  onSuccess?: () => void;
  mutation: UseMutationResult<unknown, ApiError, IngredientSchema>;
  defaultValues: IngredientSchema;
}) {
  const { form, handleCustomSubmit } = useCustomForm({
    formSchema: ingredientSchema,
    defaultValues: defaultValues,
    onSubmit: async (formData) => {
      const result = await form.trigger();
      if (!result) {
        return;
      }

      mutation.mutate(formData, {
        onSuccess: () => {
          onSuccess?.();
        },
      });
    },
  });

  return {
    form,
    handleCustomSubmit,
  };
}

export default useIngredientDialog;
