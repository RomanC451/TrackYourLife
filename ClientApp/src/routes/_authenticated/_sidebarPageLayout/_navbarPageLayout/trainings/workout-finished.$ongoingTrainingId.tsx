import { createFileRoute } from "@tanstack/react-router";

import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import WorkoutFinished from "@/pages/trainings/WorkoutFinishedPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workout-finished/$ongoingTrainingId",
)({
  loader: ({ params }) => {
    return queryClient.ensureQueryData(
      ongoingTrainingsQueryOptions.byId(params.ongoingTrainingId),
    );
  },
  component: WorkoutFinished,
});
