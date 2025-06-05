import { createFileRoute } from "@tanstack/react-router";

import OngoingWorkoutPage from "@/pages/trainings/OngoingWorkoutPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/ongoing-workout",
)({
  component: OngoingWorkoutPage,
});
