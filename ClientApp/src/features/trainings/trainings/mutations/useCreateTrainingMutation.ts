import { ErrorOption } from "react-hook-form";
import { v4 as uuidv4 } from "uuid";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import {
  CreateTrainingRequest,
  TrainingDto,
  TrainingsApi,
} from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";

import { TrainingFormSchema } from "../data/trainingsSchemas";
import { trainingsQueryKeys } from "../queries/trainingsQueries";

const trainingsApi = new TrainingsApi();

export type TrainingMutationVariables = {
  id: string | undefined;
  request: CreateTrainingRequest;
  setError: (
    name: keyof TrainingFormSchema,
    error: ErrorOption,
    options?: {
      shouldFocus: boolean;
    },
  ) => void;
};
const useCreateTrainingMutation = () => {
  const createTrainingMutation = useCustomMutation({
    mutationFn: ({ request }: TrainingMutationVariables) =>
      trainingsApi.createTraining(request),

    meta: {
      onSuccessToast: {
        message: "Training created",
        type: "success",
      },
      invalidateQueries: [trainingsQueryKeys.all],
    },

    onSuccess: (_, { request }) => {
      queryClient.setQueryData(
        trainingsQueryKeys.all,
        (oldData: TrainingDto[]) =>
          [
            ...oldData,
            {
              id: uuidv4(),
              name: request.name,
              duration: request.duration,
              muscleGroups: request.muscleGroups,
              difficulty: request.difficulty,
              restSeconds: request.restSeconds,
              description: request.description,
              exercises: [],
              createdOnUtc: new Date().toISOString(),
              isLoading: true,
              isDeleting: false,
            },
          ].sort((a, b) => a.name.localeCompare(b.name)),
      );
    },
    onError: (error, { setError }) => {
      handleApiError({
        error,
        validationErrorsHandler: setError,
      });
    },
  });

  return createTrainingMutation;
};

export default useCreateTrainingMutation;
