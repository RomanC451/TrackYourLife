import { ExerciseSet } from "@/services/openapi";

import AdjustmentBadge from "../AdjustmentBadge";

function DistanceBasedSetChange({
  oldSet,
  newSet,
}: {
  oldSet: ExerciseSet;
  newSet: ExerciseSet;
}) {
  const distanceChange = (newSet.distance ?? 0) - (oldSet.distance ?? 0);
  return (
    <div>
      {distanceChange !== 0 && (
        <AdjustmentBadge value={distanceChange} unit="m" />
      )}
    </div>
  );
}

export default DistanceBasedSetChange;
