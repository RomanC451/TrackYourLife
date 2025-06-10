import { createFileRoute } from "@tanstack/react-router";

import AdjustExercisePage from "@/pages/trainings/AdjustExercisePage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/adjust-exercise/$exerciseId",
)({
  component: AdjustExercisePage,
});
