import { createFileRoute } from "@tanstack/react-router";
import z from "zod";

import { exercisesQueryOptions } from "@/features/trainings/exercises/queries/exercisesQuery";
import CreateTrainingDialog from "@/features/trainings/trainings/components/trainingsDialogs/CreateTrainingDialog";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import LoadingPage from "@/pages/LoadingPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/_dialogs/create",
)({
  component: RouteComponent,

  validateSearch: z.object({
    tab: z.enum(["details", "exercises"]).default("details"),
  }),
  loader: async () => {
    await queryClient.ensureQueryData(exercisesQueryOptions.all);
  },
  pendingComponent: () => <LoadingPage />,

  errorComponent: () => <>error</>,
});

function RouteComponent() {
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/trainings/workouts",
  });

  return (
    <CreateTrainingDialog
      onClose={() => navigateBackOrDefault()}
      onSuccess={() => {
        navigateBackOrDefault();
      }}
    />
  );
}
