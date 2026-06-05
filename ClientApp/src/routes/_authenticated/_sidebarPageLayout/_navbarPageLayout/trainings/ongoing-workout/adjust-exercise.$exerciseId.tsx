import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute, useParams } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import AdjustExercise from "@/features/trainings/ongoing-workout/components/adjustExerciseForm/AdjustExercise";
import { ensureActiveOngoingTraining } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { MuscleGroupWorkoutIcon } from "@/features/trainings/utils/muscleGroupWorkoutIcon";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/ongoing-workout/adjust-exercise/$exerciseId",
)({
  loader: async () => {
    await ensureActiveOngoingTraining();
  },
  component: RouteComponent,
});

function RouteComponent() {
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
    <PageCard className="max-w-2xl space-y-6">
      <div className="flex items-center gap-3">
        <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary/15">
          <MuscleGroupWorkoutIcon
            muscleGroups={exercise.muscleGroups}
            className="h-6 w-6 text-primary"
          />
        </div>
        <div>
          <h1 className="text-2xl font-bold tracking-tight">
            {data.training.name}
          </h1>
          <p className="text-sm text-muted-foreground">
            Track your workout progress
          </p>
        </div>
      </div>
      <AdjustExercise exercise={exercise} />
    </PageCard>
  );
}
