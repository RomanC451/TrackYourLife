import { format } from "date-fns";
import {
  CheckCircle2,
  Clock,
  Dumbbell,
  Flame,
  MinusCircle,
  Target,
} from "lucide-react";
import { useQuery } from "@tanstack/react-query";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Skeleton } from "@/components/ui/skeleton";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { capitalizeFirstLetter } from "@/lib/stringUtils";
import { formatDurationMs } from "@/lib/time";
import type {
  ExerciseDto,
  ExerciseSet,
  OngoingTrainingDto,
  WorkoutHistoryDto,
} from "@/services/openapi";

function formatSetLine(set: ExerciseSet) {
  const primary = `${set.count1} ${set.unit1}`;
  const secondary =
    set.count2 != null && set.unit2 != null
      ? `${set.count2} ${set.unit2}`
      : null;
  return { primary, secondary };
}

function ExerciseSessionBlock({
  exercise,
  variant,
}: {
  exercise: ExerciseDto;
  variant: "completed" | "skipped";
}) {
  return (
    <div
      className={
        variant === "skipped"
          ? "rounded-lg border border-border/60 bg-muted/30 p-3"
          : "rounded-lg border border-border/60 bg-secondary/30 p-3"
      }
    >
      <div className="mb-2 flex items-start gap-2">
        {variant === "completed" ? (
          <CheckCircle2 className="mt-0.5 h-4 w-4 shrink-0 text-success" />
        ) : (
          <MinusCircle className="mt-0.5 h-4 w-4 shrink-0 text-muted-foreground" />
        )}
        <div className="min-w-0 flex-1">
          <p className="font-medium leading-tight">{exercise.name}</p>
          {exercise.equipment ? (
            <p className="text-xs text-muted-foreground">{exercise.equipment}</p>
          ) : null}
        </div>
      </div>

      {variant === "completed" && exercise.exerciseSets.length > 0 ? (
        <ul className="space-y-1.5 border-t border-border/40 pt-2">
          {exercise.exerciseSets.map((set, index) => {
            const { primary, secondary } = formatSetLine(set);
            return (
              <li
                key={`${exercise.id}-${set.id}-${index}`}
                className="flex flex-wrap items-baseline gap-x-3 gap-y-0.5 text-sm"
              >
                <span className="text-muted-foreground">
                  {index + 1}. {capitalizeFirstLetter(set.name)}
                </span>
                <span className="font-medium tabular-nums">{primary}</span>
                {secondary ? (
                  <span className="text-muted-foreground tabular-nums">
                    {secondary}
                  </span>
                ) : null}
              </li>
            );
          })}
        </ul>
      ) : null}

      {variant === "skipped" ? (
        <p className="border-t border-border/40 pt-2 text-sm text-muted-foreground">
          Skipped
        </p>
      ) : null}
    </div>
  );
}

function SessionExerciseLists({ data }: { data: OngoingTrainingDto }) {
  const completedIds = new Set(data.completedExerciseIds ?? []);
  const skippedIds = new Set(data.skippedExerciseIds ?? []);
  const exercises = data.training.exercises ?? [];

  const completedExercises = exercises.filter((ex) => completedIds.has(ex.id));
  const skippedExercises = exercises.filter((ex) => skippedIds.has(ex.id));

  const showEmpty = completedExercises.length === 0 && skippedExercises.length === 0;

  return (
    <>
      {completedExercises.length > 0 ? (
        <section className="space-y-2">
          <h3 className="text-sm font-semibold text-foreground">
            Exercises completed ({completedExercises.length})
          </h3>
          <div className="space-y-2">
            {completedExercises.map((ex) => (
              <ExerciseSessionBlock key={ex.id} exercise={ex} variant="completed" />
            ))}
          </div>
        </section>
      ) : null}

      {skippedExercises.length > 0 ? (
        <section className="space-y-2">
          <h3 className="text-sm font-semibold text-muted-foreground">
            Skipped ({skippedExercises.length})
          </h3>
          <div className="space-y-2">
            {skippedExercises.map((ex) => (
              <ExerciseSessionBlock key={ex.id} exercise={ex} variant="skipped" />
            ))}
          </div>
        </section>
      ) : null}

      {showEmpty ? (
        <p className="text-sm text-muted-foreground">
          No exercise breakdown is available for this session.
        </p>
      ) : null}
    </>
  );
}

type WorkoutSessionDetailsDialogProps = {
  workout: WorkoutHistoryDto;
  onClose: () => void;
};

export function WorkoutSessionDetailsDialog({
  workout,
  onClose,
}: WorkoutSessionDetailsDialogProps) {
  const { data, isPending, isError, error, refetch } = useQuery({
    ...ongoingTrainingsQueryOptions.byId(workout.id),
  });

  const finishedAt = new Date(workout.finishedOnUtc);

  const headerSummary = (
    <div className="space-y-1">
      <DialogTitle className="text-left text-xl">{workout.trainingName}</DialogTitle>
      <DialogDescription className="text-left">
        {format(finishedAt, "MMM d, yyyy")} at {format(finishedAt, "h:mm a")}
      </DialogDescription>
    </div>
  );

  return (
    <Dialog
      open
      onOpenChange={(open) => {
        if (open) {
          return;
        }
        onClose();
      }}
    >
      <DialogContent
        className="flex max-h-[min(90vh,720px)] flex-col gap-0 overflow-hidden p-0 sm:max-w-lg"
        onClick={(e) => e.stopPropagation()}
      >
        <DialogHeader className="shrink-0 space-y-3 border-b border-border px-6 py-4 text-left">
          {headerSummary}

          <div className="flex flex-wrap gap-3 text-sm">
            <div className="flex items-center gap-1.5 text-muted-foreground">
              <Clock className="h-4 w-4 shrink-0" />
              <span>
                {formatDurationMs(workout.durationSeconds * 1000)}
              </span>
            </div>
            {typeof workout.caloriesBurned === "number" ? (
              <div className="flex items-center gap-1.5">
                <Flame className="h-4 w-4 shrink-0 text-primary" />
                <span className="font-medium tabular-nums text-primary">
                  {workout.caloriesBurned.toLocaleString()} kcal
                </span>
              </div>
            ) : null}
          </div>
        </DialogHeader>

        <div className="min-h-0 flex-1 px-6 py-4">
          {isPending ? (
            <div className="space-y-3">
              <Skeleton className="h-4 w-40" />
              <Skeleton className="h-24 w-full" />
              <Skeleton className="h-24 w-full" />
            </div>
          ) : null}

          {isError ? (
            <div className="rounded-lg border border-destructive/40 bg-destructive/5 p-4 text-sm">
              <p className="font-medium text-destructive">
                Could not load session details
              </p>
              <p className="mt-1 text-muted-foreground">
                {error?.message ?? "Something went wrong."}
              </p>
              <Button
                type="button"
                variant="outline"
                size="sm"
                className="mt-3"
                onClick={() => refetch()}
              >
                Try again
              </Button>
            </div>
          ) : null}

          {data ? (
            <ScrollArea className="h-[min(420px,calc(90vh-220px))] pr-3">
              <div className="space-y-6 pb-2">
                <section className="space-y-2">
                  <h3 className="flex items-center gap-2 text-sm font-semibold text-foreground">
                    <Target className="h-4 w-4 text-primary" />
                    Session summary
                  </h3>
                  <div className="flex flex-wrap gap-2 text-sm text-muted-foreground">
                    <span className="inline-flex items-center gap-1.5 rounded-md border border-border/60 bg-secondary/40 px-2 py-1">
                      <Dumbbell className="h-3.5 w-3.5" />
                      {data.training.name}
                    </span>
                    {data.finishedOnUtc ? (
                      <span className="inline-flex items-center gap-1.5 rounded-md border border-border/60 bg-secondary/40 px-2 py-1">
                        <Clock className="h-3.5 w-3.5" />
                        {formatDurationMs(
                          new Date(data.finishedOnUtc).getTime() -
                            new Date(data.startedOnUtc).getTime(),
                        )}
                      </span>
                    ) : null}
                    {typeof data.caloriesBurned === "number" ? (
                      <span className="inline-flex items-center gap-1.5 rounded-md border border-border/60 bg-secondary/40 px-2 py-1">
                        <Flame className="h-3.5 w-3.5 text-primary" />
                        {data.caloriesBurned.toLocaleString()} kcal
                      </span>
                    ) : null}
                  </div>
                </section>

                <SessionExerciseLists data={data} />
              </div>
            </ScrollArea>
          ) : null}
        </div>

        <div className="shrink-0 border-t border-border px-6 py-3">
          <Button type="button" variant="secondary" className="w-full" onClick={onClose}>
            Close
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
