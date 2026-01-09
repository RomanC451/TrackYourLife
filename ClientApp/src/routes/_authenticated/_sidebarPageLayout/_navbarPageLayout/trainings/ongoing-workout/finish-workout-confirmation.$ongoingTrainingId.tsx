import { createFileRoute } from "@tanstack/react-router";

import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import FinishWorkoutConfirmationPage from "@/pages/trainings/FinishWorkoutConfirmationPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/ongoing-workout/finish-workout-confirmation/$ongoingTrainingId",
)({
  loader: ({ params }) => {
    queryClient.ensureQueryData(
      ongoingTrainingsQueryOptions.byId(params.ongoingTrainingId),
    );
  },
  component: FinishWorkoutConfirmationPage,
});
