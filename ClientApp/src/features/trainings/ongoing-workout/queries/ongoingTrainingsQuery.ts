import { queryOptions } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";

import { queryClient } from "@/queryClient";
import {
  OngoingTrainingDto,
  OngoingTrainingsApi,
} from "@/services/openapi/api";
import { retryQueryExcept } from "@/services/openapi/retry";

const ongoingTrainingsApi = new OngoingTrainingsApi();

export const ongoingTrainingsQueryKeys = {
  all: ["ongoingTrainings"] as const,
  active: ["ongoingTrainings", "active"] as const,
  byId: (id: string) => ["ongoingTrainings", id] as const,
};

export const ongoingTrainingsQueryOptions = {
  active: queryOptions({
    queryKey: ongoingTrainingsQueryKeys.active,
    queryFn: () =>
      ongoingTrainingsApi.getActiveOngoingTraining().then((res) => res.data),
    retry: (failureCount, error) =>
      retryQueryExcept(failureCount, error, {
        max_retries: 3,
        checkedCodes: {
          [StatusCodes.NOT_FOUND]: () => {
            queryClient.setQueryData(ongoingTrainingsQueryKeys.active, null);
            return null;
          },
        },
      }),
  }),
  byId: (id: string) =>
    queryOptions({
      queryKey: ongoingTrainingsQueryKeys.byId(id),
      queryFn: () =>
        ongoingTrainingsApi.getOngoingTrainingById(id).then((res) => res.data),
    }),
};

export function setActiveOngoingTrainingQueryDataByAction({
  action,
}: {
  action: "next" | "previous" | "skip";
}) {
  queryClient.setQueryData(
    ongoingTrainingsQueryKeys.active,
    (oldData: OngoingTrainingDto) => {
      let updatedData: OngoingTrainingDto;
      if (action === "next") {
        updatedData = next(oldData);
      } else if (action === "previous") {
        updatedData = previous(oldData);
      } else {
        updatedData = skipExercise(oldData);
      }
      return {
        ...updatedData,
        isLoading: true,
      };
    },
  );
}

function next(ongoingTraining: OngoingTrainingDto) {
  if (ongoingTraining.finishedOnUtc) {
    return ongoingTraining;
  }

  if (!ongoingTraining.hasNext) {
    return ongoingTraining;
  }

  if (ongoingTraining.isLastSet) {
    return {
      ...ongoingTraining,
      exerciseIndex: ongoingTraining.exerciseIndex + 1,
      setIndex: 0,
    };
  }

  return {
    ...ongoingTraining,
    setIndex: ongoingTraining.setIndex + 1,
  };
}

function previous(ongoingTraining: OngoingTrainingDto) {
  if (ongoingTraining.finishedOnUtc) {
    return ongoingTraining;
  }

  if (ongoingTraining.isFirstSet && ongoingTraining.isFirstExercise) {
    return ongoingTraining;
  }

  if (ongoingTraining.isFirstSet) {
    return {
      ...ongoingTraining,
      exerciseIndex: ongoingTraining.exerciseIndex - 1,
      setIndex:
        ongoingTraining.training.exercises[ongoingTraining.exerciseIndex - 1]
          .exerciseSets.length - 1,
    };
  }

  return {
    ...ongoingTraining,
    setIndex: ongoingTraining.setIndex - 1,
  };
}

function skipExercise(ongoingTraining: OngoingTrainingDto) {
  if (ongoingTraining.finishedOnUtc) {
    return ongoingTraining;
  }

  // Get the list of completed or skipped exercise IDs
  const completedExerciseIds = new Set(
    ongoingTraining.completedExerciseIds || [],
  );
  const skippedExerciseIds = new Set(ongoingTraining.skippedExerciseIds || []);
  const completedOrSkippedExerciseIds = new Set([
    ...completedExerciseIds,
    ...skippedExerciseIds,
  ]);

  // Get the current exercise ID and add it to the skipped set
  const currentExercise =
    ongoingTraining.training?.exercises?.[ongoingTraining.exerciseIndex];
  if (currentExercise) {
    completedOrSkippedExerciseIds.add(currentExercise.id);
  }

  const exercises = ongoingTraining.training?.exercises || [];

  // First, try to find the next incomplete exercise after the current one
  let nextIncompleteExercise = exercises
    .slice(ongoingTraining.exerciseIndex + 1)
    .find((exercise) => !completedOrSkippedExerciseIds.has(exercise.id));

  let nextExerciseIndex: number;

  if (nextIncompleteExercise) {
    // Found an incomplete exercise after the current one
    nextExerciseIndex = exercises.indexOf(nextIncompleteExercise);
  } else {
    // No incomplete exercises after current, wrap around to find the first incomplete exercise
    nextIncompleteExercise = exercises
      .slice(0, ongoingTraining.exerciseIndex + 1)
      .find((exercise) => !completedOrSkippedExerciseIds.has(exercise.id));

    if (!nextIncompleteExercise) {
      // All exercises are completed or skipped, stay on current exercise
      return ongoingTraining;
    }

    nextExerciseIndex = exercises.indexOf(nextIncompleteExercise);
  }

  if (nextExerciseIndex < 0) {
    return ongoingTraining;
  }

  // Move to the next incomplete exercise, first set (matching server's SkipExercise behavior)
  return {
    ...ongoingTraining,
    exerciseIndex: nextExerciseIndex,
    setIndex: 0,
  };
}
