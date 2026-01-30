import { createFileRoute } from "@tanstack/react-router";
import { subDays } from "date-fns";

import { difficultyDistributionQueryOptions } from "@/features/trainings/overview/queries/useDifficultyDistributionQuery";
import { exercisePerformanceQueryOptions } from "@/features/trainings/overview/queries/useExercisePerformanceQuery";
import { muscleGroupDistributionQueryOptions } from "@/features/trainings/overview/queries/useMuscleGroupDistributionQuery";
import { topExercisesQueryOptions } from "@/features/trainings/overview/queries/useTopExercisesQuery";
import { trainingTemplatesUsageQueryOptions } from "@/features/trainings/overview/queries/useTrainingTemplatesUsageQuery";
import { trainingsOverviewQueryOptions } from "@/features/trainings/overview/queries/useTrainingsOverviewQuery";
import { workoutFrequencyQueryOptions } from "@/features/trainings/overview/queries/useWorkoutFrequencyQuery";
import { workoutDurationHistoryQueryOptions } from "@/features/trainings/overview/queries/useWorkoutDurationHistoryQuery";
import { caloriesBurnedHistoryQueryOptions } from "@/features/trainings/overview/queries/useCaloriesBurnedHistoryQuery";
import { getDateOnly } from "@/lib/date";
import OverviewPage from "@/pages/trainings/OverviewPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/overview",
)({
  loader: async () => {
    // Default date range: last 31 days (matches useDateRange initial state)
    const defaultStartDate = getDateOnly(subDays(new Date(), 31));
    const defaultEndDate = getDateOnly(new Date());

    // Preload all queries with the default date range
    await Promise.all([
      // WorkoutSummaryCards - overview totals
      queryClient.ensureQueryData(
        trainingsOverviewQueryOptions.byDateRange(
          defaultStartDate,
          defaultEndDate,
        ),
      ),
      // WorkoutFrequencyChart - default: last 31 days, "Weekly"
      queryClient.ensureQueryData(
        workoutFrequencyQueryOptions.byFilters(
          defaultStartDate,
          defaultEndDate,
          "Weekly",
        ),
      ),
      // MuscleGroupsChart, DifficultyChart - distribution with default range
      queryClient.ensureQueryData(
        muscleGroupDistributionQueryOptions.byDateRange(
          defaultStartDate,
          defaultEndDate,
        ),
      ),
      queryClient.ensureQueryData(
        difficultyDistributionQueryOptions.byDateRange(
          defaultStartDate,
          defaultEndDate,
        ),
      ),
      // DurationChart and CaloriesChart - default: last 31 days, Weekly, Sum
      queryClient.ensureQueryData(
        workoutDurationHistoryQueryOptions.byFilters(
          defaultStartDate,
          defaultEndDate,
          "Weekly",
          "Sum",
        ),
      ),
      queryClient.ensureQueryData(
        caloriesBurnedHistoryQueryOptions.byFilters(
          defaultStartDate,
          defaultEndDate,
          "Weekly",
          "Sum",
        ),
      ),
      // ExercisePerformanceChart - same default range as useDateRange() (last 31 days), page 1
      queryClient.ensureQueryData(
        exercisePerformanceQueryOptions.byFilters(
          defaultStartDate,
          defaultEndDate,
          null,
          "Sequential",
          1,
          10,
        ),
      ),
      // TopExercisesChart - page 1, default date range
      queryClient.ensureQueryData(
        topExercisesQueryOptions.byPage(
          1,
          10,
          defaultStartDate,
          defaultEndDate,
        ),
      ),
      // TrainingTemplatesChart - default date range
      queryClient.ensureQueryData(
        trainingTemplatesUsageQueryOptions.byDateRange(
          defaultStartDate,
          defaultEndDate,
        ),
      ),
    ]);
  },
  component: RouteComponent,
});

function RouteComponent() {
  return <OverviewPage />;
}
