import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { ExercisesHistoriesApi } from "@/services/openapi/api";

import { exerciseHistoryQueryKeys } from "../queries/exerciseHistoryQuery";

const exercisesHistoriesApi = new ExercisesHistoriesApi();

type Variables = {
  id: string;
  exerciseId: string;
};

const useDeleteExerciseHistoryMutation = () => {
  const deleteExerciseHistoryMutation = useCustomMutation({
    mutationFn: ({ id }: Variables) => {
      return exercisesHistoriesApi.deleteExerciseHistory(id);
    },
    meta: {
      onSuccessToast: {
        message: "Adjustment history deleted",
        type: "success",
      },
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: exerciseHistoryQueryKeys.byExerciseId(variables.exerciseId),
      });
    },
  });

  return deleteExerciseHistoryMutation;
};

export default useDeleteExerciseHistoryMutation;
