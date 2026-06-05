import { describe, expect, it } from "vitest";

import { Difficulty, type TrainingDto } from "@/services/openapi";

import {
  selectTrainingFromList,
  TrainingNotFoundError,
} from "../trainingsQueries";

function training(id: string, name: string): TrainingDto {
  return {
    id,
    name,
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

describe("selectTrainingFromList", () => {
  it("returns the matching training", () => {
    const trainings = [training("a", "Alpha"), training("b", "Beta")];
    expect(selectTrainingFromList(trainings, "b")?.name).toBe("Beta");
  });

  it("returns undefined when the list or id is missing", () => {
    expect(selectTrainingFromList(undefined, "a")).toBeUndefined();
    expect(selectTrainingFromList([training("a", "Alpha")], "missing")).toBeUndefined();
  });
});

describe("TrainingNotFoundError", () => {
  it("includes the training id in the message", () => {
    const error = new TrainingNotFoundError("workout-1");
    expect(error).toBeInstanceOf(Error);
    expect(error.trainingId).toBe("workout-1");
    expect(error.message).toContain("workout-1");
  });
});
