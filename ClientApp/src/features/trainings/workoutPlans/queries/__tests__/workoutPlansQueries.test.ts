import { describe, expect, it } from "vitest";

import { Difficulty, type TrainingDto, type WorkoutPlanDto } from "@/services/openapi";

import { workoutPlansQueryKeys, workoutPlansQueryOptions } from "../workoutPlansQueries";

function training(id: string): TrainingDto {
  return {
    id,
    name: id,
    muscleGroups: [],
    difficulty: Difficulty.Easy,
    duration: 60,
    restSeconds: 60,
    exercises: [],
    createdOnUtc: "2026-01-01T09:00:00Z",
    isLoading: false,
    isDeleting: false,
  };
}

describe("workoutPlansQueryKeys", () => {
  it("builds keys for list, active, next workout, and by id", () => {
    expect(workoutPlansQueryKeys.all).toEqual(["workoutPlans"]);
    expect(workoutPlansQueryKeys.active).toEqual(["workoutPlans", "active"]);
    expect(workoutPlansQueryKeys.nextWorkout).toEqual([
      "workoutPlans",
      "nextWorkout",
    ]);
    expect(workoutPlansQueryKeys.byId("plan-1")).toEqual([
      "workoutPlans",
      "plan-1",
    ]);
  });
});

describe("workoutPlansQueryOptions selectors", () => {
  const plans: WorkoutPlanDto[] = [
    {
      id: "plan-1",
      name: "Plan A",
      isActive: true,
      workouts: [training("workout-a"), training("workout-b")],
      createdOnUtc: "2026-01-01T09:00:00Z",
      isLoading: false,
      isDeleting: false,
    },
    {
      id: "plan-2",
      name: "Plan B",
      isActive: false,
      workouts: [training("workout-c")],
      createdOnUtc: "2026-01-02T09:00:00Z",
      isLoading: false,
      isDeleting: false,
    },
  ];

  it("selects a plan by id", () => {
    const options = workoutPlansQueryOptions.byId("plan-2");
    expect(options.select?.(plans).id).toBe("plan-2");
  });

  it("filters plans that contain a training id", () => {
    const options = workoutPlansQueryOptions.containsTrainingId("workout-b");
    expect(options.select?.(plans).map((plan) => plan.id)).toEqual(["plan-1"]);
  });
});
