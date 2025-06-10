import { createFileRoute } from "@tanstack/react-router";

import { prefetchActiveOngoingTrainingQuery } from "@/features/trainings/ongoing-workout/queries/useActiveOngoingTrainingQuery";
import OngoingWorkoutPage from "@/pages/trainings/OngoingWorkoutPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/ongoing-workout",
)({
  loader: () => {
    prefetchActiveOngoingTrainingQuery();
  },
  component: OngoingWorkoutPage,
});
