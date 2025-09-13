import { createFileRoute, Outlet, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";

import { router } from "@/App";
import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
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

      await Promise.all([trainingsQuery, ongoingTrainingsQuery]);
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
