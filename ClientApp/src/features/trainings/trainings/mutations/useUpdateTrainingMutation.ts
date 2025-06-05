import useDelayedLoading from "@/hooks/useDelayedLoading";
import { TrainingsApi } from "@/services/openapi";
import { useMutation } from "@tanstack/react-query";
import {  invalidateTrainingsQuery, setTrainingsQueryData } from "../queries/useTrainingsQuery";
import { v4 as uuidv4 } from "uuid";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";
import trainingUpdatedToast from "../toasts/trainingUpdatedToast";
import { TrainingMutationVariables } from "./useCreateTrainingMutation";
import Assert from "@/lib/assert";


const trainingsApi = new TrainingsApi();


const useUpdateTrainingMutation = (
) => {
  const updateTrainingMutation = useMutation({
    mutationFn: ({id, request}: TrainingMutationVariables) => {
      Assert.isNotUndefined(id, "Id is required");
      
      return trainingsApi.updateTraining(id, request);
    },

    onSuccess: (_, {request}) => {
        trainingUpdatedToast({
            name: request.name,
        });  

        setTrainingsQueryData({
            setter: (oldData) => [...oldData, {
                id: uuidv4(),
                name: request.name,
                duration: request.duration,
                description: request.description,
                exercises: [],
            }],
        });
    },
    onError: (error: ApiError, {setError}) => {
        handleApiError({
            error,
            validationErrorsHandler: 
                setError
            
        });
    },
    onSettled: () => {
        invalidateTrainingsQuery();
    },
  });

  const isPending = useDelayedLoading(updateTrainingMutation.isPending);

  return { updateTrainingMutation, isPending };
};

export default useUpdateTrainingMutation;


