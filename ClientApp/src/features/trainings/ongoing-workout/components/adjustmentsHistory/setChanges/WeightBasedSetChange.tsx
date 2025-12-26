import { ExerciseSet } from "@/services/openapi";

import AdjustmentBadge from "../AdjustmentBadge";

function WeightBasedSetChange({
  oldSet,
  newSet,
}: {
  oldSet: ExerciseSet;
  newSet: ExerciseSet;
}) {
  const weightChange = (newSet.weight ?? 0) - (oldSet.weight ?? 0);
  const repsChange = (newSet.reps ?? 0) - (oldSet.reps ?? 0);

  return (
    <div>
      {weightChange !== 0 && <AdjustmentBadge value={weightChange} unit="kg" />}
      {repsChange !== 0 && <AdjustmentBadge value={repsChange} unit="reps" />}
    </div>
  );
}

export default WeightBasedSetChange;
