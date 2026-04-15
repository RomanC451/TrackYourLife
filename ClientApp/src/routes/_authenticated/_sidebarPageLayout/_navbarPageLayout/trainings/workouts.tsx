import { createFileRoute, Outlet, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";

import { router } from "@/App";
import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import WorkoutPlansTopSection from "@/features/trainings/workoutPlans/components/WorkoutPlansTopSection";
import { workoutPlansQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutPlansQueries";
import {
  getCurrentWeekDateRange,
  workoutsWeeklyGoalQueryOptions,
} from "@/features/trainings/workoutPlans/queries/workoutsWeeklyGoalQuery";
import WorkoutsList from "@/features/trainings/trainings/components/trainingsList/TrainingsList";
import { trainingsOverviewQueryOptions } from "@/features/trainings/overview/queries/useTrainingsOverviewQuery";
import { trainingsQueryOptions } from "@/features/trainings/trainings/queries/trainingsQueries";
import { queryClient } from "@/queryClient";
import { getDateOnly } from "@/lib/date";
import { endOfMonth, startOfMonth } from "date-fns";
import { workoutStreakQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutStreakQuery";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts",
)({
  loader: async () => {
    const { weekStart, weekEnd } = getCurrentWeekDateRange();
    const today = new Date();
    const currentMonthStart = getDateOnly(startOfMonth(today));
    const currentMonthEnd = getDateOnly(endOfMonth(today));


    try {
      const trainingsQuery = queryClient.ensureQueryData(
        trainingsQueryOptions.all,
      );
      const ongoingTrainingsQuery = queryClient.ensureQueryData(
        ongoingTrainingsQueryOptions.active,
      );
      const workoutPlansQuery = queryClient.ensureQueryData(
        workoutPlansQueryOptions.all,
      );

      const overviewQuery = queryClient.ensureQueryData(
        trainingsOverviewQueryOptions.byDateRange(weekStart, weekEnd),
      );

      const monthOverviewQuery = queryClient.ensureQueryData(
        trainingsOverviewQueryOptions.byDateRange(currentMonthStart, currentMonthEnd),
      );

      const workoutsGoalQuery = queryClient.ensureQueryData(
        workoutsWeeklyGoalQueryOptions.current(),
      );

      const workoutStreakQuery = queryClient.ensureQueryData(
        workoutStreakQueryOptions.current(),
      );

      const nextWorkoutQuery = queryClient.ensureQueryData(
        workoutPlansQueryOptions.nextWorkout,
      );

      await Promise.all([
        trainingsQuery,
        ongoingTrainingsQuery,
        workoutPlansQuery,
        overviewQuery,
        workoutsGoalQuery,
        workoutStreakQuery,
        nextWorkoutQuery,
        monthOverviewQuery,
      ]);

      // // Active plan and next workout are optional (404 when no active plan).
      // try {
      //   await Promise.all([
      //     queryClient.ensureQueryData(workoutPlansQueryOptions.active),
      //     queryClient.ensureQueryData(workoutPlansQueryOptions.nextWorkout),
      //   ]);
      // } catch {
      //   // Ignore: page can still render without an active plan.
      // }

      // try {
      //   const { weekStart, weekEnd } = getCurrentWeekDateRange();
      //   await Promise.all([
      //     queryClient.ensureQueryData(
      //       trainingsOverviewQueryOptions.byDateRange(weekStart, weekEnd),
      //     ),
      //     queryClient.ensureQueryData(workoutsWeeklyGoalQueryOptions.current()),
      //   ]);
      // } catch {
      //   // Ignore: optional weekly goal or overview.
      // }
    } catch {
      // do nothing
    }
  },
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();

  return (
    <PageCard>
      <WorkoutPlansTopSection />
      <PageTitle title="Workouts">
        <Button asChild>
          <Button
            onClick={() => {
              navigate({
                to: "/trainings/workouts/create",
              });
            }}
            onMouseEnter={() => {
              router.preloadRoute({
                to: "/trainings/workouts/create",
              });
            }}
            onTouchStart={() => {
              router.preloadRoute({
                to: "/trainings/workouts/create",
              });
            }}
          >
            <Plus />
            Create
          </Button>
        </Button>
      </PageTitle>

      <WorkoutsList />
      <Outlet />
    </PageCard>
  );
}
