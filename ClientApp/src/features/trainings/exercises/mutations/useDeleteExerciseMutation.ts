import { ExercisesApi } from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";
import { ApiError } from "@/services/openapi/apiSettings";
import { useMutation } from "@tanstack/react-query";
import exerciseDeletedToast from "../toasts/exerciseDeletedToast";
import useDelayedLoading from "@/hooks/useDelayedLoading";

import { toast } from "sonner";
import { StatusCodes } from "http-status-codes";
import { apiExercisesErrors } from "../data/apiExercisesErrors";
import { invalidateExercisesQuery, setExercisesQueryData } from "../queries/useExercisesQuery";
import { invalidateTrainingsQuery } from "../../trainings/queries/useTrainingsQuery";

const exercisesApi = new ExercisesApi();

type DeleteExerciseMutationVariables ={
    id: string;
    forceDelete: boolean;
    name: string;
    onShowAlert?:() => void;
}

function useDeleteExerciseMutation() {
    const deleteExerciseMutation = useMutation({
        mutationFn: ({id, forceDelete}: DeleteExerciseMutationVariables) => {
            return exercisesApi.deleteExercise(id, {forceDelete});
        },

        onSuccess: (_, variables) => {
            exerciseDeletedToast({
                name: variables.name,
            });

            setExercisesQueryData({
                setter: (oldData) => {
                    const deletedExercise = oldData.find((exercise) => exercise.id === variables.id);

                    if (deletedExercise) {
                        return [...oldData.filter((exercise) => exercise.id !== variables.id), {
                            ...deletedExercise,

                            isDeleting: true,
                        }];
                    }

                    return oldData;
                },
            });
        },
        onError: (error: ApiError, variables) => {
            handleApiError(
                {
                    error,
                    errorHandlers: {
                        [StatusCodes.BAD_REQUEST]: {
                            default: () => {
                                toast.error("Exercise not found");
                            },
                            [apiExercisesErrors.Exercise.UsedInTrainings]: variables.onShowAlert,
                        }
                    }
                }
            );
        },
        onSettled: () => {
            invalidateExercisesQuery();
            invalidateTrainingsQuery();
        },
    });

    const isPending = useDelayedLoading(deleteExerciseMutation.isPending);

    return {
        deleteExerciseMutation,
        isPending,
    }
}

export default useDeleteExerciseMutation