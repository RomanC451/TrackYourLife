import { StatusCodes } from "http-status-codes";
import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { OngoingTrainingsApi } from "@/services/openapi/api";
import { handleApiError } from "@/services/openapi/handleApiError";

import { ongoingTrainingsQueryKeys } from "../queries/ongoingTrainingsQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

type Variables = {
  trainingId: string;
};

const useCreateOngoingTrainingMutation = () => {
  const createOngoingTrainingMutation = useCustomMutation({
    mutationFn: ({ trainingId }: Variables) => {
      return ongoingTrainingsApi.createOngoingTraining({ trainingId });
    },
    meta: {
      invalidateQueries: [ongoingTrainingsQueryKeys.active],
      awaitInvalidationQuery: ongoingTrainingsQueryKeys.active,
    },
    onError: (error) => {
      handleApiError({
        error,
        errorHandlers: {
          [StatusCodes.BAD_REQUEST]: {
            "Trainings.NoExercises": () => {
              toast.error("Training has no exercises", {
                description: "Please add some exercises to the training",
              });
            },
          },
        },
      });
    },
  });

  return createOngoingTrainingMutation;
};

export default useCreateOngoingTrainingMutation;
