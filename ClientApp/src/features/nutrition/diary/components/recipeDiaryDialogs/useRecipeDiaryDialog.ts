import useCustomForm from "@/components/forms/useCustomForm";
import { UseCustomMutationResult } from "@/hooks/useCustomMutation";
import { IdResponse } from "@/services/openapi";

import {
  recipeDiaryFormSchema,
  RecipeDiaryFormSchema,
} from "../../data/recipeDiarySchema";
import { AddRecipeDiaryMutationVariables } from "../../mutations/useAddRecipeDiaryMutation";
import { UpdateRecipeDiaryMutationVariables } from "../../mutations/useUpdateRecipeDiaryMutation";

type useRecipeDiaryDialogProps = {
  onSuccess: () => void;
  defaultValues: RecipeDiaryFormSchema;

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
};

function useRecipeDiaryDialog({
  onSuccess,
  mutation,
  defaultValues,
}: useRecipeDiaryDialogProps) {
  const { form, handleCustomSubmit } = useCustomForm({
    formSchema: recipeDiaryFormSchema,
    defaultValues: defaultValues,
    onSubmit: async (formData) => {
      await mutation.mutateAsync({
        id: formData.id!,
        ...formData,
      });
      onSuccess?.();
    },
  });

  return { form, handleCustomSubmit };
}

export default useRecipeDiaryDialog;
