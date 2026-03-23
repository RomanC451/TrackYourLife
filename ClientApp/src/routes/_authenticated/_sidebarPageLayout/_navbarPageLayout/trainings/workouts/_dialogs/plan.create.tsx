import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import WorkoutPlanDialog from "@/features/trainings/workoutPlans/components/WorkoutPlanDialog";
import useCreateWorkoutPlanMutation from "@/features/trainings/workoutPlans/mutations/useCreateWorkoutPlanMutation";
import { trainingsQueryOptions } from "@/features/trainings/trainings/queries/trainingsQueries";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import CancelableLoadingPage from "@/pages/CancelableLoadingPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/_dialogs/plan/create",
)({
  component: RouteComponent,
  loader: async () => {
    await queryClient.ensureQueryData(trainingsQueryOptions.all);
  },
  pendingComponent: () => (
    <CancelableLoadingPage defaultRouteOnCancel="/trainings/workouts" />
  ),
});

function RouteComponent() {
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/trainings/workouts",
  });

  const { data: trainings } = useSuspenseQuery(trainingsQueryOptions.all);
  const createWorkoutPlanMutation = useCreateWorkoutPlanMutation();

  return (
    <WorkoutPlanDialog
      title="Create Workout Plan"
      description="Define your plan name, order workouts, and add new ones."
      submitButtonText="Save Changes"
      trainings={trainings}
      defaultValues={{
        name: "",
        isActive: false,
        trainingIds: [],
      }}
      isPending={createWorkoutPlanMutation.isDelayedPending}
      onClose={() => navigateBackOrDefault()}
      onSubmit={(values) => {
        createWorkoutPlanMutation.mutate(
          {
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
