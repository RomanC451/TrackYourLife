import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { TrainingDto, TrainingsApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

import { ongoingTrainingsQueryKeys } from "../../ongoing-workout/queries/ongoingTrainingsQuery";
import { trainingsQueryKeys } from "../queries/trainingsQueries";

const trainingsApi = new TrainingsApi();

type DeleteTrainingMutationVariables = {
  id: string;
  name: string;
  force?: boolean;
};

const useDeleteTrainingMutation = () => {
  const deleteTrainingMutation = useCustomMutation({
    mutationFn: ({ id, force = false }: DeleteTrainingMutationVariables) =>
      trainingsApi.deleteTraining(id, { force }),

    meta: {
      onSuccessToast: {
        message: "Training deleted",
        type: "success",
      },
      invalidateQueries: [
        trainingsQueryKeys.all,
        ongoingTrainingsQueryKeys.active,
      ],
    },

    onSuccess: (_, variables) => {
      queryClient.setQueryData(
        trainingsQueryKeys.all,
        (oldData: TrainingDto[]) =>
          oldData
            .filter((training) => training.id !== variables.id)
            .sort((a, b) => a.name.localeCompare(b.name)),
      );
    },

    onError: (error: ApiError) => {
      handleApiError({
        error,
      });
    },
  });

  return deleteTrainingMutation;
};

export default useDeleteTrainingMutation;
