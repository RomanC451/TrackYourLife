import { createFileRoute } from "@tanstack/react-router";

import CreateExerciseDialog from "@/features/trainings/exercises/components/exercisesDialogs/CreateExerciseDialog";
import { exercisesQueryOptions } from "@/features/trainings/exercises/queries/exercisesQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/exercises/_dialogs/create",
)({
  loader: async () => {
    await queryClient.ensureQueryData(exercisesQueryOptions.all);
  },
  component: RouteComponent,
});

function RouteComponent() {
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/trainings/exercises",
  });

  return (
    <CreateExerciseDialog
      onClose={() => navigateBackOrDefault()}
      onSuccess={() => {
        navigateBackOrDefault();
      }}
    />
  );
}
