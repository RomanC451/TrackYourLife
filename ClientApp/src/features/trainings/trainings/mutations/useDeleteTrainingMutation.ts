import { TrainingsApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";
import { useMutation } from "@tanstack/react-query";
import { invalidateTrainingsQuery, setTrainingsQueryData } from "../queries/useTrainingsQuery";
import trainingDeletedToast from "../toasts/trainingDeletedToast";
import useDelayedLoading from "@/hooks/useDelayedLoading";


const trainingsApi = new TrainingsApi();


type DeleteTrainingMutationVariables = {
    id: string;
    name: string;
    force?: boolean;
}

 const useDeleteTrainingMutation = () => {
    const deleteTrainingMutation = useMutation({
        mutationFn: ({id, force = false}: DeleteTrainingMutationVariables) => trainingsApi.deleteTraining(id, {force}),
        onSuccess: (_, variables) => {
            setTrainingsQueryData({
                setter: (oldData) => oldData.filter((training) => training.id !== variables.id),
            });
            trainingDeletedToast({
                    name: variables.name,
             });
        },
        onError: (error: ApiError) => {
            handleApiError({
                error,
            });
        },
        onSettled: () => {
            invalidateTrainingsQuery();
        },
    });

    const isPending = useDelayedLoading(deleteTrainingMutation.isPending);

    return { deleteTrainingMutation, isPending };
};

export default useDeleteTrainingMutation;
