import { describe, expect, it } from "vitest";

import { apiExercisesErrors } from "../apiExercisesErrors";

describe("apiExercisesErrors", () => {
  it("exposes the used-in-trainings error code", () => {
    expect(apiExercisesErrors.Exercise.UsedInTrainings).toBe(
      "Exercise.UsedInTrainings",
    );
  });
});
