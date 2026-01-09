import { CheckCircle2, Circle, SkipForward } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { ScrollArea } from "@/components/ui/scroll-area";
import { OngoingTrainingDto } from "@/services/openapi";

import useJumpToExerciseMutation from "../../mutations/useJumpToExerciseMutation";

type ExerciseStatus = "completed" | "skipped" | "pending" | "current";

function getExerciseStatus(
  exerciseId: string,
  ongoingTraining: OngoingTrainingDto,
  currentExerciseIndex: number,
  exerciseIndex: number,
): ExerciseStatus {
  if (exerciseIndex === currentExerciseIndex) {
    return "current";
  }
  const completedIds = ongoingTraining.completedExerciseIds || [];
  const skippedIds = ongoingTraining.skippedExerciseIds || [];

  if (completedIds.includes(exerciseId)) {
    return "completed";
  }
  if (skippedIds.includes(exerciseId)) {
    return "skipped";
  }
  return "pending";
}

function ExerciseSelectionDialog({
  open,
  onOpenChange,
  ongoingTraining,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  ongoingTraining: OngoingTrainingDto;
}) {
  const jumpToExerciseMutation = useJumpToExerciseMutation();

  const exercises = ongoingTraining.training?.exercises || [];
  const orderedExercises = [...exercises].sort((a, b) => {
    // Sort by orderIndex if available, otherwise by index
    const aIndex = exercises.indexOf(a);
    const bIndex = exercises.indexOf(b);
    return aIndex - bIndex;
  });

  const handleExerciseSelect = (exerciseIndex: number) => {
    // If selecting the current exercise, just close the dialog without making a request
    if (exerciseIndex === ongoingTraining.exerciseIndex) {
      onOpenChange(false);
      return;
    }

    jumpToExerciseMutation.mutate(
      {
        ongoingTraining,
        exerciseIndex,
      },
      {
        onSuccess: () => {
          onOpenChange(false);
        },
      },
    );
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="">
        <DialogHeader className="pb-4">
          <DialogTitle>Choose next exercise</DialogTitle>
          <DialogDescription>
            Select an exercise to jump to in your workout
          </DialogDescription>
        </DialogHeader>
        <ScrollArea className="max-h-[60vh]">
          <div className="space-y-2">
            {orderedExercises.map((exercise, index) => {
              const status = getExerciseStatus(
                exercise.id,
                ongoingTraining,
                ongoingTraining.exerciseIndex,
                index,
              );

              return (
                <Button
                  key={exercise.id}
                  variant={status === "current" ? "default" : "outline"}
                  className="h-auto w-full justify-start py-2.5"
                  onClick={() => handleExerciseSelect(index)}
                  disabled={jumpToExerciseMutation.isPending}
                >
                  <div className="flex w-full items-center justify-between gap-2">
                    <div className="flex min-w-0 items-center gap-2">
                      {status === "completed" && (
                        <CheckCircle2 className="size-4 shrink-0 text-green-500" />
                      )}
                      {status === "skipped" && (
                        <SkipForward className="size-4 shrink-0 text-yellow-500" />
                      )}
                      {status === "pending" && (
                        <Circle className="size-4 shrink-0 text-gray-400" />
                      )}
                      {status === "current" && (
                        <Circle className="size-4 shrink-0 fill-current" />
                      )}
                      <span className="truncate font-medium">
                        {exercise.name}
                      </span>
                    </div>
                    {status === "current" && (
                      <span className="shrink-0 text-xs text-muted-foreground">
                        Current
                      </span>
                    )}
                  </div>
                </Button>
              );
            })}
          </div>
        </ScrollArea>
      </DialogContent>
    </Dialog>
  );
}

export default ExerciseSelectionDialog;
