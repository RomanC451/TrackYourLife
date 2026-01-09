import { useCustomMutation } from "@/hooks/useCustomMutation";
import {
  OngoingTrainingDto,
  OngoingTrainingsApi,
} from "@/services/openapi/api";

import { ongoingTrainingsQueryKeys } from "../queries/ongoingTrainingsQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

type Variables = {
  ongoingTraining: OngoingTrainingDto;
  exerciseIndex: number;
};

const useJumpToExerciseMutation = () => {
  const jumpToExerciseMutation = useCustomMutation({
    mutationFn: ({ ongoingTraining, exerciseIndex }: Variables) =>
      ongoingTrainingsApi.jumpToExercise({
        ongoingTrainingId: ongoingTraining.id,
        exerciseIndex: exerciseIndex,
      }),
    meta: {
      invalidateQueries: [ongoingTrainingsQueryKeys.active],
    },
  });

  return jumpToExerciseMutation;
};

export default useJumpToExerciseMutation;
