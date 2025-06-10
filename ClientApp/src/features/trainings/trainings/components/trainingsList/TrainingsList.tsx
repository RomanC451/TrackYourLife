import handleQuery from "@/components/handle-query";
import useActiveOngoingTrainingQuery from "@/features/trainings/ongoing-workout/queries/useActiveOngoingTrainingQuery";

import useTrainingsQuery from "../../queries/useTrainingsQuery";
import TrainingListItem from "./TrainingListItem";

function TrainingsList() {
  const { trainingsQuery } = useTrainingsQuery();

  const { activeOngoingTrainingQuery } = useActiveOngoingTrainingQuery();

  const activeOngoingTrainingId = activeOngoingTrainingQuery.isError
    ? null
    : activeOngoingTrainingQuery.data?.training?.id;

  return handleQuery(trainingsQuery, (trainings) => (
    <div className="grid grid-cols-1 gap-6 @3xl/page-card:grid-cols-2 @6xl/page-card:grid-cols-3">
      {trainings.map((training) => (
        <TrainingListItem
          key={training.id}
          training={training}
          isActive={activeOngoingTrainingId === training.id}
        />
      ))}
    </div>
  ));
}

export default TrainingsList;
