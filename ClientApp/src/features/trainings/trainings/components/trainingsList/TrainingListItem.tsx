import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { ChevronDown, ChevronUp, Clock, Play, Target, X } from "lucide-react";
import { toast } from "sonner";

import { router } from "@/App";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { getDifficultyColor } from "@/features/trainings/exercises/exercisesList/exercisesListItem";
import useCreateOngoingTrainingMutation from "@/features/trainings/ongoing-workout/mutations/useCreateOngoingTrainingMutation";
import useDeleteOngoingTrainingMutation from "@/features/trainings/ongoing-workout/mutations/useDeleteOngoingTrainingMutation";
import { formatDuration } from "@/lib/time";
import { cn } from "@/lib/utils";
import { TrainingDto } from "@/services/openapi";

import DeleteTrainingAlert from "../common/DeleteTrainingAlert";

function TrainingListItem({
  training,
  isActive,
}: {
  training: TrainingDto;
  isActive: boolean;
}) {
  const [detailsShown, setDetailsShown] = useState(false);
  const navigate = useNavigate();
  const createOngoingTrainingMutation = useCreateOngoingTrainingMutation();
  const deleteOngoingTrainingMutation = useDeleteOngoingTrainingMutation();

  return (
    <Card key={training.id} className="overflow-hidden @container">
      <CardHeader className="pt-2">
        <div className="flex items-start justify-between">
          <CardTitle className="mt-4">{training.name}</CardTitle>
          <div className="flex flex-col items-end gap-1">
            <Badge
              variant="outline"
              className={cn(getDifficultyColor(training.difficulty), "w-fit")}
            >
              {training.difficulty}
            </Badge>
            <Badge variant="outline" className="w-fit">
              {training.exercises.length} exercises
            </Badge>
          </div>
        </div>
      </CardHeader>
      <CardContent>
        <div className="mb-4 flex gap-4 text-sm text-muted-foreground">
          <div className="flex items-center text-primary">
            <Target className="mr-1 size-4" />
            {training.muscleGroups.join(", ")}
          </div>
        </div>
        <div className="mb-4 flex gap-4 text-sm text-muted-foreground">
          <div className="flex items-center">
            <Clock className="mr-1 size-4" />
            {training.duration ? formatDuration(training.duration) : ""}
          </div>
        </div>
        {detailsShown && (
          <div
            className={cn(
              "mt-4 border-y py-2",
              training.isLoading && "opacity-50",
            )}
          >
            <h3 className="mb-2 font-medium">Exercises:</h3>
            <ul className="space-y-2 px-2">
              {training.exercises.map((exercise, index) => {
                return exercise ? (
                  <li key={exercise.id} className="flex items-center gap-2">
                    <span className="flex h-6 w-6 items-center justify-center rounded-full bg-primary/10 text-xs text-primary">
                      {index + 1}
                    </span>
                    <span>{exercise.name}</span>
                  </li>
                ) : null;
              })}
            </ul>
          </div>
        )}
      </CardContent>
      <CardFooter className="flex flex-col justify-between gap-2 @2xl:flex-row">
        <div className="flex w-full justify-between">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => setDetailsShown(!detailsShown)}
          >
            {detailsShown ? (
              <>
                <ChevronUp className="mr-1 h-4 w-4" /> Hide details
              </>
            ) : (
              <>
                <ChevronDown className="mr-1 h-4 w-4" /> Show details
              </>
            )}
          </Button>

          <div className="inline-flex gap-2">
            {isActive ? (
              <Button
                variant="outline"
                onClick={() => {
                  toast.error(
                    "You can't edit an ongoing training, please finish or cancel it first. ",
                  );
                }}
                disabled={
                  training.isLoading ||
                  training.isDeleting ||
                  createOngoingTrainingMutation.isPending ||
                  deleteOngoingTrainingMutation.isPending
                }
              >
                Edit
              </Button>
            ) : (
              <Button
                variant="outline"
                disabled={
                  training.isLoading ||
                  training.isDeleting ||
                  createOngoingTrainingMutation.isPending ||
                  deleteOngoingTrainingMutation.isPending
                }
                onClick={() => {
                  navigate({
                    to: "/trainings/workouts/edit/$workoutId",
                    params: { workoutId: training.id },
                  });
                }}
                onMouseEnter={() => {
                  router.preloadRoute({
                    to: "/trainings/workouts/edit/$workoutId",
                    params: { workoutId: training.id },
                  });
                }}
                onTouchStart={() => {
                  router.preloadRoute({
                    to: "/trainings/workouts/edit/$workoutId",
                    params: { workoutId: training.id },
                  });
                }}
              >
                Edit
              </Button>
            )}
            <DeleteTrainingAlert
              training={training}
              force={isActive}
              disabled={
                training.isLoading ||
                training.isDeleting ||
                createOngoingTrainingMutation.isPending ||
                deleteOngoingTrainingMutation.isPending
              }
            />
          </div>
        </div>

        {isActive && (
          <ButtonWithLoading
            className="w-full gap-1 text-red-600 @2xl:w-auto"
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
          className="w-full gap-1 @2xl:w-auto"
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
      </CardFooter>
    </Card>
  );
}

export default TrainingListItem;
