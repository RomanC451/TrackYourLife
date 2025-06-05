import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Separator } from "@/components/ui/separator";
import ExercisePreview from "@/features/trainings/ongoing-workout/components/exercisePreview/ExercisePreview";
import OngoingWorkoutHeader from "@/features/trainings/ongoing-workout/components/OngoingWorkoutHeader";

function OngoingWorkoutPage() {
  return (
    <PageCard>
      <PageTitle title="Ongoing workout" />
      <Separator className="h-[1px] w-full" />
      <OngoingWorkoutHeader />
      <ExercisePreview />
    </PageCard>
  );
}

export default OngoingWorkoutPage;
