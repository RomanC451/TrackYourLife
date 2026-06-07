import { StatusCodes } from "http-status-codes";
import { describe, expect, it } from "vitest";

import { goal as goalFixture } from "@/features/nutrition/__tests__/fixtures";
import { GoalType } from "@/services/openapi";

import {
  nutritionGoalsQueryKeys,
  nutritionGoalsQueryOptions,
} from "../nutritionGoalsQuery";

function goal(type: GoalType, value: number) {
  return goalFixture(type, value);
}

describe("nutritionGoalsQueryKeys", () => {
  it("builds a date-scoped query key", () => {
    expect(nutritionGoalsQueryKeys.byDate("2026-06-05")).toEqual([
      "nutritionGoals",
      "2026-06-05",
    ]);
  });
});

describe("nutritionGoalsQueryOptions.byDate select", () => {
  const select = nutritionGoalsQueryOptions.byDate("2026-06-05").select!;

  it("maps goal DTOs into macro targets", () => {
    expect(
      select([
        goal(GoalType.Calories, 2000),
        goal(GoalType.Protein, 150),
        goal(GoalType.Carbohydrates, 220),
        goal(GoalType.Fats, 70),
      ]),
    ).toEqual({
      calories: goal(GoalType.Calories, 2000),
      proteins: goal(GoalType.Protein, 150),
      carbs: goal(GoalType.Carbohydrates, 220),
      fat: goal(GoalType.Fats, 70),
    });
  });

  it("returns undefined when no goals exist", () => {
    expect(select([])).toBeUndefined();
  });
});

describe("nutritionGoalsQueryOptions.byDate retry", () => {
  const retry = nutritionGoalsQueryOptions.byDate("2026-06-05").retry as (
    failureCount: number,
    error: unknown,
  ) => boolean;

  it("does not retry after a 404 response", () => {
    expect(
      retry(0, { status: StatusCodes.NOT_FOUND } as never),
    ).toBe(false);
  });
});
