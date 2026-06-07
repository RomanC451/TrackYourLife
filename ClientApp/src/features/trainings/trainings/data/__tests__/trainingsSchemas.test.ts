import { describe, expect, it } from "vitest";

import { Difficulty } from "@/services/openapi";

import { trainingFormSchema } from "../trainingsSchemas";

const validExercise = {
  id: "ex-1",
  name: "Bench press",
  muscleGroups: ["chest"],
  difficulty: Difficulty.Easy,
  exerciseSets: [
    {
      id: "set-1",
      name: "Set 1",
      orderIndex: 0,
      count1: 10,
      unit1: "reps",
    },
  ],
};

describe("trainingFormSchema", () => {
  it("accepts a valid training form", () => {
    expect(
      trainingFormSchema.parse({
        name: "Push day",
        muscleGroups: ["chest"],
        difficulty: Difficulty.Medium,
        duration: 60,
        restSeconds: 90,
        exercises: [validExercise],
      }),
    ).toMatchObject({
      name: "Push day",
      exercises: [expect.objectContaining({ id: "ex-1" })],
    });
  });

  it("rejects missing required fields", () => {
    const result = trainingFormSchema.safeParse({
      name: "",
      muscleGroups: [],
      difficulty: Difficulty.Easy,
      duration: 60,
      restSeconds: 60,
      exercises: [],
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues.map((issue) => issue.path.join("."))).toEqual(
        expect.arrayContaining(["name", "muscleGroups", "exercises"]),
      );
    }
  });
});
