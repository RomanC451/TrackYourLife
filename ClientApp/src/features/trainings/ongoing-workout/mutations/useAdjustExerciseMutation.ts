import { useCustomMutation } from "@/hooks/useCustomMutation";
import { ExerciseSetChange, OngoingTrainingsApi } from "@/services/openapi/api";

const ongoingTrainingsApi = new OngoingTrainingsApi();

type Variables = {
  ongoingTrainingId: string;
  exerciseId: string;
  changes: ExerciseSetChange[];
};

const useAdjustExerciseMutation = () => {
  const adjustExerciseMutation = useCustomMutation({
    mutationFn: ({ ongoingTrainingId, exerciseId, changes }: Variables) =>
      ongoingTrainingsApi.adjustExerciseSets(ongoingTrainingId, {
        exerciseId,
        exerciseSetChanges: changes,
      }),
  });

  return adjustExerciseMutation;
};

export default useAdjustExerciseMutation;
