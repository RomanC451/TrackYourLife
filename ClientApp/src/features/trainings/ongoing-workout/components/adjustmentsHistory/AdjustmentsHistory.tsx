import { useState } from "react";
import { ChevronDown, History } from "lucide-react";

import HandleQuery from "@/components/handle-query";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import {
  Collapsible,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { cn } from "@/lib/utils";
import { ExerciseHistoryDto } from "@/services/openapi";

import { exerciseHistoryQueryOptions } from "../../queries/exerciseHistoryQuery";
import AdjustmentSession from "./AdjustmentSession";

function AdjustmentsHistory({ exerciseId }: { exerciseId: string }) {
  const [showAllSessions, setShowAllSessions] = useState(false);

  const { query: exerciseHistoryQuery } = useCustomQuery(
    exerciseHistoryQueryOptions.byExerciseId(exerciseId),
  );

  return HandleQuery({
    query: exerciseHistoryQuery,
    pending: () => <AdjustmentsHistorySkeleton />,
    success: (data: ExerciseHistoryDto[]) => {
      const historiesWithChanges = data;
      const hasMultipleSessions = historiesWithChanges.length > 1;

      return (
        <Card className="border-border/50 bg-card/80 backdrop-blur-sm">
          <CardHeader className="pb-3">
            <div className="flex items-center gap-2">
              <History className="h-4 w-4 text-muted-foreground" />
              <h2 className="text-sm font-semibold">Adjustments History</h2>
            </div>
          </CardHeader>
          <CardContent className="space-y-3">
            {historiesWithChanges.length === 0 ? (
              <p className="text-center text-sm text-muted-foreground">
                No adjustments history
              </p>
            ) : showAllSessions ? (
              <ScrollArea type="always" className="h-[400px] max-h-[500px] pr-3">
                <div className="space-y-3">
                  {historiesWithChanges.map((history, index) => (
                    <div key={history.id}>
                      <AdjustmentSession history={history} />
                      {index < historiesWithChanges.length - 1 ? (
                        <Separator className="mt-3" />
                      ) : null}
                    </div>
                  ))}
                </div>
              </ScrollArea>
            ) : (
              <AdjustmentSession history={historiesWithChanges[0]} />
            )}

            {hasMultipleSessions ? (
              <Collapsible
                open={showAllSessions}
                onOpenChange={setShowAllSessions}
              >
                <CollapsibleTrigger asChild>
                  <Button
                    variant="ghost"
                    className="w-full text-primary hover:bg-primary/5 hover:text-primary/80"
                  >
                    <span>
                      {showAllSessions
                        ? "Show less sessions"
                        : `Show ${historiesWithChanges.length - 1} more sessions`}
                    </span>
                    <ChevronDown
                      className={cn(
                        "ml-2 h-4 w-4 transition-transform duration-200",
                        showAllSessions && "rotate-180",
                      )}
                    />
                  </Button>
                </CollapsibleTrigger>
              </Collapsible>
            ) : null}
          </CardContent>
        </Card>
      );
    },
  });
}

function AdjustmentsHistorySkeleton() {
  return (
    <Card className="border-border/50 bg-card/80 backdrop-blur-sm">
      <CardHeader className="pb-3">
        <div className="flex items-center gap-2">
          <Skeleton className="h-4 w-4" />
          <Skeleton className="h-4 w-36" />
        </div>
      </CardHeader>
      <CardContent className="space-y-3">
        <Skeleton className="h-16 w-full" />
      </CardContent>
    </Card>
  );
}

export default AdjustmentsHistory;
