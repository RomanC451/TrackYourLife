import { useMutation } from "@tanstack/react-query";

import { OngoingTrainingsApi, ExerciseSetChange } from "@/services/openapi/api";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";


const ongoingTrainingsApi = new OngoingTrainingsApi();

const useAdjustExerciseMutation = () => {
  const adjustExerciseMutation = useMutation({
    mutationFn: ({ ongoingTrainingId, exerciseId, changes }: { ongoingTrainingId: string, exerciseId: string, changes: ExerciseSetChange[] }) => {
      return ongoingTrainingsApi.adjustExerciseSets(ongoingTrainingId, {
        exerciseId,
        exerciseSetChanges: changes,
      });
    },
    onError: (error) => {
      toastDefaultServerError(error);
    },
  });

  const isPending = useDelayedLoading(adjustExerciseMutation.isPending);

  return { adjustExerciseMutation, isPending };
};

export default useAdjustExerciseMutation;