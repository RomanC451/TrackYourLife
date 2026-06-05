import { format, formatDistanceToNowStrict, parseISO } from "date-fns";
import { Calendar, Trash2 } from "lucide-react";
import { zip } from "lodash";
import { useToggle } from "usehooks-ts";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { ExerciseHistoryDto, ExerciseSet } from "@/services/openapi";

import useDeleteExerciseHistoryMutation from "../../mutations/useDeleteExerciseHistoryMutation";
import AdjustmentSetChange from "./AdjustmentSetChange";

const VISIBLE_SETS_LIMIT = 4;

type AdjustmentSessionProps = {
  history: ExerciseHistoryDto;
};

function AdjustmentSession({ history }: AdjustmentSessionProps) {
  const [expanded, toggleExpanded] = useToggle(false);
  const deleteExerciseHistoryMutation = useDeleteExerciseHistoryMutation();

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

  const adjustmentCount = adjustedSetsPairs.length;
  const hasMoreSets = adjustedSetsPairs.length > VISIBLE_SETS_LIMIT;
  const visibleSets = expanded
    ? adjustedSetsPairs
    : adjustedSetsPairs.slice(0, VISIBLE_SETS_LIMIT);

  return (
    <div className="space-y-3">
      <div className="flex flex-wrap items-center justify-between gap-3 rounded-lg bg-secondary/50 p-3 transition-colors hover:bg-secondary/70">
        <div className="flex min-w-0 items-center gap-3">
          <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-background/50">
            <Calendar className="h-4 w-4 text-muted-foreground" />
          </div>
          <div className="min-w-0">
            <p className="text-sm font-medium">
              {format(parseISO(history.createdOnUtc), "EEE, MMM dd")}
            </p>
            <p className="text-xs text-muted-foreground">
              {formatDistanceToNowStrict(parseISO(history.createdOnUtc), {
                addSuffix: true,
              })}
            </p>
          </div>
        </div>
        <div className="flex shrink-0 items-center gap-2">
          <Badge
            variant="secondary"
            className="bg-primary/15 text-primary hover:bg-primary/20"
          >
            {adjustmentCount} adjustment{adjustmentCount === 1 ? "" : "s"}
          </Badge>
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8 text-muted-foreground hover:text-destructive"
            onClick={() =>
              deleteExerciseHistoryMutation.mutate({
                id: history.id,
                exerciseId: history.exerciseId,
              })
            }
            disabled={deleteExerciseHistoryMutation.isPending}
            title="Delete adjustment history"
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      </div>

      {adjustmentCount > 0 ? (
        <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
          {visibleSets.map(([oldSet, newSet], index) => (
            <AdjustmentSetChange
              key={oldSet.id}
              oldSet={oldSet}
              newSet={newSet}
              index={index}
            />
          ))}
        </div>
      ) : null}

      {hasMoreSets ? (
        <button
          className="flex items-center gap-1 text-xs text-primary hover:underline"
          onClick={toggleExpanded}
        >
          {expanded
            ? "Show less adjustments"
            : `Show ${adjustedSetsPairs.length - VISIBLE_SETS_LIMIT} more adjustments`}{" "}
          <span>{expanded ? "▲" : "▼"}</span>
        </button>
      ) : null}
    </div>
  );
}

export default AdjustmentSession;
