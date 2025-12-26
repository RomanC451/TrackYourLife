import { ExerciseSet } from "@/services/openapi";

import AdjustmentBadge from "../AdjustmentBadge";

function TimeBasedSetChange({
  oldSet,
  newSet,
}: {
  oldSet: ExerciseSet;
  newSet: ExerciseSet;
}) {
  const durationChange =
    (newSet.durationSeconds ?? 0) - (oldSet.durationSeconds ?? 0);
  return (
    <div>
      {durationChange !== 0 && (
        <AdjustmentBadge value={durationChange} unit="seconds" />
      )}
    </div>
  );
}

export default TimeBasedSetChange;
