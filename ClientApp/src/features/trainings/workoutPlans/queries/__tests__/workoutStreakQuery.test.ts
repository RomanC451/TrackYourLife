import { describe, expect, it } from "vitest";

import { workoutStreakQueryKeys } from "../workoutStreakQuery";

describe("workoutStreakQueryKeys", () => {
  it("uses a stable root key", () => {
    expect(workoutStreakQueryKeys.all).toEqual(["workoutStreak"]);
  });
});
