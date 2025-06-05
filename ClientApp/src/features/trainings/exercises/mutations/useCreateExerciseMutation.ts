import { CreateExerciseRequest, ExercisesApi } from "@/services/openapi";
import { useMutation } from "@tanstack/react-query";
import { ExerciseFormSchema } from "../data/exercisesSchemas";
import { ErrorOption } from "react-hook-form";
import exerciseCreatedToast from "../toasts/exerciseCreatedToast";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";
import useDelayedLoading from "@/hooks/useDelayedLoading";
import { invalidateExercisesQuery, setExercisesQueryData } from "../queries/useExercisesQuery";

import { v4 as uuidv4 } from 'uuid';

const exercisesApi = new ExercisesApi();

export type CreateExerciseMutationVariables = {
    request: CreateExerciseRequest,
    setError: (
        name: keyof ExerciseFormSchema,
        error: ErrorOption,
        options?: {
            shouldFocus: boolean;
        },
    ) => void,
}

function useCreateExerciseMutation() {
    const createExerciseMutation = useMutation({
        mutationFn: ({request}: CreateExerciseMutationVariables) => exercisesApi.createExercise(request),
        onSuccess: (_, {request}) => {
            exerciseCreatedToast({
                name: request.name,
            });

            setExercisesQueryData({
                setter: (oldData) => [...oldData, {
                    id: uuidv4(),
                    name: request.name,
                    description: request.description,
                    equipment: undefined,
                    exerciseSets: request.exerciseSets,
                    videoUrl: request.videoUrl,
                    pictureUrl: request.pictureUrl,
                    createdOnUtc: new Date().toISOString(),
                    updatedOnUtc: undefined,
                    isLoading: true,
                }]
            })
        },

        onError: (error: ApiError, {setError}) => {
            handleApiError({
                error,
                validationErrorsHandler: setError,
            });
        },

        onSettled: () => {
            invalidateExercisesQuery();
        },
        


    });

    const isPending = useDelayedLoading(createExerciseMutation.isPending);

    return { createExerciseMutation, isPending };
}

export default useCreateExerciseMutation