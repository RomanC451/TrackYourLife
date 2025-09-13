import { useSuspenseQuery } from "@tanstack/react-query";
import { useParams } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import AdjustExercise from "@/features/trainings/ongoing-workout/components/adjustExerciseForm/AdjustExercise";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";

function AdjustExercisePage() {
  const params = useParams({
    from: "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/ongoing-workout/adjust-exercise/$exerciseId",
  });

  const { data } = useSuspenseQuery(ongoingTrainingsQueryOptions.active);

  const exercise = data.training.exercises.find(
    (e) => e.id === params.exerciseId,
  );

  if (!exercise) {
    return <div>Exercise not found</div>;
  }

  return (
    <PageCard className="max-w-2xl">
      <PageTitle className="w-full text-center" title={data.training.name} />
      <AdjustExercise exercise={exercise} />
    </PageCard>
  );
}

export default AdjustExercisePage;
