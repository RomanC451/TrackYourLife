import { useCustomMutation } from "@/hooks/useCustomMutation";
import {
  OngoingTrainingDto,
  OngoingTrainingsApi,
} from "@/services/openapi/api";

import {
  ongoingTrainingsQueryKeys,
  setActiveOngoingTrainingQueryDataByAction,
} from "../queries/ongoingTrainingsQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

type Variables = {
  ongoingTraining: OngoingTrainingDto;
};

const useSkipExerciseMutation = () => {
  const skipExerciseMutation = useCustomMutation({
    mutationFn: ({ ongoingTraining }: Variables) =>
      ongoingTrainingsApi.skipExercise({
        ongoingTrainingId: ongoingTraining.id,
      }),
    meta: {
      invalidateQueries: [ongoingTrainingsQueryKeys.active],
    },

    onSuccess: () => {
      setActiveOngoingTrainingQueryDataByAction({
        action: "skip",
      });
    },
  });

  return skipExerciseMutation;
};

export default useSkipExerciseMutation;
