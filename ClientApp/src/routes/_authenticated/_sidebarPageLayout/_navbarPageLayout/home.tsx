import { createFileRoute } from "@tanstack/react-router";
import { endOfMonth, startOfMonth } from "date-fns";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import HomeNutritionSection from "@/features/home/components/HomeNutritionSection";
import HomeTrainingsSection from "@/features/home/components/HomeTrainingsSection";
import HomeYoutubeSection from "@/features/home/components/HomeYoutubeSection";
import { dailyNutritionOverviewTodaySumQueryOptions } from "@/features/nutrition/overview/queries/useDailyNutritionOverviewsQuery";
import { trainingsOverviewQueryOptions } from "@/features/trainings/overview/queries/trainingsOverviewQueries";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { workoutPlansQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutPlansQueries";
import {
  getCurrentWeekDateRange,
  workoutsWeeklyGoalQueryOptions,
} from "@/features/trainings/workoutPlans/queries/workoutsWeeklyGoalQuery";
import { workoutStreakQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutStreakQuery";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";
import { getDateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/home",
)({
  loader: async () => {
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
        queryClient.ensureQueryData(dailyNutritionOverviewTodaySumQueryOptions()),
        queryClient.ensureQueryData(youtubeQueryOptions.homeRecommendation()),
      ]);
    } catch {
      // Page can still render; sections handle their own loading states.
    }
  },
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <PageCard>
      <PageTitle title="Home" />
      <div className="flex flex-col gap-8">
        <HomeTrainingsSection />
        <div className="grid grid-cols-1 gap-8 @5xl/page-card:grid-cols-2 @5xl/page-card:items-start">
          <div className="min-w-0">
            <HomeNutritionSection />
          </div>
          <div className="min-w-0">
            <HomeYoutubeSection />
          </div>
        </div>
      </div>
    </PageCard>
  );
}
