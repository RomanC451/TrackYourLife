import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";

import { router } from "@/App";
import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import WorkoutPlansTopSection from "@/features/trainings/workoutPlans/components/WorkoutPlansTopSection";
import WorkoutsList from "@/features/trainings/trainings/components/trainingsList/TrainingsList";
import { prefetchWorkoutsPageQueries } from "@/features/trainings/workouts/prefetchWorkoutsPageQueries";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/",
)({
  loader: () => prefetchWorkoutsPageQueries(queryClient),
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
    </PageCard>
  );
}
