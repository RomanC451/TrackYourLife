import { useCustomMutation } from "@/hooks/useCustomMutation";
import { ExerciseSet, OngoingTrainingsApi } from "@/services/openapi/api";

import { exerciseHistoryQueryKeys } from "../queries/exerciseHistoryQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

type Variables = {
  ongoingTrainingId: string;
  exerciseId: string;
  newSets: Array<ExerciseSet>;
};

const useAdjustExerciseMutation = () => {
  const adjustExerciseMutation = useCustomMutation({
    mutationFn: ({ ongoingTrainingId, exerciseId, newSets }: Variables) =>
      ongoingTrainingsApi.adjustExerciseSets(ongoingTrainingId, {
        exerciseId,
        newExerciseSets: newSets,
      }),
    meta: {
      invalidateQueries: [exerciseHistoryQueryKeys.all],
    },
  });

  return adjustExerciseMutation;
};

export default useAdjustExerciseMutation;
