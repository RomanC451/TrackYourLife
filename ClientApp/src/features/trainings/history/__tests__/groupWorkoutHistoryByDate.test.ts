import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import type { WorkoutHistoryDto } from "@/services/openapi";

import { workoutHistory as workoutHistoryFixture } from "@/features/trainings/__tests__/fixtures";

import { groupWorkoutHistoryByDate } from "../groupWorkoutHistoryByDate";

function workout(id: string, finishedOnUtc: string): WorkoutHistoryDto {
  return workoutHistoryFixture(id, { finishedOnUtc });
}

describe("groupWorkoutHistoryByDate", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("groups workouts into Today, Yesterday, This week, Last week, and months", () => {
    const groups = groupWorkoutHistoryByDate([
      workout("today", "2026-06-05T18:00:00Z"),
      workout("yesterday", "2026-06-04T10:00:00Z"),
      workout("this-week", "2026-06-03T10:00:00Z"),
      workout("last-week", "2026-05-28T10:00:00Z"),
      workout("older", "2026-04-15T10:00:00Z"),
    ]);

    expect(groups.map((g) => g.label)).toEqual([
      "Today",
      "Yesterday",
      "This week",
      "Last week",
      "April 2026",
    ]);
    expect(groups[0].workouts.map((w) => w.id)).toEqual(["today"]);
    expect(groups[4].workouts.map((w) => w.id)).toEqual(["older"]);
  });

  it("sorts workouts within each group by finished time descending", () => {
    const groups = groupWorkoutHistoryByDate([
      workout("later", "2026-06-05T20:00:00Z"),
      workout("earlier", "2026-06-05T08:00:00Z"),
    ]);

    expect(groups[0].workouts.map((w) => w.id)).toEqual(["later", "earlier"]);
  });

  it("omits empty groups", () => {
    const groups = groupWorkoutHistoryByDate([
      workout("today", "2026-06-05T18:00:00Z"),
    ]);

    expect(groups).toEqual([{ label: "Today", workouts: [expect.any(Object)] }]);
  });
});
