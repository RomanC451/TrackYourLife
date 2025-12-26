import { ExerciseSet } from "@/services/openapi";

import AdjustmentBadge from "../AdjustmentBadge";

function BodyWeightSetChange({
  oldSet,
  newSet,
}: {
  oldSet: ExerciseSet;
  newSet: ExerciseSet;
}) {
  const repsChange = (newSet.reps ?? 0) - (oldSet.reps ?? 0);
  return (
    <div>
      {repsChange !== 0 && <AdjustmentBadge value={repsChange} unit="reps" />}
    </div>
  );
}

export default BodyWeightSetChange;
