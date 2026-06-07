import { describe, expect, it } from "vitest";

import { exerciseHistoryQueryKeys } from "../exerciseHistoryQuery";

describe("exerciseHistoryQueryKeys", () => {
  it("scopes history by exercise id", () => {
    expect(exerciseHistoryQueryKeys.byExerciseId("ex-1")).toEqual([
      "exerciseHistory",
      "ex-1",
    ]);
  });
});
