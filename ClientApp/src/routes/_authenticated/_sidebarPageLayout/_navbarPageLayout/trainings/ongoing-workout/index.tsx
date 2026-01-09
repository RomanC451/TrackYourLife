import { createFileRoute } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import ExercisePreview from "@/features/trainings/ongoing-workout/components/exercisePreview/ExercisePreview";
import OngoingWorkoutHeader from "@/features/trainings/ongoing-workout/components/exercisePreview/ExercisePreviewHeader";
import CancelTrainingButton from "@/features/trainings/ongoing-workout/components/CancelTrainingButton";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/ongoing-workout/",
)({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <PageCard className="max-w-6xl">
      <PageTitle title="Ongoing workout">
        <CancelTrainingButton />
      </PageTitle>
      <OngoingWorkoutHeader />
      <ExercisePreview />
    </PageCard>
  );
}
