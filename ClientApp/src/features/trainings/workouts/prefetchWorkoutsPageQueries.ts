import type { QueryClient } from "@tanstack/react-query";
import { endOfMonth, startOfMonth } from "date-fns";

import { getDateOnly } from "@/lib/date";

import { ongoingTrainingsQueryOptions } from "../ongoing-workout/queries/ongoingTrainingsQuery";
import { trainingsOverviewQueryOptions } from "../overview/queries/trainingsOverviewQueries";
import { trainingsQueryOptions } from "../trainings/queries/trainingsQueries";
import { workoutPlansQueryOptions } from "../workoutPlans/queries/workoutPlansQueries";
import {
  getCurrentWeekDateRange,
  workoutsWeeklyGoalQueryOptions,
} from "../workoutPlans/queries/workoutsWeeklyGoalQuery";
import { workoutStreakQueryOptions } from "../workoutPlans/queries/workoutStreakQuery";

export async function prefetchWorkoutsPageQueries(
  queryClient: QueryClient,
): Promise<void> {
  const { weekStart, weekEnd } = getCurrentWeekDateRange();
  const today = new Date();
  const currentMonthStart = getDateOnly(startOfMonth(today));
  const currentMonthEnd = getDateOnly(endOfMonth(today));

  const results = await Promise.allSettled([
    queryClient.ensureQueryData(trainingsQueryOptions.all),
    queryClient.ensureQueryData(ongoingTrainingsQueryOptions.active),
    queryClient.ensureQueryData(workoutPlansQueryOptions.all),
    queryClient.ensureQueryData(
      trainingsOverviewQueryOptions.byDateRange(weekStart, weekEnd),
    ),
    queryClient.ensureQueryData(
      trainingsOverviewQueryOptions.byDateRange(
        currentMonthStart,
        currentMonthEnd,
      ),
    ),
    queryClient.ensureQueryData(workoutsWeeklyGoalQueryOptions.current()),
    queryClient.ensureQueryData(workoutStreakQueryOptions.current()),
    queryClient.ensureQueryData(workoutPlansQueryOptions.nextWorkout),
  ]);

  const failures = results.filter((result) => result.status === "rejected");
  if (failures.length > 0 && import.meta.env.DEV) {
    console.error(
      "[prefetchWorkoutsPageQueries] failed:",
      failures.map((failure) => (failure as PromiseRejectedResult).reason),
    );
  }
}
