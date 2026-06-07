import {
  Difficulty,
  ExerciseStatus,
  type ExerciseDto,
  type ExerciseHistoryDto,
  type ExerciseSet,
  type OngoingTrainingDto,
  type TrainingDto,
  type WorkoutHistoryDto,
  type WorkoutPlanDto,
} from "@/services/openapi";

export function exerciseSet(
  id: string,
  overrides: Partial<ExerciseSet> = {},
): ExerciseSet {
  return {
    id,
    name: `Set ${id}`,
    orderIndex: 0,
    count1: 10,
    unit1: "reps",
    ...overrides,
  };
}

export function exercise(
  id: string,
  overrides: Partial<ExerciseDto> = {},
): ExerciseDto {
  return {
    id,
    name: `Exercise ${id}`,
    muscleGroups: ["chest"],
    difficulty: Difficulty.Easy,
    exerciseSets: [exerciseSet(`${id}-set-1`)],
    createdOnUtc: "2026-01-01T09:00:00Z",
    isLoading: false,
    isDeleting: false,
    ...overrides,
  };
}

export function training(
  id: string,
  overrides: Partial<TrainingDto> = {},
): TrainingDto {
  return {
    id,
    name: `Training ${id}`,
    muscleGroups: ["chest"],
    difficulty: Difficulty.Easy,
    duration: 60,
    restSeconds: 60,
    exercises: [],
    createdOnUtc: "2026-01-01T09:00:00Z",
    isLoading: false,
    isDeleting: false,
    ...overrides,
  };
}

export function workoutPlan(
  id: string,
  overrides: Partial<WorkoutPlanDto> = {},
): WorkoutPlanDto {
  return {
    id,
    name: `Plan ${id}`,
    isActive: true,
    workouts: [training("workout-1")],
    createdOnUtc: "2026-01-01T09:00:00Z",
    isLoading: false,
    isDeleting: false,
    ...overrides,
  };
}

export function workoutHistory(
  id: string,
  overrides: Partial<WorkoutHistoryDto> = {},
): WorkoutHistoryDto {
  return {
    id,
    trainingId: "training-1",
    trainingName: "Push day",
    startedOnUtc: "2026-06-05T16:00:00Z",
    finishedOnUtc: "2026-06-05T18:00:00Z",
    durationSeconds: 7200,
    completedExercisesCount: 5,
    totalExercisesCount: 6,
    isLoading: false,
    isDeleting: false,
    ...overrides,
  };
}

export function exerciseHistory(
  id: string,
  overrides: Partial<ExerciseHistoryDto> = {},
): ExerciseHistoryDto {
  return {
    id,
    exerciseId: "exercise-1",
    oldExerciseSets: [exerciseSet(`${id}-old`, { count1: 10 })],
    newExerciseSets: [exerciseSet(`${id}-new`, { count1: 12 })],
    areChangesApplied: true,
    status: ExerciseStatus.Completed,
    createdOnUtc: "2026-06-05T10:00:00Z",
    isLoading: false,
    isDeleting: false,
    ...overrides,
  };
}

export function ongoingTraining(
  overrides: Partial<OngoingTrainingDto> = {},
): OngoingTrainingDto {
  const exercises = overrides.training?.exercises ?? [
    exercise("ex-1", {
      name: "Bench press",
      exerciseSets: [exerciseSet("set-1"), exerciseSet("set-2")],
    }),
    exercise("ex-2", { name: "Squat" }),
  ];

  const trainingData: TrainingDto = {
    ...training("training-1", { exercises }),
    ...overrides.training,
  };

  return {
    id: "ongoing-1",
    training: trainingData,
    exerciseIndex: 0,
    setIndex: 0,
    startedOnUtc: "2026-06-05T10:00:00Z",
    hasNext: true,
    isLastSet: false,
    isFirstSet: true,
    isLastExercise: false,
    isFirstExercise: true,
    completedExerciseIds: [],
    skippedExerciseIds: [],
    isLoading: false,
    isDeleting: false,
    ...overrides,
  };
}
