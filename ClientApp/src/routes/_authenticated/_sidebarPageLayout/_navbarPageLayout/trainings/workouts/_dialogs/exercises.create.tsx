import { createFileRoute } from "@tanstack/react-router";

import CreateExerciseDialog from "@/features/trainings/exercises/components/exercisesDialogs/CreateExerciseDialog";
import { ensureExercisesList } from "@/features/trainings/exercises/queries/exercisesQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/_dialogs/exercises/create",
)({
  loader: async () => {
    await ensureExercisesList();
  },
  component: RouteComponent,
});

function RouteComponent() {
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/trainings/workouts",
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
