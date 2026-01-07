import { UseMutationResult } from "@tanstack/react-query";

import useCustomForm from "@/components/forms/useCustomForm";
import { ExerciseDto } from "@/services/openapi";

import {
  ExerciseFormSchema,
  exerciseFormSchema,
} from "../../data/exercisesSchemas";
import { ExerciseMutationVariables } from "./ExerciseDialog";

function useExerciseDialog<TResponse>({
  onSuccess,
  mutation,
  defaultValues,
  setTab,
}: {
  onSuccess: (exercise: Partial<ExerciseDto>) => void;
  mutation: UseMutationResult<
    TResponse,
    Error | undefined,
    ExerciseMutationVariables,
    unknown
  >;
  defaultValues: ExerciseFormSchema;
  setTab: (tab: string) => void;
}) {
  "use no memo";
  const { form, handleCustomSubmit } = useCustomForm({
    formSchema: exerciseFormSchema,
    defaultValues: defaultValues,
    onSubmit: async (formData) => {
      mutation.mutate(
        {
          request: formData,
          setError: (name, error, options) => {
            form.setError(name, error, options);
            if (name === "exerciseSets") {
              setTab("sets");
            } else {
              setTab("details");
            }
          },
          id: defaultValues.id,
        },
        {
          onSuccess: (_, variables) => {
            onSuccess?.({
              ...variables.request,
              exerciseSets: variables.request.exerciseSets,
            });
          },
        },
      );
    },
  });

  return { form, handleCustomSubmit };
}

export default useExerciseDialog;
