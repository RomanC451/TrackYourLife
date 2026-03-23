import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import WorkoutPlanDialog from "@/features/trainings/workoutPlans/components/WorkoutPlanDialog";
import useUpdateWorkoutPlanMutation from "@/features/trainings/workoutPlans/mutations/useUpdateWorkoutPlanMutation";
import { workoutPlansQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutPlansQueries";
import { trainingsQueryOptions } from "@/features/trainings/trainings/queries/trainingsQueries";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import CancelableLoadingPage from "@/pages/CancelableLoadingPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/_dialogs/plan/edit/$planId",
)({
  component: RouteComponent,
  loader: async ({ params }) => {
    await Promise.all([
      queryClient.ensureQueryData(trainingsQueryOptions.all),
      queryClient.ensureQueryData(workoutPlansQueryOptions.byId(params.planId)),
    ]);
  },
  pendingComponent: () => (
    <CancelableLoadingPage defaultRouteOnCancel="/trainings/workouts" />
  ),
});

function RouteComponent() {
  const { planId } = Route.useParams();
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/trainings/workouts",
  });

  const { data: trainings } = useSuspenseQuery(trainingsQueryOptions.all);
  const { data: workoutPlan } = useSuspenseQuery(workoutPlansQueryOptions.byId(planId));
  const updateWorkoutPlanMutation = useUpdateWorkoutPlanMutation();

  return (
    <WorkoutPlanDialog
      title="Edit workout plan"
      description="Update plan details and workout order."
      submitButtonText="Save Changes"
      trainings={trainings}
      defaultValues={{
        name: workoutPlan.name,
        isActive: workoutPlan.isActive,
        trainingIds: workoutPlan.workouts.map((workout) => workout.id),
      }}
      isPending={updateWorkoutPlanMutation.isDelayedPending}
      onClose={() => navigateBackOrDefault()}
      onSubmit={(values) => {
        updateWorkoutPlanMutation.mutate(
          {
            id: planId,
            request: {
              name: values.name,
              isActive: values.isActive,
              trainingIds: values.trainingIds,
            },
          },
          {
            onSuccess: () => {
              navigateBackOrDefault();
            },
          },
        );
      }}
    />
  );
}
