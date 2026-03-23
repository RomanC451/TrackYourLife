import { createFileRoute, Outlet, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";

import { router } from "@/App";
import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import WorkoutPlansTopSection from "@/features/trainings/workoutPlans/components/WorkoutPlansTopSection";
import { workoutPlansQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutPlansQueries";
import WorkoutsList from "@/features/trainings/trainings/components/trainingsList/TrainingsList";
import { trainingsQueryOptions } from "@/features/trainings/trainings/queries/trainingsQueries";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts",
)({
  loader: async () => {
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

      await Promise.all([
        trainingsQuery,
        ongoingTrainingsQuery,
        workoutPlansQuery,
      ]);

      // Active plan and next workout are optional (404 when no active plan).
      try {
        await Promise.all([
          queryClient.ensureQueryData(workoutPlansQueryOptions.active),
          queryClient.ensureQueryData(workoutPlansQueryOptions.nextWorkout),
        ]);
      } catch {
        // Ignore: page can still render without an active plan.
      }
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
