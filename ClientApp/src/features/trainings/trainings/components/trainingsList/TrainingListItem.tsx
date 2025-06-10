import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { ChevronDown, ChevronUp, Clock, Play, X } from "lucide-react";
import { toast } from "sonner";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import useCreateOngoingTrainingMutation from "@/features/trainings/ongoing-workout/mutations/useCreateOngoingTrainingMutation";
import useDeleteOngoingTrainingMutation from "@/features/trainings/ongoing-workout/mutations/useDeleteOngoingTrainingMutation";
import { formatDuration } from "@/lib/time";
import { TrainingDto } from "@/services/openapi";

import DeleteTrainingAlert from "../common/DeleteTrainingAlert";
import EditTrainingDialog from "../trainingsDialogs/EditTrainingDialog";

function TrainingListItem({
  training,
  isActive,
}: {
  training: TrainingDto;
  isActive: boolean;
}) {
  const [detailsShown, setDetailsShown] = useState(false);
  const navigate = useNavigate();
  const { createOngoingTrainingMutation, isPending: isCreating } =
    useCreateOngoingTrainingMutation();
  const { deleteOngoingTrainingMutation, isPending: isDeleting } =
    useDeleteOngoingTrainingMutation();

  return (
    <Card key={training.id} className="overflow-hidden @container">
      <CardHeader>
        <div className="flex items-start justify-between">
          <CardTitle>{training.name}</CardTitle>
          <Badge variant="outline">{training.exercises.length} exercises</Badge>
        </div>
        <CardDescription>{training.description}</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="mb-4 flex gap-4 text-sm text-muted-foreground">
          <div className="flex items-center">
            <Clock className="mr-1 h-4 w-4" />
            {training.duration ? formatDuration(training.duration) : ""}
          </div>
        </div>
        {detailsShown && (
          <div className="mt-4 rounded-md border bg-muted/50 p-4">
            <h3 className="mb-2 font-medium">Exercises:</h3>
            <ul className="space-y-2">
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
              >
                Edit
              </Button>
            ) : (
              <EditTrainingDialog training={training} />
            )}
            <DeleteTrainingAlert training={training} force={isActive} />
          </div>
        </div>

        {isActive && (
          <ButtonWithLoading
            className="w-full gap-1 text-red-600 @2xl:w-auto"
            variant="secondary"
            disabled={!isDeleting.isLoaded}
            isLoading={isDeleting.isLoading}
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
          disabled={!isCreating.isLoaded}
          isLoading={isCreating.isLoading}
        >
          <Play className="h-4 w-4" /> {isActive ? "Continue" : "Start"}
        </ButtonWithLoading>
      </CardFooter>
    </Card>
  );
}

export default TrainingListItem;
