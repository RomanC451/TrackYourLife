import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "@tanstack/react-router";
import { ArrowLeftIcon, CheckCircle2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { exerciseHistoryQueryKeys } from "@/features/trainings/ongoing-workout/queries/exerciseHistoryQuery";
import {
  ongoingTrainingsQueryKeys,
  ongoingTrainingsQueryOptions,
} from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { formatDurationMs } from "@/lib/time";
import { queryClient } from "@/queryClient";

function WorkoutFinished() {
  const navigate = useNavigate();
  const params = useParams({
    from: "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workout-finished/$ongoingTrainingId",
  });
  const { data: ongoingTraining } = useSuspenseQuery(
    ongoingTrainingsQueryOptions.byId(params.ongoingTrainingId),
  );

  const trainingName = ongoingTraining.training.name;
  const exercisesCompleted = ongoingTraining.training.exercises.length;

  const duration = formatDurationMs(
    new Date(ongoingTraining.finishedOnUtc!).getTime() -
      new Date(ongoingTraining.startedOnUtc).getTime(),
  );

  return (
    <div className="flex w-full flex-col items-center justify-center">
      <Card className="flex w-full max-w-md flex-col items-center bg-secondary p-8 shadow-lg">
        <h2 className="mb-1 text-2xl font-bold">Great job!</h2>
        <p className="mb-6 text-secondary-foreground">
          You've completed your workout
        </p>
        <CheckCircle2 className="my-4 h-24 w-24 text-green-500" />
        <div className="mb-6 mt-2 w-full text-left">
          <div className="mb-2 font-semibold">Workout Summary</div>
          <div className="text-secondary-foreground">
            Training: <span className="font-medium">{trainingName}</span>
          </div>
          <div className="text-secondary-foreground">
            Exercises completed:{" "}
            <span className="font-medium">{exercisesCompleted}</span>
          </div>
          <div className="text-secondary-foreground">
            Time taken: <span className="font-medium">{duration}</span>
          </div>
        </div>
        <div className="mt-4 flex w-full gap-4">
          <Button
            variant="default"
            className="flex-1"
            onClick={() => {
              queryClient.removeQueries({
                queryKey: ongoingTrainingsQueryKeys.active,
              });
              queryClient.removeQueries({
                queryKey: exerciseHistoryQueryKeys.all,
              });
              navigate({ to: "/trainings/workouts" });
            }}
          >
            <ArrowLeftIcon className="mr-2 size-4" /> Back to Home
          </Button>
        </div>
      </Card>
    </div>
  );
}

export default WorkoutFinished;
