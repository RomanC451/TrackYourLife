import HandleMultipleQueries from "@/components/handle-multiple-queries";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { ExerciseDto, ExerciseHistoryDto } from "@/services/openapi";

import { exercisesQueryOptions } from "@/features/trainings/exercises/queries/exercisesQuery";
import { exerciseHistoryQueryOptions } from "@/features/trainings/ongoing-workout/queries/exerciseHistoryQuery";
import AdjustmentSession from "@/features/trainings/ongoing-workout/components/adjustmentsHistory/AdjustmentSession";

interface ExerciseHistoriesDialogProps {
  exerciseId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

function renderPending() {
  return (
    <>
      <DialogHeader>
        <DialogTitle className="text-2xl font-bold">
          <Skeleton className="h-8 w-48" />
        </DialogTitle>
        <DialogDescription hidden>
          View exercise history and adjustments
        </DialogDescription>
      </DialogHeader>
      <div className="space-y-2 h-[500px]">
        <Skeleton className="h-16 w-full" />
        <Skeleton className="h-16 w-full" />
        <Skeleton className="h-16 w-full" />
      </div>
    </>
  );
}


function renderSuccess(data: Record<string, unknown>) {
  const exercise = data.exercise as ExerciseDto;
  const histories = data.history as ExerciseHistoryDto[];

  return (
    <>
      <DialogHeader>
        <DialogTitle className="text-2xl font-bold">
          {exercise?.name} - Exercise History
        </DialogTitle>
        <DialogDescription hidden>
          View exercise history and adjustments
        </DialogDescription>
      </DialogHeader>
      <ScrollArea className="h-[500px] pr-4">
        {histories.length === 0 ? (
          <p className="text-center text-sm text-muted-foreground">
            No exercise history available
          </p>
        ) : (
          <div className="space-y-3">
            {histories.map((history, index) => (
              <div key={history.id}>
                <AdjustmentSession history={history} />
                {index < histories.length - 1 && <Separator className="mt-3" />}
              </div>
            ))}
          </div>
        )}
      </ScrollArea>
    </>
  );
}

export default function ExerciseHistoriesDialog({
  exerciseId,
  open,
  onOpenChange,
}: ExerciseHistoriesDialogProps) {
  const { query: exerciseQuery } = useCustomQuery(
    exercisesQueryOptions.byId(exerciseId),
  );

  const { query: exerciseHistoryQuery } = useCustomQuery(
    exerciseHistoryQueryOptions.byExerciseId(exerciseId),
  );

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] sm:max-w-[700px]">
        <HandleMultipleQueries
          queries={{
            exercise: exerciseQuery,
            history: exerciseHistoryQuery,
          }}
          pending={renderPending}
          success={renderSuccess}
        />
      </DialogContent>
    </Dialog>
  );
}
