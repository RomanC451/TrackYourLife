import { useCustomMutation } from "@/hooks/useCustomMutation";
import { ExerciseSet, OngoingTrainingsApi } from "@/services/openapi/api";


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
      })
  });

  return adjustExerciseMutation;
};

export default useAdjustExerciseMutation;
