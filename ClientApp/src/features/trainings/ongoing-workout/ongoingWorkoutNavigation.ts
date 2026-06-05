import type { ExerciseDto, OngoingTrainingDto } from "@/services/openapi";

export type OngoingWorkoutNavigationAction = "next" | "previous" | "skip";

export function applyOngoingWorkoutNavigation(
  ongoingTraining: OngoingTrainingDto,
  action: OngoingWorkoutNavigationAction,
): OngoingTrainingDto {
  switch (action) {
    case "next":
      return goToNextPosition(ongoingTraining);
    case "previous":
      return goToPreviousPosition(ongoingTraining);
    case "skip":
      return skipCurrentExercise(ongoingTraining);
  }
}

/** Advance to the next set, or the next incomplete exercise on the last set. */
export function goToNextPosition(
  ongoingTraining: OngoingTrainingDto,
): OngoingTrainingDto {
  if (ongoingTraining.finishedOnUtc) {
    return ongoingTraining;
  }

  if (!ongoingTraining.isLastSet) {
    return {
      ...ongoingTraining,
      setIndex: ongoingTraining.setIndex + 1,
    };
  }

  const exercises = ongoingTraining.training?.exercises ?? [];
  const completedOrSkippedExerciseIds = getCompletedOrSkippedExerciseIds(
    ongoingTraining,
  );

  const nextExerciseIndex = findNextIncompleteExerciseIndex(
    exercises,
    ongoingTraining.exerciseIndex,
    completedOrSkippedExerciseIds,
  );

  if (nextExerciseIndex === null) {
    return ongoingTraining;
  }

  return {
    ...ongoingTraining,
    exerciseIndex: nextExerciseIndex,
    setIndex: 0,
  };
}

/** Move to the previous set, or the last set of the previous exercise. */
export function goToPreviousPosition(
  ongoingTraining: OngoingTrainingDto,
): OngoingTrainingDto {
  if (ongoingTraining.finishedOnUtc) {
    return ongoingTraining;
  }

  if (ongoingTraining.isFirstSet && ongoingTraining.isFirstExercise) {
    return ongoingTraining;
  }

  if (ongoingTraining.isFirstSet) {
    const previousExercise =
      ongoingTraining.training.exercises[ongoingTraining.exerciseIndex - 1];
    return {
      ...ongoingTraining,
      exerciseIndex: ongoingTraining.exerciseIndex - 1,
      setIndex: previousExercise.exerciseSets.length - 1,
    };
  }

  return {
    ...ongoingTraining,
    setIndex: ongoingTraining.setIndex - 1,
  };
}

/** Skip the current exercise and move to the next incomplete one. */
export function skipCurrentExercise(
  ongoingTraining: OngoingTrainingDto,
): OngoingTrainingDto {
  if (ongoingTraining.finishedOnUtc) {
    return ongoingTraining;
  }

  const completedOrSkippedExerciseIds = getCompletedOrSkippedExerciseIds(
    ongoingTraining,
  );
  const currentExercise =
    ongoingTraining.training?.exercises?.[ongoingTraining.exerciseIndex];
  if (currentExercise) {
    completedOrSkippedExerciseIds.add(currentExercise.id);
  }

  const exercises = ongoingTraining.training?.exercises ?? [];
  const nextExerciseIndex = findNextIncompleteExerciseIndex(
    exercises,
    ongoingTraining.exerciseIndex,
    completedOrSkippedExerciseIds,
  );

  if (nextExerciseIndex === null) {
    return ongoingTraining;
  }

  return {
    ...ongoingTraining,
    exerciseIndex: nextExerciseIndex,
    setIndex: 0,
  };
}

function getCompletedOrSkippedExerciseIds(
  ongoingTraining: OngoingTrainingDto,
): Set<string> {
  return new Set([
    ...(ongoingTraining.completedExerciseIds ?? []),
    ...(ongoingTraining.skippedExerciseIds ?? []),
  ]);
}

function findNextIncompleteExerciseIndex(
  exercises: ExerciseDto[],
  currentExerciseIndex: number,
  completedOrSkippedExerciseIds: Set<string>,
): number | null {
  const nextAfterCurrent = exercises
    .slice(currentExerciseIndex + 1)
    .find((exercise) => !completedOrSkippedExerciseIds.has(exercise.id));

  if (nextAfterCurrent) {
    return exercises.indexOf(nextAfterCurrent);
  }

  const wrapped = exercises
    .slice(0, currentExerciseIndex + 1)
    .find((exercise) => !completedOrSkippedExerciseIds.has(exercise.id));

  if (!wrapped) {
    return null;
  }

  const index = exercises.indexOf(wrapped);
  return index < 0 ? null : index;
}
