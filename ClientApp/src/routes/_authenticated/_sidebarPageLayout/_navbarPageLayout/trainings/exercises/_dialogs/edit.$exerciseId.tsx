import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import EditExerciseDialog from "@/features/trainings/exercises/components/exercisesDialogs/EditExerciseDialog";
import {
  ensureExerciseByIdWithPicturePreload,
  exercisesQueryOptions,
} from "@/features/trainings/exercises/queries/exercisesQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/exercises/_dialogs/edit/$exerciseId",
)({
  loader: async ({ params }) => {
    await ensureExerciseByIdWithPicturePreload(params.exerciseId);
  },
  component: RouteComponent,
});

function RouteComponent() {
  const { exerciseId } = Route.useParams();
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/trainings/workouts",
  });

  const { data: exercise } = useSuspenseQuery(
    exercisesQueryOptions.byId(exerciseId),
  );

  return (
    <EditExerciseDialog
      exercise={exercise}
      onSuccess={() => {
        navigateBackOrDefault();
      }}
      onClose={() => {
        navigateBackOrDefault();
      }}
    />
  );
}
