import { useState } from "react";
import { useQuery } from "@tanstack/react-query";

import HandleQuery from "@/components/handle-query";
import { Card, CardContent, CardTitle } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { cn } from "@/lib/utils";
import { ExerciseHistoryDto } from "@/services/openapi";

import { exerciseHistoryQueryOptions } from "../../queries/exerciseHistoryQuery";
import AdjustmentSession from "./AdjustmentSession";

function AdjustmentsHistory({ exerciseId }: { exerciseId: string }) {
  const [showAllSessions, setShowAllSessions] = useState(false);

  const exerciseHistoryQuery = useQuery(
    exerciseHistoryQueryOptions.byExerciseId(exerciseId),
  );

  return HandleQuery({
    query: exerciseHistoryQuery,
    pending: () => <AdjustmentsHistorySkeleton />,
    success: (data: ExerciseHistoryDto[]) => {
      const historiesWithChanges = data; //data.filter(hasHistoryChanges);

      console.log(historiesWithChanges.length);

      return (
        <Card className="flex flex-col gap-4">
          <CardContent className={cn("p-4", showAllSessions && "pr-1")}>
            <CardTitle
              hidden={historiesWithChanges.length === 0}
              className="mb-4"
            >
              Adjustments History
            </CardTitle>
            <AdjustmentsHistoryContent
              historiesWithChanges={historiesWithChanges}
              showAllSessions={showAllSessions}
            />

            {historiesWithChanges.length > 1 && (
              <button
                className="mx-auto mt-2 flex items-center gap-1 text-xs text-primary hover:underline"
                onClick={() => setShowAllSessions((prev) => !prev)}
              >
                {showAllSessions
                  ? "Show less sessions"
                  : `Show ${historiesWithChanges.length - 1} more sessions`}{" "}
                <span>{showAllSessions ? "▲" : "▼"}</span>
              </button>
            )}
          </CardContent>
        </Card>
      );
    },
  });
}

function AdjustmentsHistoryContent({
  historiesWithChanges,
  showAllSessions,
}: {
  historiesWithChanges: ExerciseHistoryDto[];
  showAllSessions: boolean;
}) {
  if (historiesWithChanges.length === 0) {
    return (
      <p className="text-center text-sm text-muted-foreground">
        No adjustments history
      </p>
    );
  }

  if (historiesWithChanges.length === 1 || !showAllSessions) {
    return (
      <div className="flex flex-col gap-4">
        <AdjustmentSession history={historiesWithChanges[0]} />
      </div>
    );
  }

  return (
    <ScrollArea type="always" className="h-[400px] max-h-[500px] pr-3">
      <div className="space-y-3">
        {historiesWithChanges.map((history, index) => {
          if (!showAllSessions && index > 0) {
            return null;
          }

          return (
            <>
              <AdjustmentSession history={history} />
              {index < historiesWithChanges.length - 1 && showAllSessions && (
                <Separator />
              )}
            </>
          );
        })}
      </div>
    </ScrollArea>
  );
}

function AdjustmentsHistorySkeleton() {
  return (
    <Card className="flex flex-col gap-4">
      <CardContent className="space-y-2 p-4">
        <CardTitle className="mb-4">Adjustments History</CardTitle>
        <div className="flex flex-col gap-2">
          <Skeleton className="h-6 w-20 bg-foreground" />
        </div>
        <Skeleton className="h-16 w-full" />
      </CardContent>
    </Card>
  );
}

// function hasHistoryChanges(history: ExerciseHistoryDto) {
//   return history.newExerciseSets.some((set) => {
//     switch (set.type) {
//       case ExerciseSetType.Weight:
//         return set.weight !== 0 || set.reps !== 0;
//       case ExerciseSetType.Time:
//         return set.durationSeconds !== 0;
//       case ExerciseSetType.Bodyweight:
//         return set.reps !== 0;
//       case ExerciseSetType.Distance:
//         return set.distance !== 0;
//     }
//   });
// }

export default AdjustmentsHistory;
