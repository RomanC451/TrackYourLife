import { StatusCodes } from "http-status-codes";
import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { ExerciseDto, ExercisesApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

import { trainingsQueryKeys } from "../../trainings/queries/trainingsQueries";
import { apiExercisesErrors } from "../data/apiExercisesErrors";
import { exercisesQueryKeys } from "../queries/exercisesQuery";

const exercisesApi = new ExercisesApi();

type Variables = {
  id: string;
  forceDelete: boolean;
  name: string;
  onShowAlert?: () => void;
};

function useDeleteExerciseMutation() {
  const deleteExerciseMutation = useCustomMutation({
    mutationFn: ({ id, forceDelete }: Variables) => {
      return exercisesApi.deleteExercise(id, { forceDelete });
    },

    meta: {
      onSuccessToast: {
        message: "Exercise deleted",
        type: "success",
      },
      invalidateQueries: [exercisesQueryKeys.all, trainingsQueryKeys.all],
    },

    onMutate: (variables) => {
      const previousData = queryClient.getQueryData<ExerciseDto[]>(
        exercisesQueryKeys.all,
      );

      queryClient.setQueryData(
        exercisesQueryKeys.all,
        (oldData: ExerciseDto[]) => {
          const deletedExercise = oldData.find(
            (exercise) => exercise.id === variables.id,
          );

          if (deletedExercise) {
            return [
              ...oldData.filter((exercise) => exercise.id !== variables.id),
              {
                ...deletedExercise,

                isDeleting: true,
              },
            ];
          }

          return oldData;
        },
      );

      return { previousData };
    },
    onError: (error: ApiError, variables, context) => {
      if (context?.previousData) {
        queryClient.setQueryData(exercisesQueryKeys.all, context.previousData);
      }

      handleApiError({
        error,
        errorHandlers: {
          [StatusCodes.BAD_REQUEST]: {
            default: () => {
              toast.error("Exercise not found");
            },
            [apiExercisesErrors.Exercise.UsedInTrainings]:
              variables.onShowAlert,
          },
        },
      });
    },
  });

  return deleteExerciseMutation;
}

export default useDeleteExerciseMutation;
