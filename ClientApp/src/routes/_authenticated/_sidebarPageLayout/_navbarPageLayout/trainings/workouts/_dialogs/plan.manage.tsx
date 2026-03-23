import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import WorkoutPlanList from "@/features/trainings/workoutPlans/components/WorkoutPlanList";
import { workoutPlansQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutPlansQueries";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/_dialogs/plan/manage",
)({
  component: RouteComponent,
  loader: async () => {
    await queryClient.ensureQueryData(workoutPlansQueryOptions.all);
  },
});

function RouteComponent() {
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/trainings/workouts",
  });

  const { data: workoutPlans } = useSuspenseQuery(workoutPlansQueryOptions.all);

  return (
    <Dialog
      onOpenChange={(state) => {
        if (!state) {
          navigateBackOrDefault();
        }
      }}
      defaultOpen={true}
    >
      <DialogContent className="sm:max-w-md" withoutOverlay>
        <DialogHeader className="text-left">
          <DialogTitle className="text-xl font-semibold">Manage Plans</DialogTitle>
          <DialogDescription>
            Create, edit, and organize your workout plans.
          </DialogDescription>
        </DialogHeader>

        <WorkoutPlanList plans={workoutPlans} />
      </DialogContent>
    </Dialog>
  );
}
