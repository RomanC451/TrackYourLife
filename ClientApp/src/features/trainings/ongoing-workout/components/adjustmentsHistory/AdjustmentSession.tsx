import { format, formatDistanceToNowStrict, parseISO } from "date-fns";
import { zip } from "lodash";
import { useToggle } from "usehooks-ts";

import { Badge } from "@/components/ui/badge";
import { ExerciseHistoryDto, ExerciseSet } from "@/services/openapi";

import AdjustmentSetChange from "./AdjustmentSetChange";

type AdjustmentSessionProps = {
  history: ExerciseHistoryDto;
};

function AdjustmentSession({ history }: AdjustmentSessionProps) {
  const [expanded, toggleExpanded] = useToggle(false);

  const adjustedSetsPairs: [ExerciseSet, ExerciseSet][] = zip(
    history.oldExerciseSets,
    history.newExerciseSets,
  ).filter((pair): pair is [ExerciseSet, ExerciseSet] => {
    const [oldSet, newSet] = pair;
    return (
      oldSet !== undefined &&
      newSet !== undefined &&
      (oldSet.id !== newSet.id ||
        oldSet.reps !== newSet.reps ||
        oldSet.weight !== newSet.weight ||
        oldSet.durationSeconds !== newSet.durationSeconds ||
        oldSet.distance !== newSet.distance)
    );
  });

  console.log(adjustedSetsPairs);

  return (
    <div className="space-y-2">
      <div className="flex items-center justify-between">
        <div>
          <span className="mr-2 text-base font-semibold">
            {format(parseISO(history.createdOnUtc), "EEE, MMM dd")}
          </span>
          <span className="text-xs text-muted-foreground">
            (
            {formatDistanceToNowStrict(parseISO(history.createdOnUtc), {
              addSuffix: true,
            })}
            )
          </span>
        </div>
        <Badge variant="outline">
          {history.newExerciseSets.length} adjustment
          {history.newExerciseSets.length > 1 ? "s" : ""}
        </Badge>
      </div>
      <div className="space-y-2">
        {adjustedSetsPairs.map(([oldSet, newSet], index) => (
          <AdjustmentSetChange
            key={oldSet.id}
            oldSet={oldSet}
            newSet={newSet}
            index={index}
          />
        ))}
        {history.newExerciseSets.length > 1 && (
          <button
            className="mt-1 flex items-center gap-1 text-xs text-primary hover:underline"
            onClick={toggleExpanded}
          >
            {expanded
              ? "Show less adjustments"
              : `Show ${history.newExerciseSets.length - 2} more adjustment${history.newExerciseSets.length - 2 > 1 ? "s" : ""}`}{" "}
            <span>{expanded ? "▲" : "▼"}</span>
          </button>
        )}
      </div>
    </div>
  );
}

export default AdjustmentSession;
