import { useState } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "@tanstack/react-router";
import {
  ArrowLeft,
  CheckCircle2,
  Clock,
  Dumbbell,
  Flame,
  Trophy,
} from "lucide-react";

import PageCard from "@/components/common/PageCard";
import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
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
      <div className="flex flex-1 flex-col items-center justify-start pt-[30%]">
        {/* Header Section with Trophy Icon */}
        <div className="mb-8 flex flex-col items-center">
          <Trophy className="mb-4 size-16 text-primary" />
          <h1 className="mb-2 text-4xl font-bold text-foreground">
            Workout Complete
          </h1>
          <p className="text-lg text-muted-foreground">You crushed it today!</p>
        </div>

        {/* Main Content Card */}
        <Card className="flex w-full flex-col bg-card-secondary p-6 shadow-lg">
          {/* Completion Status Section with Green Background */}
          <div className="mb-6 rounded-lg bg-green-600 p-4">
            <div className="flex items-center gap-3">
              <div className="flex size-8 items-center justify-center rounded-full bg-green-500">
                <CheckCircle2 className="size-5 text-white" />
              </div>
              <div className="flex-1">
                <p className="font-semibold text-white">
                  All exercises completed!
                </p>
                <p className="text-sm text-white/90">
                  Ready to finish or continue?
                </p>
              </div>
            </div>
          </div>

          {/* Calories Burned Input */}
          <div className="mb-6 space-y-2">
            <Label
              htmlFor="calories-burned"
              className="flex items-center gap-2"
            >
              <Flame className="size-4 text-primary" />
              <span>Calories Burned (optional)</span>
            </Label>
            <Input
              id="calories-burned"
              type="number"
              min="0"
              placeholder="e.g. 350"
              value={caloriesBurned}
              onChange={(e) => {
                const value = e.target.value;
                if (value === "") {
                  setCaloriesBurned("");
                } else {
                  const numValue = Number.parseInt(value, 10);
                  if (!Number.isNaN(numValue) && numValue >= 0) {
                    setCaloriesBurned(numValue);
                  }
                }
              }}
              disabled={finishOngoingTrainingMutation.isPending}
              className="bg-background/50"
            />
            <p className="text-xs text-muted-foreground">
              Track your estimated calorie burn for this session
            </p>
          </div>

          {/* Action Buttons */}
          <div className="mb-4 flex w-full flex-col gap-4 sm:flex-row">
            <Button
              variant="outline"
              className="flex-1"
              onClick={() => setIsExerciseSelectionOpen(true)}
              disabled={finishOngoingTrainingMutation.isPending}
            >
              <Dumbbell className="mr-2 size-4" />
              Choose exercise
            </Button>
            <ButtonWithLoading
              variant="default"
              className="flex-1 bg-primary text-primary-foreground hover:bg-primary/90"
              onClick={handleFinish}
              isLoading={finishOngoingTrainingMutation.isDelayedPending}
              disabled={finishOngoingTrainingMutation.isPending}
            >
              <Clock className="mr-2 size-4" />
              Finish Workout
            </ButtonWithLoading>
          </div>

          {/* Back to Workout Link */}
          <Button
            variant="ghost"
            className="w-full justify-center"
            onClick={handleBack}
            disabled={finishOngoingTrainingMutation.isPending}
          >
            <ArrowLeft className="mr-2 size-4" />
            Back to Workout
          </Button>
        </Card>

        {/* Motivational Message */}
        <p className="mt-8 text-center text-sm text-muted-foreground">
          Every workout counts. Keep pushing your limits!
        </p>
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
