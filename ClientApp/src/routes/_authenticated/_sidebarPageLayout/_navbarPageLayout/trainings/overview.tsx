import { createFileRoute } from "@tanstack/react-router";
import { endOfWeek, startOfWeek, subDays } from "date-fns";

import { exercisePerformanceQueryOptions } from "@/features/trainings/overview/queries/useExercisePerformanceQuery";
import { topExercisesQueryOptions } from "@/features/trainings/overview/queries/useTopExercisesQuery";
import { trainingTemplatesUsageQueryOptions } from "@/features/trainings/overview/queries/useTrainingTemplatesUsageQuery";
import { trainingsOverviewQueryOptions } from "@/features/trainings/overview/queries/useTrainingsOverviewQuery";
import { workoutHistoryQueryOptions } from "@/features/trainings/overview/queries/useWorkoutHistoryQuery";
import { getDateOnly } from "@/lib/date";
import OverviewPage from "@/pages/trainings/OverviewPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/overview",
)({
  loader: async () => {
    // Default date range: last 31 days (used by WorkoutFrequencyChart, DurationChart and CaloriesChart)
    const defaultStartDate = getDateOnly(subDays(new Date(), 31));
    const defaultEndDate = getDateOnly(new Date());

    // Default date range for ExercisePerformanceChart: current week
    const weekStartDate = getDateOnly(startOfWeek(new Date(), { weekStartsOn: 1 }));
    const weekEndDate = getDateOnly(endOfWeek(new Date(), { weekStartsOn: 1 }));

    // Preload all queries with their default values
    await Promise.all([
      // WorkoutSummaryCards, MuscleGroupsChart, DifficultyChart - no date range
      queryClient.ensureQueryData(
        trainingsOverviewQueryOptions.byDateRange(null, null, "Daily"),
      ),
      // WorkoutFrequencyChart - default: last 31 days, "Weekly"
      queryClient.ensureQueryData(
        trainingsOverviewQueryOptions.byDateRange(
          defaultStartDate,
          defaultEndDate,
          "Weekly",
        ),
      ),
      // DurationChart and CaloriesChart - default: last 31 days
      queryClient.ensureQueryData(
        workoutHistoryQueryOptions.byDateRange(defaultStartDate, defaultEndDate),
      ),
      // ExercisePerformanceChart - default: current week, page 1
      queryClient.ensureQueryData(
        exercisePerformanceQueryOptions.byFilters(
          weekStartDate,
          weekEndDate,
          null,
          "Sequential",
          1,
          10,
        ),
      ),
      // TopExercisesChart - page 1
      queryClient.ensureQueryData(topExercisesQueryOptions.byPage(1, 10)),
      // TrainingTemplatesChart - all templates
      queryClient.ensureQueryData(trainingTemplatesUsageQueryOptions.all),
    ]);
  },
  component: RouteComponent,
});

function RouteComponent() {
  return <OverviewPage />;
}
