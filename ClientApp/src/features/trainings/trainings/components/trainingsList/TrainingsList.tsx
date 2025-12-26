import { useQuery, useSuspenseQuery } from "@tanstack/react-query";

import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { usePerformanceMonitor } from "@/hooks/use-performance-monitor";

import { trainingsQueryOptions } from "../../queries/trainingsQueries";
import TrainingListItem from "./TrainingListItem";

function TrainingsList() {
  usePerformanceMonitor("TrainingsList");

  const { data: trainings } = useSuspenseQuery(trainingsQueryOptions.all);

  const activeOngoingTrainingQuery = useQuery(
    ongoingTrainingsQueryOptions.active,
  );

  const activeOngoingTrainingId = activeOngoingTrainingQuery.isError
    ? null
    : activeOngoingTrainingQuery.data?.training?.id;

  return (
    <div className="grid grid-cols-1 gap-6 @3xl/page-card:grid-cols-2 @6xl/page-card:grid-cols-3">
      {trainings.map((training) => (
        <TrainingListItem
          key={training.id}
          training={training}
          isActive={activeOngoingTrainingId === training.id}
        />
      ))}
    </div>
  );
}

export default TrainingsList;
