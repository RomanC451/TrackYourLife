import { describe, expect, it } from "vitest";

import {
  Difficulty,
  type ExerciseDto,
  type OngoingTrainingDto,
  type TrainingDto,
} from "@/services/openapi";

import {
  applyOngoingWorkoutNavigation,
  goToNextPosition,
  goToPreviousPosition,
  skipCurrentExercise,
} from "../ongoingWorkoutNavigation";

function exercise(id: string, setCount: number): ExerciseDto {
  return {
    id,
    name: id,
    muscleGroups: [],
    difficulty: Difficulty.Easy,
    exerciseSets: Array.from({ length: setCount }, (_, i) => ({
      id: `${id}-set-${i}`,
      name: `Set ${i + 1}`,
      orderIndex: i,
      count1: 10,
      unit1: "reps",
    })),
    createdOnUtc: "2026-01-01T09:00:00Z",
    isLoading: false,
    isDeleting: false,
  };
}

function ongoingTraining(
  overrides: Partial<OngoingTrainingDto> & {
    exercises?: ExerciseDto[];
  } = {},
): OngoingTrainingDto {
  const exercises = overrides.exercises ?? [
    exercise("ex-1", 2),
    exercise("ex-2", 3),
  ];

  const training: TrainingDto = {
    id: "training-1",
    name: "Workout",
    difficulty: Difficulty.Easy,
    muscleGroups: [],
    duration: 60,
    restSeconds: 60,
    exercises,
    createdOnUtc: "2026-01-01T09:00:00Z",
    isLoading: false,
    isDeleting: false,
  };

  return {
    id: "ongoing-1",
    training,
    exerciseIndex: 0,
    setIndex: 0,
    startedOnUtc: "2026-01-01T10:00:00Z",
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

describe("goToNextPosition", () => {
  it("increments set index when not on the last set", () => {
    const result = goToNextPosition(
      ongoingTraining({ setIndex: 0, isLastSet: false }),
    );

    expect(result.setIndex).toBe(1);
    expect(result.exerciseIndex).toBe(0);
  });

  it("moves to the next incomplete exercise on the last set", () => {
    const result = goToNextPosition(
      ongoingTraining({
        exerciseIndex: 0,
        setIndex: 1,
        isLastSet: true,
        isFirstSet: false,
      }),
    );

    expect(result.exerciseIndex).toBe(1);
    expect(result.setIndex).toBe(0);
  });

  it("returns unchanged when the workout is finished", () => {
    const input = ongoingTraining({ finishedOnUtc: "2026-01-01T11:00:00Z" });
    expect(goToNextPosition(input)).toBe(input);
  });

  it("returns unchanged when every exercise is already completed", () => {
    const input = ongoingTraining({
      exerciseIndex: 1,
      setIndex: 2,
      isLastSet: true,
      isLastExercise: true,
      completedExerciseIds: ["ex-1", "ex-2"],
    });
    expect(goToNextPosition(input)).toBe(input);
  });
});

describe("goToPreviousPosition", () => {
  it("decrements set index when not on the first set", () => {
    const result = goToPreviousPosition(
      ongoingTraining({
        setIndex: 1,
        isFirstSet: false,
        isLastSet: false,
      }),
    );

    expect(result.setIndex).toBe(0);
    expect(result.exerciseIndex).toBe(0);
  });

  it("moves to the last set of the previous exercise on the first set", () => {
    const result = goToPreviousPosition(
      ongoingTraining({
        exerciseIndex: 1,
        setIndex: 0,
        isFirstSet: true,
        isFirstExercise: false,
      }),
    );

    expect(result.exerciseIndex).toBe(0);
    expect(result.setIndex).toBe(1);
  });

  it("returns unchanged on the first set of the first exercise", () => {
    const input = ongoingTraining({
      exerciseIndex: 0,
      setIndex: 0,
      isFirstSet: true,
      isFirstExercise: true,
    });
    expect(goToPreviousPosition(input)).toBe(input);
  });

  it("returns unchanged when the workout is finished", () => {
    const input = ongoingTraining({ finishedOnUtc: "2026-01-01T11:00:00Z" });
    expect(goToPreviousPosition(input)).toBe(input);
  });
});

describe("skipCurrentExercise", () => {
  it("moves to the next exercise when skipping the current one", () => {
    const result = skipCurrentExercise(
      ongoingTraining({
        exerciseIndex: 0,
        setIndex: 0,
      }),
    );

    expect(result.exerciseIndex).toBe(1);
    expect(result.setIndex).toBe(0);
  });

  it("wraps to the first incomplete exercise when skipping the last one", () => {
    const result = skipCurrentExercise(
      ongoingTraining({
        exerciseIndex: 1,
        setIndex: 0,
        isFirstExercise: false,
        isLastExercise: true,
      }),
    );

    expect(result.exerciseIndex).toBe(0);
    expect(result.setIndex).toBe(0);
  });

  it("returns unchanged when every exercise is already completed or skipped", () => {
    const input = ongoingTraining({
      exerciseIndex: 0,
      completedExerciseIds: ["ex-1"],
      skippedExerciseIds: ["ex-2"],
    });
    expect(skipCurrentExercise(input)).toBe(input);
  });

  it("returns unchanged when the workout is finished", () => {
    const input = ongoingTraining({ finishedOnUtc: "2026-01-01T11:00:00Z" });
    expect(skipCurrentExercise(input)).toBe(input);
  });
});

describe("applyOngoingWorkoutNavigation", () => {
  it("dispatches next, previous, and skip actions", () => {
    const base = ongoingTraining({ setIndex: 0, isLastSet: false });

    expect(applyOngoingWorkoutNavigation(base, "next").setIndex).toBe(1);
    expect(
      applyOngoingWorkoutNavigation(
        ongoingTraining({ setIndex: 1, isFirstSet: false }),
        "previous",
      ).setIndex,
    ).toBe(0);
    expect(applyOngoingWorkoutNavigation(base, "skip").exerciseIndex).toBe(1);
  });
});
