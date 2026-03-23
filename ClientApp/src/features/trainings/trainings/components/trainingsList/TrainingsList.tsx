import { useSuspenseQuery } from "@tanstack/react-query";

import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { workoutPlansQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutPlansQueries";
import { useCustomQuery } from "@/hooks/useCustomQuery";

import { trainingsQueryOptions } from "../../queries/trainingsQueries";
import TrainingListItem from "./TrainingListItem";

function TrainingsList() {
  const { data: trainings } = useSuspenseQuery(trainingsQueryOptions.all);
  const { data: workoutPlans } = useSuspenseQuery(workoutPlansQueryOptions.all);

  const { query: activeOngoingTrainingQuery } = useCustomQuery(
    ongoingTrainingsQueryOptions.active,
  );

  const activeOngoingTrainingId = activeOngoingTrainingQuery.isError
    ? null
    : activeOngoingTrainingQuery.data?.training?.id;

  return (
    <div className="grid grid-cols-1 gap-6 @3xl/page-card:grid-cols-2 @6xl/page-card:grid-cols-3">
      {trainings.map((training) => {
        const plansContainingWorkout = workoutPlans.filter((plan) =>
          plan.workouts.some((workout) => workout.id === training.id),
        );
        const activePlan = plansContainingWorkout.find((plan) => plan.isActive);
        return (
          <TrainingListItem
            key={training.id}
            training={training}
            isActive={activeOngoingTrainingId === training.id}
            isInActivePlan={Boolean(activePlan)}
          />
        );
      })}
    </div>
  );
}

export default TrainingsList;
