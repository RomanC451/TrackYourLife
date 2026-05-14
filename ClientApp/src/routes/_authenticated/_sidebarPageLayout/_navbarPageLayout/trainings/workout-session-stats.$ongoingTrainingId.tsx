import { createFileRoute } from "@tanstack/react-router";

import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import WorkoutSessionStatsPage from "@/pages/trainings/WorkoutSessionStatsPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workout-session-stats/$ongoingTrainingId",
)({
  loader: ({ params }) =>
    queryClient.ensureQueryData(
      ongoingTrainingsQueryOptions.byId(params.ongoingTrainingId),
    ),
  component: function WorkoutSessionStatsRoute() {
    const { ongoingTrainingId } = Route.useParams();
    return <WorkoutSessionStatsPage ongoingTrainingId={ongoingTrainingId} />;
  },
});
