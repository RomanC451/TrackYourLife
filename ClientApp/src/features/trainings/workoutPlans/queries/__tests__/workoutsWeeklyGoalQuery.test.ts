import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { endOfWeek, startOfWeek } from "date-fns";
import { StatusCodes } from "http-status-codes";

import { getDateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import { createQueryFnContext } from "@/test/queryFnContext";
import { GoalType } from "@/services/openapi";

const { mockGetGoal } = vi.hoisted(() => ({
  mockGetGoal: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockGoalsApi {
    getGoal = mockGetGoal;
  }
  return { ...actual, GoalsApi: MockGoalsApi };
});

import {
  getCurrentWeekDateRange,
  workoutsWeeklyGoalQueryKeys,
  workoutsWeeklyGoalQueryOptions,
} from "../workoutsWeeklyGoalQuery";
describe("getCurrentWeekDateRange", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("returns Monday through Sunday for the current week", () => {
    const today = new Date();
    expect(getCurrentWeekDateRange()).toEqual({
      weekStart: getDateOnly(startOfWeek(today, { weekStartsOn: 1 })),
      weekEnd: getDateOnly(endOfWeek(today, { weekStartsOn: 1 })),
    });
  });
});

describe("workoutsWeeklyGoalQueryKeys", () => {
  it("scopes goals by date", () => {
    expect(workoutsWeeklyGoalQueryKeys.byDate("2026-06-05")).toEqual([
      "workoutsWeeklyGoal",
      "2026-06-05",
    ]);
  });
});

describe("workoutsWeeklyGoalQueryOptions.current", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("returns the current weekly goal", async () => {
    mockGetGoal.mockResolvedValue({
      data: { id: "goal-1", target: 4 },
    });

    const result = await workoutsWeeklyGoalQueryOptions.current().queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: workoutsWeeklyGoalQueryOptions.current().queryKey,
      }),
    );
    expect(mockGetGoal).toHaveBeenCalledWith(GoalType.Workouts, "2026-06-05");
    expect(result).toEqual({ id: "goal-1", target: 4 });
  });

  it("returns null when no goal exists", async () => {
    mockGetGoal.mockRejectedValue({
      response: { status: StatusCodes.NOT_FOUND },
    });

    const result = await workoutsWeeklyGoalQueryOptions.current().queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: workoutsWeeklyGoalQueryOptions.current().queryKey,
      }),
    );
    expect(result).toBeNull();
  });

  it("rethrows unexpected errors", async () => {
    const serverError = new Error("server down");
    mockGetGoal.mockRejectedValue(serverError);

    await expect(
      workoutsWeeklyGoalQueryOptions.current().queryFn!(
        createQueryFnContext({
          client: queryClient,
          queryKey: workoutsWeeklyGoalQueryOptions.current().queryKey,
        }),
      ),
    ).rejects.toThrow("server down");
  });
});