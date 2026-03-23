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

const lastWeekReference = startOfWeek(subWeeks(new Date(), 1), {
  weekStartsOn: 1,
});

function isInPreviousCalendarWeek(date: Date): boolean {
  return isSameWeek(date, lastWeekReference, { weekStartsOn: 1 });
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
      new Date(b.finishedOnUtc).getTime() -
      new Date(a.finishedOnUtc).getTime(),
  );

  const today: WorkoutHistoryDto[] = [];
  const yesterday: WorkoutHistoryDto[] = [];
  const thisWeek: WorkoutHistoryDto[] = [];
  const lastWeek: WorkoutHistoryDto[] = [];
  const byMonth = new Map<string, WorkoutHistoryDto[]>();

  for (const w of sorted) {
    const d = new Date(w.finishedOnUtc);
    if (isToday(d)) {
      today.push(w);
    } else if (isYesterday(d)) {
      yesterday.push(w);
    } else if (isThisWeek(d, { weekStartsOn: 1 })) {
      thisWeek.push(w);
    } else if (isInPreviousCalendarWeek(d)) {
      lastWeek.push(w);
    } else {
      const key = format(d, "yyyy-MM");
      const existing = byMonth.get(key);
      if (existing) {
        existing.push(w);
      } else {
        byMonth.set(key, [w]);
      }
    }
  }

  const out: WorkoutHistoryDateGroup[] = [];
  if (today.length) out.push({ label: "Today", workouts: today });
  if (yesterday.length) out.push({ label: "Yesterday", workouts: yesterday });
  if (thisWeek.length) out.push({ label: "This week", workouts: thisWeek });
  if (lastWeek.length) out.push({ label: "Last week", workouts: lastWeek });

  const monthKeys = [...byMonth.keys()].sort((a, b) => b.localeCompare(a));
  for (const key of monthKeys) {
    const list = byMonth.get(key)!;
    const label = format(new Date(list[0].finishedOnUtc), "MMMM yyyy");
    out.push({ label, workouts: list });
  }

  return out;
}
