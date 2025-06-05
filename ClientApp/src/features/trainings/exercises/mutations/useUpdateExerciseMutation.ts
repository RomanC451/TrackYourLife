import { ExercisesApi } from "@/services/openapi";
import { ExerciseMutationVariables } from "../components/exercisesDialogs/ExerciseDialog";
import { useMutation } from "@tanstack/react-query";
import Assert from "@/lib/assert";
import exerciseUpdatedToast from "../toasts/exerciseUpdatedToast";
import { invalidateExercisesQuery, setExercisesQueryData } from "../queries/useExercisesQuery";

import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";
import useDelayedLoading from "@/hooks/useDelayedLoading";



const exercisesApi = new ExercisesApi();


const useUpdateExerciseMutation = () => {
    const updateExerciseMutation = useMutation({
        mutationFn: ({id, request}: ExerciseMutationVariables) => {
            Assert.isNotUndefined(id, "Id is required");
            
            return exercisesApi.updateExercise(id, request);
        },
        onSuccess: (_, {id, request}) => {
            Assert.isNotUndefined(id, "Id is required");

            exerciseUpdatedToast({
                name: request.name,
            });
            setExercisesQueryData({
                setter: (oldData) => [...oldData.filter(e => e.id !== id), {
                    id: id,
                    name: request.name,
                    description: request.description,
                    equipment: request.equipment,
                    exerciseSets: request.exerciseSets,
                    createdOnUtc: new Date().toISOString(),
                    updatedOnUtc: undefined,
                    isLoading: true,
                }],
            });
        },
        onError: (error: ApiError, {setError}) => {
            handleApiError({
                error,
                validationErrorsHandler: setError
            });
        },
        onSettled: () => {
            invalidateExercisesQuery();
        }
    })

    const isPending = useDelayedLoading(updateExerciseMutation.isPending);

    return {
        updateExerciseMutation,
        isPending
    }
}

export default useUpdateExerciseMutation;
