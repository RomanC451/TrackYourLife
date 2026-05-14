import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import ShowExerciseInfoDialog from "@/features/trainings/exercises/components/exercisesDialogs/ShowExerciseInfoDialog";
import {
  ensureExerciseByIdWithPicturePreload,
  exercisesQueryOptions,
} from "@/features/trainings/exercises/queries/exercisesQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/_dialogs/exercises/info/$exerciseId",
)({
  loader: async ({ params }) => {
    await ensureExerciseByIdWithPicturePreload(params.exerciseId);
  },
  component: RouteComponent,
});

function RouteComponent() {
  const { exerciseId } = Route.useParams();

  const { data: exercise } = useSuspenseQuery(
    exercisesQueryOptions.byId(exerciseId),
  );

  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/trainings/workouts",
  });

  return (
    <ShowExerciseInfoDialog
      exercise={exercise}
      onClose={() => navigateBackOrDefault()}
    />
  );
}
