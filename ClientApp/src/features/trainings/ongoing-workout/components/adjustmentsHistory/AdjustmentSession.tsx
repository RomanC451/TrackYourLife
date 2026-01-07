import { format, formatDistanceToNowStrict, parseISO } from "date-fns";
import { chunk, zip } from "lodash";
import { useToggle } from "usehooks-ts";

import { Badge } from "@/components/ui/badge";
import { screensEnum } from "@/constants/tailwindSizes";
import { useAppGeneralStateContext } from "@/contexts/AppGeneralContextProvider";
import { ExerciseHistoryDto, ExerciseSet } from "@/services/openapi";

import AdjustmentSetChange from "./AdjustmentSetChange";

type AdjustmentSessionProps = {
  history: ExerciseHistoryDto;
};

function AdjustmentSession({ history }: AdjustmentSessionProps) {
  const [expanded, toggleExpanded] = useToggle(false);

  const { screenSize } = useAppGeneralStateContext();

  const adjustedSetsPairs: [ExerciseSet, ExerciseSet][] = zip(
    history.oldExerciseSets,
    history.newExerciseSets,
  ).filter((pair): pair is [ExerciseSet, ExerciseSet] => {
    const [oldSet, newSet] = pair;
    return (
      oldSet !== undefined &&
      newSet !== undefined &&
      (oldSet.id !== newSet.id ||
        oldSet.count1 !== newSet.count1 ||
        oldSet.unit1 !== newSet.unit1 ||
        oldSet.count2 !== newSet.count2 ||
        oldSet.unit2 !== newSet.unit2)
    );
  });

  const itemsPerRow = screenSize.width < screensEnum.md ? 2 : 4;
  const chunks = chunk(adjustedSetsPairs, itemsPerRow);
  const firstRow = chunks[0] ?? [];
  const remainingRows = chunks.slice(1);

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
      <div
        className="grid gap-2"
        style={{
          gridTemplateColumns: `repeat(${itemsPerRow}, minmax(0, 1fr))`,
        }}
      >
        {firstRow.map(([oldSet, newSet], index) => (
          <AdjustmentSetChange
            key={oldSet.id}
            oldSet={oldSet}
            newSet={newSet}
            index={index}
          />
        ))}
        {expanded &&
          remainingRows
            .flat()
            .map(([oldSet, newSet], index) => (
              <AdjustmentSetChange
                key={oldSet.id}
                oldSet={oldSet}
                newSet={newSet}
                index={firstRow.length + index}
              />
            ))}
      </div>
      {remainingRows.length > 0 &&
        (() => {
          const remainingCount = adjustedSetsPairs.length - firstRow.length;
          const adjustmentText =
            remainingCount > 1 ? "adjustments" : "adjustment";
          const buttonText = expanded
            ? "Show less adjustments"
            : `Show ${remainingCount} more ${adjustmentText}`;

          return (
            <button
              className="mt-1 flex items-center gap-1 text-xs text-primary hover:underline"
              onClick={toggleExpanded}
            >
              {buttonText} <span>{expanded ? "▲" : "▼"}</span>
            </button>
          );
        })()}
    </div>
  );
}

export default AdjustmentSession;
