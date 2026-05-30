import { createFileRoute } from "@tanstack/react-router";
import { endOfMonth, startOfMonth } from "date-fns";

import { dailyNutritionOverviewsQueryOptions } from "@/features/nutrition/overview/queries/useDailyNutritionOverviewsQuery";
import { trainingsOverviewQueryOptions } from "@/features/trainings/overview/queries/useTrainingsOverviewQuery";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { workoutPlansQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutPlansQueries";
import {
  getCurrentWeekDateRange,
  workoutsWeeklyGoalQueryOptions,
} from "@/features/trainings/workoutPlans/queries/workoutsWeeklyGoalQuery";
import { workoutStreakQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutStreakQuery";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";
import HomePage from "@/pages/HomePage";
import { queryClient } from "@/queryClient";
import { getDateOnly } from "@/lib/date";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/home",
)({
  loader: async () => {
    const today = getDateOnly(new Date());
    const { weekStart, weekEnd } = getCurrentWeekDateRange();
    const currentMonthStart = getDateOnly(startOfMonth(new Date()));
    const currentMonthEnd = getDateOnly(endOfMonth(new Date()));

    try {
      await Promise.all([
        queryClient.ensureQueryData(ongoingTrainingsQueryOptions.active),
        queryClient.ensureQueryData(workoutPlansQueryOptions.all),
        queryClient.ensureQueryData(workoutPlansQueryOptions.active),
        queryClient.ensureQueryData(workoutPlansQueryOptions.nextWorkout),
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
        queryClient.ensureQueryData(
          dailyNutritionOverviewsQueryOptions.byDateRange(
            today,
            today,
            "Daily",
            "Sum",
          ),
        ),
        queryClient.ensureQueryData(youtubeQueryOptions.homeRecommendation()),
      ]);
    } catch {
      // Page can still render; sections handle their own loading states.
    }
  },
  component: HomePage,
});
