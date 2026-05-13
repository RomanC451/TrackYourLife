import { CheckCircle2, Repeat, Weight } from "lucide-react";

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { capitalizeFirstLetter } from "@/lib/stringUtils";
import { OngoingTrainingDto } from "@/services/openapi";

interface ExerciseOverviewDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  ongoingTraining: OngoingTrainingDto;
}

function ExerciseOverviewDialog({
  open,
  onOpenChange,
  ongoingTraining,
}: ExerciseOverviewDialogProps) {
  const exercise = ongoingTraining.training.exercises[ongoingTraining.exerciseIndex];
  const sets = exercise.exerciseSets;
  const currentSetIndex = ongoingTraining.setIndex;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>{exercise.name} - Exercise Overview</DialogTitle>
          <DialogDescription className="space-y-1">
            {exercise.equipment ? <span>Equipment: {exercise.equipment}</span> : null}
            <span className="block">
              Set {currentSetIndex + 1} of {sets.length}
            </span>
          </DialogDescription>
        </DialogHeader>

        <ScrollArea className="max-h-[55vh] pr-4">
          <div className="space-y-3">
            {sets.map((set, index) => {
              const isCompleted = index < currentSetIndex;
              const isCurrent = index === currentSetIndex;
              let statusBadge = <Badge variant="outline">Upcoming</Badge>;

              if (isCurrent) {
                statusBadge = <Badge>Current</Badge>;
              } else if (isCompleted) {
                statusBadge = (
                  <Badge
                    variant="outline"
                    className="border-green-500 text-green-500"
                  >
                    <CheckCircle2 className="mr-1 size-3" />
                    Completed
                  </Badge>
                );
              }

              return (
                <div
                  key={`${set.name}-${index}`}
                  className={cn(
                    "rounded-lg border p-3",
                    isCurrent && "border-primary bg-primary/10",
                    isCompleted && "border-green-500/50 bg-green-500/10",
                  )}
                >
                  <div className="mb-2 flex items-center justify-between gap-2">
                    <span className="font-medium">Set {index + 1}</span>
                    {statusBadge}
                  </div>

                  <div className="mb-2 text-sm text-muted-foreground">
                    {capitalizeFirstLetter(set.name)}
                  </div>

                  <div className="flex items-center gap-4 text-sm">
                    <div className="flex items-center gap-1.5">
                      <Weight className="size-4 text-muted-foreground" />
                      <span>
                        {set.count1} {set.unit1}
                      </span>
                    </div>
                    {set.count2 && set.unit2 ? (
                      <div className="flex items-center gap-1.5">
                        <Repeat className="size-4 text-muted-foreground" />
                        <span>
                          {set.count2} {set.unit2}
                        </span>
                      </div>
                    ) : null}
                  </div>
                </div>
              );
            })}
          </div>
        </ScrollArea>
      </DialogContent>
    </Dialog>
  );
}

export default ExerciseOverviewDialog;
