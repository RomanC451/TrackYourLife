import { describe, expect, it, vi } from "vitest";

import { Difficulty, type ExerciseSet } from "@/services/openapi";

import { createDefaultExerciseSet, getDifficultyColor } from "../exercisesUtils";

vi.mock("uuid", () => ({
  v4: () => "generated-uuid",
}));

describe("getDifficultyColor", () => {
  it("returns the matching tailwind classes per difficulty", () => {
    expect(getDifficultyColor(Difficulty.Easy)).toContain("green");
    expect(getDifficultyColor(Difficulty.Medium)).toContain("orange");
    expect(getDifficultyColor(Difficulty.Hard)).toContain("red");
  });

  it("falls back to gray for unknown values", () => {
    expect(getDifficultyColor("Unknown" as Difficulty)).toContain("gray");
  });
});

describe("createDefaultExerciseSet", () => {
  it("creates the first set with default weight and reps units", () => {
    expect(createDefaultExerciseSet([])).toEqual({
      id: "generated-uuid",
      name: "Set 1",
      orderIndex: 0,
      count1: -1,
      unit1: "kg",
      count2: -1,
      unit2: "reps",
    });
  });

  it("copies the previous set and clears tracked values", () => {
    const existing: ExerciseSet = {
      id: "set-1",
      name: "Set 1",
      orderIndex: 0,
      count1: 50,
      unit1: "kg",
      count2: 10,
      unit2: "reps",
    };

    expect(createDefaultExerciseSet([existing])).toEqual({
      ...existing,
      id: "generated-uuid",
      name: "Set 2",
      count1: -1,
      count2: -1,
    });
  });

  it("omits count2 when the previous set had no secondary count", () => {
    const existing: ExerciseSet = {
      id: "set-1",
      name: "Set 1",
      orderIndex: 0,
      count1: 60,
      unit1: "seconds",
    };

    expect(createDefaultExerciseSet([existing])).toEqual({
      ...existing,
      id: "generated-uuid",
      name: "Set 2",
      count1: -1,
    });
  });
});
