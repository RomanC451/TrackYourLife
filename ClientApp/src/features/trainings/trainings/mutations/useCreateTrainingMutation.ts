import useDelayedLoading from "@/hooks/useDelayedLoading";
import { CreateTrainingRequest, TrainingsApi } from "@/services/openapi";
import { useMutation } from "@tanstack/react-query";
import {  invalidateTrainingsQuery, setTrainingsQueryData } from "../queries/useTrainingsQuery";
import trainingCreatedToast from "../toasts/trainingCreatedToast";
import { v4 as uuidv4 } from "uuid";
import { ApiError } from "@/services/openapi/apiSettings";
import { ErrorOption } from "react-hook-form";
import { TrainingFormSchema } from "../data/trainingsSchemas";
import { handleApiError } from "@/services/openapi/handleApiError";


const trainingsApi = new TrainingsApi();

export type TrainingMutationVariables ={
    id: string | undefined,
    request: CreateTrainingRequest,
    setError: (
        name: keyof TrainingFormSchema,
        error: ErrorOption,
        options?: {
          shouldFocus: boolean;
        },
      ) => void,
}
const useCreateTrainingMutation = (
) => {
  const createTrainingMutation = useMutation({
    mutationFn: ({request}: TrainingMutationVariables) => {
      return trainingsApi.createTraining(request);
    },

    onSuccess: (_, {request}) => {
        trainingCreatedToast({
            name: request.name,
        });  

        setTrainingsQueryData({
            setter: (oldData) => [...oldData, {
                id: uuidv4(),
                name: request.name,
                duration: request.duration,
                description: request.description,
                exercises: [],
                createdOnUtc: new Date().toISOString(),
                isLoading: true,
                isDeleting: false,
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

  const isPending = useDelayedLoading(createTrainingMutation.isPending);

  return { createTrainingMutation, isPending };
};

export default useCreateTrainingMutation;


