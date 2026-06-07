import { describe, expect, it } from "vitest";

import { Difficulty } from "@/services/openapi";

import {
  exerciseFormSchema,
  exerciseSetChangesSchema,
  exerciseSetType,
  exerciseSetTypes,
} from "../exercisesSchemas";

const validSet = {
  id: "set-1",
  name: "Set 1",
  orderIndex: 0,
  count1: 10,
  unit1: "reps",
};

describe("exerciseFormSchema", () => {
  it("accepts a valid exercise form", () => {
    expect(
      exerciseFormSchema.parse({
        id: "ex-1",
        name: "Squat",
        muscleGroups: ["legs"],
        difficulty: Difficulty.Hard,
        videoUrl: "",
        pictureUrl: "",
        exerciseSets: [validSet],
      }),
    ).toMatchObject({ name: "Squat" });
  });

  it("rejects invalid video URLs", () => {
    const result = exerciseFormSchema.safeParse({
      id: "ex-1",
      name: "Squat",
      muscleGroups: ["legs"],
      difficulty: Difficulty.Easy,
      videoUrl: "not-a-url",
      exerciseSets: [validSet],
    });

    expect(result.success).toBe(false);
  });
});

describe("exerciseSetChangesSchema", () => {
  it("accepts a list of replacement sets", () => {
    expect(
      exerciseSetChangesSchema.parse({
        newSets: [validSet],
      }),
    ).toEqual({ newSets: [validSet] });
  });
});

describe("exercise set type constants", () => {
  it("exposes the supported set types", () => {
    expect(exerciseSetTypes.map((t) => t.value)).toEqual([
      exerciseSetType.Weight,
      exerciseSetType.Time,
      exerciseSetType.Bodyweight,
      exerciseSetType.Distance,
    ]);
  });
});
