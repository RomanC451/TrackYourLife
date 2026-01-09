import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { OngoingTrainingDto, OngoingTrainingsApi } from "@/services/openapi";

import { exercisesQueryKeys } from "../../exercises/queries/exercisesQuery";
import { trainingsQueryKeys } from "../../trainings/queries/trainingsQueries";
import { ongoingTrainingsQueryKeys } from "../queries/ongoingTrainingsQuery";

const ongoingTrainingsApi = new OngoingTrainingsApi();

type Variables = {
  ongoingTraining: OngoingTrainingDto;
};

/**
 * Checks if all exercises in the training are completed or skipped
 */
export function areAllExercisesCompletedOrSkipped(
  ongoingTraining: OngoingTrainingDto,
): { allCompleted: boolean; incompleteExercises: string[] } {
  const allExerciseIds =
    ongoingTraining.training?.exercises?.map((ex) => ex.id) || [];
  const completedIds = ongoingTraining.completedExerciseIds || [];
  const skippedIds = ongoingTraining.skippedExerciseIds || [];
  const completedOrSkippedIds = new Set([...completedIds, ...skippedIds]);

  const incompleteExercises = allExerciseIds.filter(
    (id) => !completedOrSkippedIds.has(id),
  );

  return {
    allCompleted: incompleteExercises.length === 0,
    incompleteExercises,
  };
}

const useFinishOngoingTrainingMutation = () => {
  const finishOngoingTrainingMutation = useCustomMutation({
    mutationFn: ({ ongoingTraining }: Variables) => {
      // Frontend validation - check if all exercises are completed or skipped
      const { allCompleted, incompleteExercises } =
        areAllExercisesCompletedOrSkipped(ongoingTraining);

      if (!allCompleted) {
        const incompleteNames = incompleteExercises
          .map((id) => {
            const exercise = ongoingTraining.training?.exercises?.find(
              (ex) => ex.id === id,
            );
            return exercise?.name || id;
          })
          .join(", ");

        throw new Error(
          `Cannot finish training. The following exercises are not completed or skipped: ${incompleteNames}`,
        );
      }

      return ongoingTrainingsApi.finishOngoingTraining(ongoingTraining.id);
    },

    meta: {
      invalidateQueries: [
        ongoingTrainingsQueryKeys.active,
        exercisesQueryKeys.all,
        trainingsQueryKeys.all,
      ],
    },

    onSuccess: () => {
      queryClient.setQueryData(
        ongoingTrainingsQueryKeys.active,
        (oldData: OngoingTrainingDto) => ({
          ...oldData,
          finishedOnUtc: new Date().toISOString(),
        }),
      );
    },
  });

  return finishOngoingTrainingMutation;
};

export default useFinishOngoingTrainingMutation;
