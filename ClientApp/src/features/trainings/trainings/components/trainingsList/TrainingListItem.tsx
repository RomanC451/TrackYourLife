import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import {
  Clock,
  MoreVertical,
  Pencil,
  Play,
  Trash2,
  X,
} from "lucide-react";
import { toast } from "sonner";

import { router } from "@/App";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { getDifficultyColor } from "@/features/trainings/exercises/utils/exercisesUtils";
import { MuscleGroupWorkoutIcon } from "@/features/trainings/utils/muscleGroupWorkoutIcon";
import useCreateOngoingTrainingMutation from "@/features/trainings/ongoing-workout/mutations/useCreateOngoingTrainingMutation";
import useDeleteOngoingTrainingMutation from "@/features/trainings/ongoing-workout/mutations/useDeleteOngoingTrainingMutation";
import { formatDuration } from "@/lib/time";
import { cn } from "@/lib/utils";
import { TrainingDto } from "@/services/openapi";

import DeleteTrainingAlert from "../common/DeleteTrainingAlert";

function TrainingListItem({
  training,
  isActive,
  isInActivePlan,
}: {
  training: TrainingDto;
  isActive: boolean;
  isInActivePlan: boolean;
}) {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const validExercises = training.exercises.filter(
    (exercise): exercise is NonNullable<(typeof training.exercises)[number]> =>
      Boolean(exercise),
  );
  const previewExercises = validExercises.slice(0, 3);
  const moreExerciseCount = validExercises.length - previewExercises.length;
  const navigate = useNavigate();
  const createOngoingTrainingMutation = useCreateOngoingTrainingMutation();
  const deleteOngoingTrainingMutation = useDeleteOngoingTrainingMutation();
  return (
    <Card key={training.id} className="overflow-hidden @container">
      <CardHeader className="pt-2">
        <div className="flex items-start justify-between gap-2">
          <CardTitle className="mt-4 min-w-0 pr-2">{training.name}</CardTitle>
          <div className="mt-4 flex shrink-0 items-center gap-1">
            <DropdownMenu modal={false}>
              <DropdownMenuTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8 shrink-0"
                  disabled={
                    training.isLoading ||
                    training.isDeleting ||
                    createOngoingTrainingMutation.isPending ||
                    deleteOngoingTrainingMutation.isPending
                  }
                  aria-label="Training actions"
                >
                  <MoreVertical className="h-4 w-4" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuItem
                  className="cursor-pointer"
                  disabled={
                    training.isLoading ||
                    training.isDeleting ||
                    createOngoingTrainingMutation.isPending ||
                    deleteOngoingTrainingMutation.isPending
                  }
                  onSelect={(e) => {
                    e.preventDefault();
                    if (isActive) {
                      toast.error(
                        "You can't edit an ongoing training, please finish or cancel it first. ",
                      );
                      return;
                    }
                    navigate({
                      to: "/trainings/workouts/edit/$workoutId",
                      params: { workoutId: training.id },
                    });
                  }}
                  onPointerEnter={() => {
                    if (!isActive) {
                      router.preloadRoute({
                        to: "/trainings/workouts/edit/$workoutId",
                        params: { workoutId: training.id },
                      });
                    }
                  }}
                >
                  <Pencil className="mr-2 h-4 w-4" />
                  Edit
                </DropdownMenuItem>
                <DropdownMenuItem
                  className="cursor-pointer text-destructive focus:text-destructive"
                  disabled={
                    training.isLoading ||
                    training.isDeleting ||
                    createOngoingTrainingMutation.isPending ||
                    deleteOngoingTrainingMutation.isPending
                  }
                  onSelect={(e) => {
                    e.preventDefault();
                    setDeleteDialogOpen(true);
                  }}
                >
                  <Trash2 className="mr-2 h-4 w-4" />
                  Delete
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
            <DeleteTrainingAlert
              training={training}
              force={isActive}
              open={deleteDialogOpen}
              onOpenChange={setDeleteDialogOpen}
              disabled={
                training.isLoading ||
                training.isDeleting ||
                createOngoingTrainingMutation.isPending ||
                deleteOngoingTrainingMutation.isPending
              }
            />
          </div>
        </div>
      </CardHeader>
      <CardContent>
        <div className="mb-3 flex gap-4 text-sm text-muted-foreground">
          <div className="flex items-center text-primary">
            <MuscleGroupWorkoutIcon
              muscleGroups={training.muscleGroups}
              className="mr-1 size-6 shrink-0"
            />
            {training.muscleGroups.join(", ")}
          </div>
        </div>
        <div className="mb-3 flex flex-wrap items-center gap-3 text-sm text-muted-foreground">
          <Badge
            variant="outline"
            className={cn(
              getDifficultyColor(training.difficulty),
              "font-normal",
            )}
          >
            {training.difficulty}
          </Badge>
          {training.duration ? (
            <div className="flex items-center">
              <Clock className="mr-1 size-4 shrink-0" />
              {formatDuration(training.duration)}
            </div>
          ) : null}
          <span>{validExercises.length} exercises</span>
        </div>
        {previewExercises.length > 0 && (
          <div
            className={cn(
              "rounded-lg bg-muted/50 p-3",
              training.isLoading && "opacity-50",
            )}
          >
            <ul className="space-y-2">
              {previewExercises.map((exercise, index) => (
                <li key={exercise.id} className="flex items-center gap-2">
                  <span className="flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-primary/15 text-xs font-medium text-primary">
                    {index + 1}
                  </span>
                  <span className="text-sm">{exercise.name}</span>
                </li>
              ))}
            </ul>
            {moreExerciseCount > 0 && (
              <p className="mt-2 text-sm text-muted-foreground">
                +{moreExerciseCount} more exercise
                {moreExerciseCount === 1 ? "" : "s"}
              </p>
            )}
          </div>
        )}
      </CardContent>
      <CardFooter className="flex flex-wrap items-center gap-2">
        {isInActivePlan && (
          <span className="flex items-center gap-1.5 text-sm text-primary">
            <span
              className="h-2 w-2 shrink-0 rounded-full bg-destructive"
              aria-hidden
            />
            <span>In active plan</span>
          </span>
        )}
        <div className="ml-auto flex flex-wrap items-center justify-end gap-2">
          {isActive && (
            <ButtonWithLoading
              className="gap-1 text-red-600"
              variant="secondary"
              disabled={training.isLoading || training.isDeleting}
              isLoading={deleteOngoingTrainingMutation.isDelayedPending}
              onClick={() => {
                deleteOngoingTrainingMutation.mutate({
                  ongoingTrainingId: training.id,
                });
              }}
            >
              <X className="h-4 w-4" /> Cancel
            </ButtonWithLoading>
          )}

          <ButtonWithLoading
            className="gap-1"
            disabled={
              training.isLoading ||
              training.isDeleting ||
              createOngoingTrainingMutation.isPending ||
              deleteOngoingTrainingMutation.isPending
            }
            onClick={() => {
              if (isActive) {
                navigate({ to: "/trainings/ongoing-workout" });
              } else {
                createOngoingTrainingMutation.mutate(
                  { trainingId: training.id },
                  {
                    onSuccess: () => {
                      navigate({ to: "/trainings/ongoing-workout" });
                    },
                  },
                );
              }
            }}
            isLoading={createOngoingTrainingMutation.isDelayedPending}
          >
            <Play className="h-4 w-4" /> {isActive ? "Continue" : "Start"}
          </ButtonWithLoading>
        </div>
      </CardFooter>
    </Card>
  );
}

export default TrainingListItem;
