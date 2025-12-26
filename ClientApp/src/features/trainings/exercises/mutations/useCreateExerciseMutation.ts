import { v4 as uuidv4 } from "uuid";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { ExerciseDto, ExercisesApi } from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";

import { ExerciseMutationVariables } from "../components/exercisesDialogs/ExerciseDialog";
import { exercisesQueryKeys } from "../queries/exercisesQuery";
import { exerciseSetSchemaToApiExerciseSet } from "../utils/exerciseSetsMappings";

const exercisesApi = new ExercisesApi();

function useCreateExerciseMutation() {
  const createExerciseMutation = useCustomMutation({
    mutationFn: ({ request }: ExerciseMutationVariables) =>
      exercisesApi.createExercise({
        ...request,
        exerciseSets: request.exerciseSets.map(
          exerciseSetSchemaToApiExerciseSet,
        ),
      }),

    meta: {
      noDefaultErrorToast: true,
      onSuccessToast: {
        message: "Exercise created",
        type: "success",
      },
      invalidateQueries: [exercisesQueryKeys.all],
    },

    onSuccess: (_, { request }) => {
      queryClient.setQueryData(
        exercisesQueryKeys.all,
        (oldData: ExerciseDto[]) => {
          return [
            ...oldData,
            {
              id: uuidv4(),
              name: request.name,
              description: request.description,
              muscleGroups: request.muscleGroups,
              difficulty: request.difficulty,
              equipment: request.equipment,
              exerciseSets: request.exerciseSets,
              videoUrl: request.videoUrl,
              pictureUrl: request.pictureUrl,
              createdOnUtc: new Date().toISOString(),
              modifiedOnUtc: undefined,
              isLoading: true,
              isDeleting: false,
            },
          ];
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

  return createExerciseMutation;
}

export default useCreateExerciseMutation;
