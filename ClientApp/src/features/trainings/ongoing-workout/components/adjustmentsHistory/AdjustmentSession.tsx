import { format, formatDistanceToNowStrict, parseISO } from "date-fns";
import { zip } from "lodash";
import { useToggle } from "usehooks-ts";

import { Badge } from "@/components/ui/badge";
import { ExerciseHistoryDto } from "@/services/openapi";

import AdjustmentSetChange from "./AdjustmentSetChange";

type AdjustmentSessionProps = {
  history: ExerciseHistoryDto;
};

function AdjustmentSession({ history }: AdjustmentSessionProps) {
  const [expanded, toggleExpanded] = useToggle(false);

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
          {history.exerciseSetChanges.length} adjustment
          {history.exerciseSetChanges.length > 1 ? "s" : ""}
        </Badge>
      </div>
      <div className="space-y-2">
        {zip(history.exerciseSetsBeforeChange, history.exerciseSetChanges).map(
          ([set, change], index) => {
            if (!change || !set || (index > 1 && !expanded)) {
              return null;
            }

            return (
              <AdjustmentSetChange
                key={change.setId}
                exerciseSet={set}
                change={change}
                index={index}
              />
            );
          },
        )}
        {history.exerciseSetChanges.length > 1 && (
          <button
            className="mt-1 flex items-center gap-1 text-xs text-primary hover:underline"
            onClick={toggleExpanded}
          >
            {expanded
              ? "Show less adjustments"
              : `Show ${history.exerciseSetChanges.length - 2} more adjustment${history.exerciseSetChanges.length - 2 > 1 ? "s" : ""}`}{" "}
            <span>{expanded ? "▲" : "▼"}</span>
          </button>
        )}
      </div>
    </div>
  );
}

export default AdjustmentSession;
