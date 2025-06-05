import { createFileRoute } from "@tanstack/react-router";

import WorkoutFinished from "@/pages/trainings/WorkoutFinished";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workout-finished/$ongoingTrainingId",
)({
  component: WorkoutFinished,
});
