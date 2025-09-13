import { UseMutationResult } from "@tanstack/react-query";

import useCustomForm from "@/components/forms/useCustomForm";

import {
  TrainingFormSchema,
  trainingFormSchema,
} from "../../data/trainingsSchemas";
import { TrainingMutationVariables } from "../../mutations/useCreateTrainingMutation";

const useTrainingDialog = <TResponse>({
  onSuccess,
  mutation,
  defaultValues,
  setTab,
}: {
  onSuccess?: () => void;
  mutation: UseMutationResult<
    TResponse,
    Error | undefined,
    TrainingMutationVariables,
    unknown
  >;
  defaultValues: TrainingFormSchema;
  setTab: (tab: "details" | "exercises") => void;
}) => {
  const { form, handleCustomSubmit } = useCustomForm({
    formSchema: trainingFormSchema,
    defaultValues: defaultValues,
    sessionStorageKey: defaultValues.id
      ? `training-form-data-${defaultValues.id}`
      : "create",
    queryData: defaultValues,
    updateAlwaysOnInvalidation: ["exercises"],
    onSubmit: async (formData) => {
      mutation.mutate(
        {
          id: defaultValues.id,
          request: {
            ...formData,
            exercisesIds: formData.exercises.map(
              (exercise) => exercise.id || "",
            ),
          },
          setError: (name, error, options) => {
            form.setError(name, error, options);
            if (name === "exercises") {
              setTab("exercises");
            } else {
              setTab("details");
            }
          },
        },
        {
          onSuccess: () => {
            onSuccess?.();
          },
        },
      );
    },
  });

  return {
    form,
    handleCustomSubmit,
  };
};

export default useTrainingDialog;
