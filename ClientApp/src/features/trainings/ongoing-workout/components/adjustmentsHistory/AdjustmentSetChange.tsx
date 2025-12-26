import { Card } from "@/components/ui/card";
import { ExerciseSet, ExerciseSetType } from "@/services/openapi";

import DistanceBasedSetChange from "./setChanges/DistanceBasedSetChange";
import TimeBasedSetChange from "./setChanges/TimeBasedSetChange";
import WeightBasedSetChange from "./setChanges/WeightBasedSetChange";

type AdjustmentSetChangeProps = {
  oldSet: ExerciseSet;
  newSet: ExerciseSet;
  index: number;
};

function AdjustmentSetChange({
  newSet,
  oldSet,
  index,
}: AdjustmentSetChangeProps) {
  return (
    <Card className="bg-card-secondary p-2">
      <div className="mb-1 text-sm font-medium">
        {index + 1}. {oldSet.name || "Set"}
      </div>
      <div className="flex gap-2">
        {newSet.type === ExerciseSetType.Weight && (
          <WeightBasedSetChange oldSet={oldSet} newSet={newSet} />
        )}
        {newSet.type === ExerciseSetType.Time && (
          <TimeBasedSetChange oldSet={oldSet} newSet={newSet} />
        )}
        {newSet.type === ExerciseSetType.Distance && (
          <DistanceBasedSetChange oldSet={oldSet} newSet={newSet} />
        )}
      </div>
    </Card>
  );
}

export default AdjustmentSetChange;
