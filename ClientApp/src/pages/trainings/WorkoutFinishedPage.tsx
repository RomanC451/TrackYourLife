import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "@tanstack/react-router";
import {
  ArrowLeft,
  CheckCircle2,
  Clock,
  Dumbbell,
  Target,
  Trophy,
} from "lucide-react";

import PageCard from "@/components/common/PageCard";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
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

  const handleBackToHome = () => {
    queryClient.removeQueries({
      queryKey: ongoingTrainingsQueryKeys.active,
    });
    queryClient.removeQueries({
      queryKey: exerciseHistoryQueryKeys.all,
    });
    navigate({ to: "/trainings/workouts" });
  };

  return (
    <PageCard className="flex max-w-lg flex-1">
      <div className="mt-[30%] bg-background">
        {/* Decorative background elements */}
        <div className="pointer-events-none absolute inset-0 overflow-hidden">
          <div className="absolute top-0 left-1/2 h-[400px] w-[800px] -translate-x-1/2 rounded-full bg-primary/5 blur-3xl" />
          <div className="bg-success/5 absolute right-0 bottom-0 h-[300px] w-[600px] rounded-full blur-3xl" />
        </div>

        <div className="relative z-10 flex flex-col items-center justify-center px-4 py-8">
          {/* Header with Trophy */}
          <div className="mb-8 text-center">
            <div className="mb-6 inline-flex h-20 w-20 items-center justify-center rounded-full border border-primary/20 bg-primary/10">
              <Trophy className="h-10 w-10 text-primary" />
            </div>
            <h1 className="mb-2 text-4xl font-bold tracking-tight text-foreground md:text-5xl">
              Great job!
            </h1>
            <p className="text-lg text-muted-foreground">
              You&apos;ve completed your workout
            </p>
          </div>

          {/* Main Card */}
          <Card className="w-full max-w-md border-border/50 bg-card/80 shadow-2xl backdrop-blur-sm">
            <CardContent className="p-6 md:p-8">
              {/* Success Checkmark */}
              <div className="mb-8 flex justify-center">
                <div className="relative">
                  <div className="border-success bg-success/20 flex h-20 w-20 items-center justify-center rounded-full border-2">
                    <CheckCircle2 className="text-success h-10 w-10" />
                  </div>
                  {/* Animated ring effect */}
                  <div className="border-success/30 absolute inset-0 animate-ping rounded-full border-2" />
                </div>
              </div>

              {/* Workout Summary Section */}
              <div className="mb-8">
                <h2 className="mb-4 flex items-center gap-2 text-lg font-semibold text-foreground">
                  <Target className="h-5 w-5 text-primary" />
                  Workout Summary
                </h2>

                <div className="space-y-3">
                  {/* Training Name */}
                  <div className="flex items-center justify-between rounded-lg border border-border/50 bg-secondary/50 p-3">
                    <div className="flex items-center gap-3">
                      <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary/10">
                        <Dumbbell className="h-4 w-4 text-primary" />
                      </div>
                      <span className="text-muted-foreground">Training</span>
                    </div>
                    <span className="font-semibold text-foreground">
                      {trainingName}
                    </span>
                  </div>

                  {/* Exercises Completed */}
                  <div className="flex items-center justify-between rounded-lg border border-border/50 bg-secondary/50 p-3">
                    <div className="flex items-center gap-3">
                      <div className="bg-success/10 flex h-8 w-8 items-center justify-center rounded-lg">
                        <CheckCircle2 className="text-success h-4 w-4" />
                      </div>
                      <span className="text-muted-foreground">
                        Exercises completed
                      </span>
                    </div>
                    <span className="font-semibold text-foreground">
                      {exercisesCompleted}
                    </span>
                  </div>

                  {/* Time Taken */}
                  <div className="flex items-center justify-between rounded-lg border border-border/50 bg-secondary/50 p-3">
                    <div className="flex items-center gap-3">
                      <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary/10">
                        <Clock className="h-4 w-4 text-primary" />
                      </div>
                      <span className="text-muted-foreground">Time taken</span>
                    </div>
                    <span className="font-semibold text-foreground">
                      {duration}
                    </span>
                  </div>
                </div>
              </div>

              {/* Back to Home Button */}
              <Button
                onClick={handleBackToHome}
                className="h-12 w-full gap-2 bg-primary font-semibold text-primary-foreground hover:bg-primary/90"
              >
                <ArrowLeft className="h-4 w-4" />
                Back to Home
              </Button>
            </CardContent>
          </Card>

          {/* Motivational footer */}
          <p className="mt-8 max-w-sm text-center text-sm text-muted-foreground">
            Consistency is the key to progress. See you next time!
          </p>
        </div>
      </div>
    </PageCard>
  );
}

export default WorkoutFinished;
