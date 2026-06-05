import { Card } from "@/components/ui/card";
import { ExerciseSet } from "@/services/openapi";

import AdjustmentBadge from "./AdjustmentBadge";

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
  const diff1 = newSet.count1 - oldSet.count1;
  const diff2 = (newSet.count2 ?? 0) - (oldSet.count2 ?? 0);

  return (
    <Card className="min-w-0 bg-secondary/30 p-3">
      <div className="mb-2 text-sm font-medium">
        {index + 1}. {oldSet.name || "Set"}
      </div>
      <div className="flex flex-wrap gap-1.5">
        {diff1 !== 0 && <AdjustmentBadge value={diff1} unit={newSet.unit1} />}
        {diff2 !== 0 && newSet.unit2 && (
          <AdjustmentBadge value={diff2} unit={newSet.unit2} />
        )}
      </div>
    </Card>
  );
}

export default AdjustmentSetChange;
