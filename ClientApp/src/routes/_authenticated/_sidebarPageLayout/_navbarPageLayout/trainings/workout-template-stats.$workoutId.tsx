import { createFileRoute } from "@tanstack/react-router";

import { trainingsQueryOptions } from "@/features/trainings/trainings/queries/trainingsQueries";
import WorkoutTemplateStatsPage from "@/pages/trainings/WorkoutTemplateStatsPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workout-template-stats/$workoutId",
)({
  loader: () => queryClient.ensureQueryData(trainingsQueryOptions.all),
  component: function WorkoutTemplateStatsRoute() {
    const { workoutId } = Route.useParams();
    return <WorkoutTemplateStatsPage workoutId={workoutId} />;
  },
});
