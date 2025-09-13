import { Card } from "@/components/ui/card";
import { ExerciseSet, ExerciseSetChange } from "@/services/openapi";

import AdjustmentBadge from "./AdjustmentBadge";

type AdjustmentSetChangeProps = {
  exerciseSet: ExerciseSet;
  change: ExerciseSetChange;
  index: number;
};

function AdjustmentSetChange({
  change,
  exerciseSet: set,
  index,
}: AdjustmentSetChangeProps) {
  return (
    <Card className="bg-card-secondary p-2">
      <div className="mb-1 text-sm font-medium">
        Set {index + 1} · {set?.name || "Set"}
      </div>
      <div className="flex gap-2">
        {change.repsChange !== 0 && (
          <AdjustmentBadge value={change.repsChange} unit="reps" />
        )}
        {change.weightChange !== 0 && (
          <AdjustmentBadge value={change.weightChange} unit="kg" />
        )}
        {change.repsChange === 0 && change.weightChange === 0 && (
          <span className="text-xs text-muted-foreground">No change</span>
        )}
      </div>
    </Card>
  );
}

export default AdjustmentSetChange;
