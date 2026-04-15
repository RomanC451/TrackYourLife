import { createFileRoute } from "@tanstack/react-router";
import { subDays } from "date-fns";

import { workoutHistoryQueryOptions } from "@/features/trainings/history/queries/useWorkoutHistoryQuery";
import { getDateOnly } from "@/lib/date";
import WorkoutHistoryPage from "@/pages/trainings/WorkoutHistoryPage";
import { queryClient } from "@/queryClient";
import { trainingsQueryOptions } from "@/features/trainings/trainings/queries/trainingsQueries";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/history",
)({
  loader: async () => {
    const defaultStartDate = getDateOnly(subDays(new Date(), 30));
    const defaultEndDate = getDateOnly(new Date());

    const workoutHistoryQuery = queryClient.ensureQueryData(
      workoutHistoryQueryOptions.byDateRange(
        defaultStartDate,
        defaultEndDate,
      ),
    )

    const trainingsQuery = queryClient.ensureQueryData(
      trainingsQueryOptions.all,
    )

    const workoutsGoalQuery = queryClient.ensureQueryData(
      trainingsQueryOptions.all,
    );


    await Promise.all([workoutHistoryQuery, trainingsQuery, workoutsGoalQuery]);

  },
  component: RouteComponent,
});

function RouteComponent() {
  return <WorkoutHistoryPage />;
}

