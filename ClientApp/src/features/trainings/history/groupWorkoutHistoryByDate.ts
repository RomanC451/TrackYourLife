import {
  format,
  isSameWeek,
  isThisWeek,
  isToday,
  isYesterday,
  startOfWeek,
  subWeeks,
} from "date-fns";

import type { WorkoutHistoryDto } from "@/services/openapi";

export type WorkoutHistoryDateGroup = {
  label: string;
  workouts: WorkoutHistoryDto[];
};

function isInPreviousCalendarWeek(date: Date): boolean {
  const lastWeekReference = startOfWeek(subWeeks(new Date(), 1), {
    weekStartsOn: 1,
  });
  return isSameWeek(date, lastWeekReference, { weekStartsOn: 1 });
}

type RecentBucket = "today" | "yesterday" | "thisWeek" | "lastWeek";

function getWorkoutHistoryBucket(
  date: Date,
): RecentBucket | { monthKey: string } {
  if (isToday(date)) return "today";
  if (isYesterday(date)) return "yesterday";
  if (isThisWeek(date, { weekStartsOn: 1 })) return "thisWeek";
  if (isInPreviousCalendarWeek(date)) return "lastWeek";
  return { monthKey: format(date, "yyyy-MM") };
}

/**
 * Groups workouts into Today, Yesterday, This week, Last week, then by calendar month (newest first).
 * Workouts in each group stay sorted by finished time descending.
 */
export function groupWorkoutHistoryByDate(
  workouts: WorkoutHistoryDto[],
): WorkoutHistoryDateGroup[] {
  const sorted = [...workouts].sort(
    (a, b) =>
      new Date(b.finishedOnUtc).getTime() - new Date(a.finishedOnUtc).getTime(),
  );

  const recentBuckets: Record<RecentBucket, WorkoutHistoryDto[]> = {
    today: [],
    yesterday: [],
    thisWeek: [],
    lastWeek: [],
  };
  const byMonth = new Map<string, WorkoutHistoryDto[]>();

  for (const w of sorted) {
    const bucket = getWorkoutHistoryBucket(new Date(w.finishedOnUtc));
    if (typeof bucket === "string") {
      recentBuckets[bucket].push(w);
      continue;
    }
    const monthWorkouts = byMonth.get(bucket.monthKey) ?? [];
    monthWorkouts.push(w);
    byMonth.set(bucket.monthKey, monthWorkouts);
  }

  const out: WorkoutHistoryDateGroup[] = [];
  const recentGroupLabels: { bucket: RecentBucket; label: string }[] = [
    { bucket: "today", label: "Today" },
    { bucket: "yesterday", label: "Yesterday" },
    { bucket: "thisWeek", label: "This week" },
    { bucket: "lastWeek", label: "Last week" },
  ];
  for (const { bucket, label } of recentGroupLabels) {
    const workouts = recentBuckets[bucket];
    if (workouts.length) out.push({ label, workouts });
  }

  const monthKeys = [...byMonth.keys()].sort((a, b) => b.localeCompare(a));
  for (const key of monthKeys) {
    const list = byMonth.get(key)!;
    const label = format(new Date(list[0].finishedOnUtc), "MMMM yyyy");
    out.push({ label, workouts: list });
  }

  return out;
}
