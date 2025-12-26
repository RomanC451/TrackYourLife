import { useCustomMutation } from "@/hooks/useCustomMutation";
import Assert from "@/lib/assert";
import { queryClient } from "@/queryClient";
import { ExerciseDto, ExercisesApi } from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";

import { ExerciseMutationVariables } from "../components/exercisesDialogs/ExerciseDialog";
import { exercisesQueryKeys } from "../queries/exercisesQuery";
import { exerciseSetSchemaToApiExerciseSet } from "../utils/exerciseSetsMappings";

const exercisesApi = new ExercisesApi();

const useUpdateExerciseMutation = () => {
  const updateExerciseMutation = useCustomMutation({
    mutationFn: ({ id, request }: ExerciseMutationVariables) => {
      Assert.isNotUndefined(id, "Id is required");

      return exercisesApi.updateExercise(id, {
        ...request,
        exerciseSets: request.exerciseSets.map(
          exerciseSetSchemaToApiExerciseSet,
        ),
      });
    },

    meta: {
      onSuccessToast: {
        message: "Exercise updated",
        type: "success",
      },
      invalidateQueries: [exercisesQueryKeys.all],
    },

    onSuccess: (_, { id, request }) => {
      Assert.isNotUndefined(id, "Id is required");

      queryClient.setQueryData(
        exercisesQueryKeys.all,
        (oldData: ExerciseDto[]) => {
          return [
            ...oldData.filter((e) => e.id !== id),
            {
              id: id,
              name: request.name,
              description: request.description,
              muscleGroups: request.muscleGroups,
              difficulty: request.difficulty,
              equipment: request.equipment,
              exerciseSets: request.exerciseSets,
              createdOnUtc: new Date().toISOString(),
              modifiedOnUtc: undefined,
              isLoading: true,
              isDeleting: false,
            },
          ].sort((a, b) => a.name.localeCompare(b.name));
        },
      );
    },
    onError: (error, { setError }) => {
      handleApiError({
        error,
        validationErrorsHandler: setError,
      });
    },
  });

  return updateExerciseMutation;
};

export default useUpdateExerciseMutation;
