import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "@tanstack/react-router";
import { ArrowLeft, CheckCircle2, Clock, Dumbbell, Trophy } from "lucide-react";

import PageCard from "@/components/common/PageCard";
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
    <PageCard className="flex max-w-lg flex-1">
      <div className="flex flex-1 flex-col items-center justify-start pt-[30%]">
        {/* Header Section with Trophy Icon */}
        <div className="mb-8 flex flex-col items-center">
          <div className="mb-4 flex size-20 items-center justify-center rounded-full border-2 border-green-500">
            <Trophy className="size-12 text-primary" />
          </div>
          <h1 className="mb-2 text-4xl font-bold text-foreground">
            Great job!
          </h1>
          <p className="text-lg text-foreground">
            You've completed your workout
          </p>
        </div>

        {/* Main Content Card */}
        <Card className="flex w-full flex-col bg-card-secondary p-6 shadow-lg">
          {/* Large Checkmark Circle */}
          <div className="mb-6 flex justify-center">
            <div className="flex size-20 items-center justify-center rounded-full border-2 border-green-400 bg-green-500">
              <CheckCircle2 className="size-12 text-white" />
            </div>
          </div>

          {/* Workout Summary Title */}
          <div className="mb-6 flex items-center justify-center gap-2">
            <Dumbbell className="size-5 text-primary" />
            <h2 className="text-2xl font-bold text-foreground">
              Workout Summary
            </h2>
          </div>

          {/* Summary Details */}
          <div className="mb-6 space-y-4">
            {/* Training */}
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <Dumbbell className="size-5 text-primary" />
                <span className="text-foreground">Training</span>
              </div>
              <span className="font-semibold text-foreground">
                {trainingName}
              </span>
            </div>

            {/* Exercises Completed */}
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <CheckCircle2 className="size-5 text-green-500" />
                <span className="text-foreground">Exercises completed</span>
              </div>
              <span className="font-semibold text-foreground">
                {exercisesCompleted}
              </span>
            </div>

            {/* Time Taken */}
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <Clock className="size-5 text-primary" />
                <span className="text-foreground">Time taken</span>
              </div>
              <span className="font-semibold text-foreground">{duration}</span>
            </div>
          </div>

          {/* Back to Home Button */}
          <Button
            variant="default"
            className="w-full bg-primary text-primary-foreground hover:bg-primary/90"
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
            <ArrowLeft className="mr-2 size-4" />
            Back to Home
          </Button>
        </Card>

        {/* Motivational Message */}
        <p className="mt-8 text-center text-sm text-muted-foreground">
          Consistency is the key to progress. See you next time!
        </p>
      </div>
    </PageCard>
  );
}

export default WorkoutFinished;
