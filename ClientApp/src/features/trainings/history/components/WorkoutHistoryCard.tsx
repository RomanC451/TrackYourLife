import { format } from "date-fns";
import {
  Clock,
  Eye,
  Flame,
  ListChecks,
  MoreVertical,
  SlidersHorizontal,
} from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { MuscleGroupWorkoutIcon } from "@/features/trainings/utils/muscleGroupWorkoutIcon";
import { formatDurationMs } from "@/lib/time";
import { cn } from "@/lib/utils";
import type { WorkoutHistoryDto } from "@/services/openapi";

type WorkoutHistoryCardProps = {
  workout: WorkoutHistoryDto;
  /** Same source as workout list cards: `TrainingDto.muscleGroups` for `workout.trainingId`. */
  muscleGroups?: readonly string[] | null;
  isNewest?: boolean;
  onEditWorkout?: (workout: WorkoutHistoryDto) => void;
  onViewSessionDetails: (workout: WorkoutHistoryDto) => void;
};

export function WorkoutHistoryCard({
  workout,
  muscleGroups,
  isNewest,
  onEditWorkout,
  onViewSessionDetails,
}: WorkoutHistoryCardProps) {
  const goToDetails = () => {
    onViewSessionDetails(workout);
  };

  const finishedAt = new Date(workout.finishedOnUtc);
  const calories = workout.caloriesBurned;

  return (
    <Card
      className={cn(
        "group relative cursor-pointer border-border bg-card transition-all duration-300 hover:border-primary/40 hover:bg-secondary",
        isNewest && "ring-1 ring-primary/50",
      )}
      onClick={goToDetails}
    >
      {isNewest && (
        <div className="absolute -top-2 left-4">
          <Badge className="bg-primary text-xs text-primary-foreground">
            Most recent
          </Badge>
        </div>
      )}

      <CardContent className="p-5">
        <div className="flex items-center justify-between gap-4">
          <div className="flex items-center gap-4">
            <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/10 text-primary">
              <MuscleGroupWorkoutIcon
                muscleGroups={muscleGroups}
                className="h-6 w-6"
              />
            </div>

            <div className="space-y-1.5">
              <div className="flex flex-wrap items-center gap-2">
                <h3 className="text-lg font-semibold">{workout.trainingName}</h3>
              </div>

              <div className="flex flex-wrap items-center gap-2">
                <span className="text-sm text-muted-foreground">
                  {format(finishedAt, "MMM d, yyyy")} at{" "}
                  {format(finishedAt, "h:mm a")}
                </span>
              </div>

              <div className="flex flex-wrap items-center gap-4 pt-2">
                <div className="flex items-center gap-1.5 text-muted-foreground">
                  <ListChecks className="h-4 w-4 shrink-0" />
                  <span className="text-sm font-medium">
                    {`${workout.completedExercisesCount}/${workout.totalExercisesCount} completed exercises`}
                  </span>
                </div>
                <div className="flex items-center gap-1.5 text-muted-foreground">
                  <Clock className="h-4 w-4" />
                  <span className="text-sm font-medium">
                    {formatDurationMs(workout.durationSeconds * 1000)}
                  </span>
                </div>
                {calories != null && (
                  <div className="flex items-center gap-1.5">
                    <Flame className="h-4 w-4 text-primary" />
                    <span className="text-sm font-medium text-primary">
                      {calories.toLocaleString()} kcal
                    </span>
                  </div>
                )}
              </div>
            </div>
          </div>

          <DropdownMenu modal={false}>
            <DropdownMenuTrigger asChild onClick={(e) => e.stopPropagation()}>
              <Button
                variant="ghost"
                size="icon"
              >
                <MoreVertical className="h-4 w-4" />
                <span className="sr-only">Open menu</span>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuItem
                onClick={(e) => {
                  e.stopPropagation();
                  goToDetails();
                }}
              >
                <Eye className="mr-2 h-4 w-4" />
                View details
              </DropdownMenuItem>
              {onEditWorkout ? (
                <DropdownMenuItem
                  onClick={(e) => {
                    e.stopPropagation();
                    onEditWorkout(workout);
                  }}
                >
                  <SlidersHorizontal className="mr-2 h-4 w-4" />
                  Edit session
                </DropdownMenuItem>
              ) : null}
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </CardContent>
    </Card>
  );
}
