import useCustomForm from "@/components/forms/useCustomForm"
import { ExerciseFormSchema, exerciseFormSchema } from "../../data/exercisesSchemas"

import { UseMutationResult } from "@tanstack/react-query"
import { ExerciseMutationVariables } from "./ExerciseDialog"
import { ExerciseDto } from "@/services/openapi"

function useExerciseDialog<TResponse>({
    onSuccess,
    mutation,
    defaultValues,
    setTab
}: {
    onSuccess: (exercise: Partial<ExerciseDto>) => void,
    mutation: UseMutationResult<
    TResponse,
    Error | undefined,
    ExerciseMutationVariables,
    unknown
    >,
    defaultValues: ExerciseFormSchema,
    setTab: (tab: string) => void
}) {
    const { form, handleCustomSubmit } = useCustomForm({
        formSchema: exerciseFormSchema,
        defaultValues: defaultValues,
        onSubmit: (formData) => {
            mutation.mutate({
                request: formData,
                setError: (name, error, options) => {
                    form.setError(name, error, options)
                    if (name !== 'exerciseSets') {
                        setTab('details');
                    } else {
                        setTab('sets');
                    }
                },
                id: defaultValues.id
            }, {
                onSuccess: (_, variables) => {
                    onSuccess?.(variables.request);
                }
            });
        }
    });

    return { form, handleCustomSubmit };
}

export default useExerciseDialog;
