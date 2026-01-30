import { useState } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "@tanstack/react-router";
import { ArrowLeft, CheckCircle2, Dumbbell, Flame, Trophy } from "lucide-react";

import PageCard from "@/components/common/PageCard";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import ExerciseSelectionDialog from "@/features/trainings/ongoing-workout/components/exerciseSelection/ExerciseSelectionDialog";
import useFinishOngoingTrainingMutation from "@/features/trainings/ongoing-workout/mutations/useFinishOngoingTrainingMutation";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { queryClient } from "@/queryClient";

function FinishWorkoutConfirmationPage() {
  const navigate = useNavigate();
  const params = useParams({
    from: "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/ongoing-workout/finish-workout-confirmation/$ongoingTrainingId",
  });
  const { data: ongoingTraining } = useSuspenseQuery(
    ongoingTrainingsQueryOptions.byId(params.ongoingTrainingId),
  );

  const [isExerciseSelectionOpen, setIsExerciseSelectionOpen] = useState(false);
  const [caloriesBurned, setCaloriesBurned] = useState<number | "">("");
  const finishOngoingTrainingMutation = useFinishOngoingTrainingMutation();

  const handleFinish = () => {
    finishOngoingTrainingMutation.mutate(
      {
        ongoingTraining,
        caloriesBurned:
          caloriesBurned === "" ? undefined : Number(caloriesBurned),
      },
      {
        onSuccess: () => {
          queryClient
            .ensureQueryData(
              ongoingTrainingsQueryOptions.byId(ongoingTraining.id),
            )
            .then(() => {
              navigate({
                to: "/trainings/workout-finished/$ongoingTrainingId",
                params: { ongoingTrainingId: ongoingTraining.id },
              });
            });
        },
      },
    );
  };

  const handleBack = () => {
    navigate({
      to: "/trainings/ongoing-workout",
    });
  };

  return (
    <PageCard className="flex max-w-xl flex-1">
      <div className=" bg-background">
        {/* Decorative background elements */}
        <div className="pointer-events-none absolute inset-0 overflow-hidden">
          <div className="absolute top-0 left-1/2 h-[400px] w-[800px] -translate-x-1/2 rounded-full bg-primary/5 blur-3xl" />
          <div className="absolute right-0 bottom-0 h-[300px] w-[600px] rounded-full bg-primary/5 blur-3xl" />
        </div>

        <div className="relative z-10 flex flex-col items-center justify-center px-4 py-8">
          {/* Header */}
          <div className="mb-8 text-center">
            <div className="mb-6 inline-flex h-20 w-20 items-center justify-center rounded-full border border-primary/20 bg-primary/10">
              <Trophy className="h-10 w-10 text-primary" />
            </div>
            <h1 className="mb-2 text-4xl font-bold tracking-tight text-foreground md:text-5xl">
              Workout Complete
            </h1>
            <p className="text-lg text-muted-foreground">
              You crushed it today!
            </p>
          </div>

          {/* Main Card */}
          <Card className="w-full max-w-md border-border/50 bg-card/80 shadow-2xl backdrop-blur-sm">
            <CardContent className="p-6 md:p-8">
              {/* Success Badge */}
              <div className="border-success/20 bg-success/10 mb-6 flex items-center gap-3 rounded-xl border p-4">
                <CheckCircle2 className="text-success h-6 w-6 shrink-0" />
                <div>
                  <p className="font-semibold text-foreground">
                    All exercises completed!
                  </p>
                  <p className="text-sm text-muted-foreground">
                    Ready to finish or continue?
                  </p>
                </div>
              </div>

              {/* Calories Input */}
              <div className="mb-6">
                <label className="mb-2 flex items-center gap-2 text-sm font-medium text-foreground">
                  <Flame className="h-4 w-4 text-primary" />
                  Calories Burned
                  <span className="font-normal text-muted-foreground">
                    (optional)
                  </span>
                </label>
                <Input
                  type="number"
                  placeholder="e.g. 350"
                  value={caloriesBurned !== 0 ? caloriesBurned : ""}
                  onChange={(e) => setCaloriesBurned(Number(e.target.value))}
                  className="h-12 border-border bg-input text-foreground placeholder:text-muted-foreground focus:ring-2 focus:ring-primary/50"
                />
                <p className="mt-2 text-xs text-muted-foreground">
                  Track your estimated calorie burn for this session
                </p>
              </div>

              {/* Action Buttons */}
              <div className="mb-4 flex flex-col gap-3 sm:flex-row">
                <Button
                  variant="outline"
                  onClick={() => setIsExerciseSelectionOpen(true)}
                  className="h-12 flex-1 gap-2"
                >
                  <Dumbbell className="h-4 w-4" />
                  Choose exercise
                </Button>
                <Button
                  onClick={handleFinish}
                  className="h-12 flex-1 gap-2 bg-primary font-semibold text-primary-foreground hover:bg-primary/90"
                >
                  <CheckCircle2 className="h-4 w-4" />
                  Finish Workout
                </Button>
              </div>

              {/* Back Link */}
              <button
                onClick={handleBack}
                className="flex w-full items-center justify-center gap-2 py-3 text-sm text-muted-foreground transition-colors hover:text-foreground"
              >
                <ArrowLeft className="h-4 w-4" />
                Back to Workout
              </button>
            </CardContent>
          </Card>

          {/* Motivational footer */}
          <p className="mt-8 max-w-sm text-center text-sm text-muted-foreground">
            Every workout counts. Keep pushing your limits!
          </p>
        </div>
      </div>

      <ExerciseSelectionDialog
        open={isExerciseSelectionOpen}
        onOpenChange={setIsExerciseSelectionOpen}
        ongoingTraining={ongoingTraining}
        hideCurrentStatus={true}
        onSelectionSuccess={() => {
          navigate({
            to: "/trainings/ongoing-workout",
          });
        }}
      />
    </PageCard>
  );
}

export default FinishWorkoutConfirmationPage;
