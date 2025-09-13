import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import z from "zod";

import { exercisesQueryOptions } from "@/features/trainings/exercises/queries/exercisesQuery";
import EditTrainingDialog from "@/features/trainings/trainings/components/trainingsDialogs/EditTrainingDialog";
import { trainingsQueryOptions } from "@/features/trainings/trainings/queries/trainingsQueries";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import CancelableLoadingPage from "@/pages/CancelableLoadingPage";
import { queryClient } from "@/queryClient";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/_dialogs/edit/$workoutId",
)({
  component: RouteComponent,

  validateSearch: z.object({
    tab: z.enum(["details", "exercises"]).default("details"),
  }),
  loader: async ({ params }) => {
    const trainingQuery = queryClient.ensureQueryData(
      trainingsQueryOptions.byId(params.workoutId),
    );
    const exercisesQuery = queryClient.ensureQueryData(
      exercisesQueryOptions.all,
    );

    await Promise.all([trainingQuery, exercisesQuery]);
  },
  pendingComponent: () => (
    <CancelableLoadingPage defaultRouteOnCancel="/trainings/workouts" />
  ),
});

function RouteComponent() {
  const { workoutId } = Route.useParams();

  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/trainings/workouts",
  });

  const { data: training, isError } = useSuspenseQuery(
    trainingsQueryOptions.byId(workoutId),
  );

  if (isError) {
    toastDefaultServerError();
    navigateBackOrDefault();
  }

  return (
    <EditTrainingDialog
      training={training}
      onClose={() => {
        navigateBackOrDefault();
      }}
      onSuccess={() => {
        navigateBackOrDefault();
      }}
    />
  );
}
