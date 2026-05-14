import { createFileRoute } from "@tanstack/react-router";

import { ensureActiveOngoingTraining } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import AdjustExercisePage from "@/pages/trainings/AdjustExercisePage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/ongoing-workout/adjust-exercise/$exerciseId",
)({
  loader: async () => {
    await ensureActiveOngoingTraining();
  },
  component: AdjustExercisePage,
});
