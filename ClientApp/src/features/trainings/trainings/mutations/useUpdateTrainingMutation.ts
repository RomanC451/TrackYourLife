import { useCustomMutation } from "@/hooks/useCustomMutation";
import Assert from "@/lib/assert";
import { queryClient } from "@/queryClient";
import { TrainingDto, TrainingsApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

import { trainingsQueryKeys } from "../queries/trainingsQueries";
import { TrainingMutationVariables } from "./useCreateTrainingMutation";

const trainingsApi = new TrainingsApi();

const useUpdateTrainingMutation = () => {
  const updateTrainingMutation = useCustomMutation({
    mutationFn: ({ id, request }: TrainingMutationVariables) => {
      Assert.isNotUndefined(id, "Id is required");

      return trainingsApi.updateTraining(id, request);
    },

    meta: {
      onSuccessToast: {
        message: "Training updated",
        type: "success",
      },
      invalidateQueries: [trainingsQueryKeys.all],
    },

    onSuccess: (_, { id, request }) => {
      queryClient.setQueryData(
        trainingsQueryKeys.all,
        (oldData: TrainingDto[]) => {
          const training = oldData.find((training) => training.id === id);

          if (!training) {
            return oldData;
          }

          const newData = [
            ...oldData.filter((training) => training.id !== id),
            {
              id,
              name: request.name,
              duration: request.duration,
              muscleGroups: request.muscleGroups,
              difficulty: request.difficulty,
              restSeconds: request.restSeconds,
              description: request.description,
              exercises: training.exercises,
              createdOnUtc: new Date().toISOString(),
              isLoading: true,
              isDeleting: false,
            },
          ].sort((a, b) => a.name.localeCompare(b.name));

          return newData;
        },
      );
    },
    onError: (error: ApiError, { setError }) => {
      handleApiError({
        error,
        validationErrorsHandler: setError,
      });
    },
  });

  return updateTrainingMutation;
};

export default useUpdateTrainingMutation;
