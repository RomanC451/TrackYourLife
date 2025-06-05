import handleQuery from "@/components/handle-query";

import useTrainingsQuery from "../../queries/useTrainingsQuery";
import TrainingListItem from "./TrainingListItem";

function TrainingsList() {
  const { trainingsQuery } = useTrainingsQuery();

  return handleQuery(trainingsQuery, (trainings) => (
    <div className="grid grid-cols-1 gap-6 @3xl/page-card:grid-cols-2 @6xl/page-card:grid-cols-3">
      {trainings.map((training) => (
        <TrainingListItem key={training.id} training={training} />
      ))}
    </div>
  ));
}

export default TrainingsList;
