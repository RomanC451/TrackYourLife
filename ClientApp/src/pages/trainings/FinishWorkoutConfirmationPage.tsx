import { useState } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "@tanstack/react-router";
import { CheckCircle2, X } from "lucide-react";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
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
    <PageCard className="max-w-2xl">
      <PageTitle title="Finish Workout?" />
      <Card className="flex w-full flex-col items-center bg-secondary p-8 shadow-lg">
        <div className="mb-4 text-center">
          <p className="mb-2 text-lg font-semibold">
            You've completed all exercises in your workout!
          </p>
          <p className="text-secondary-foreground">
            Would you like to finish the workout or go back to an exercise?
          </p>
        </div>

        <div className="mb-6 w-full space-y-2">
          <Label htmlFor="calories-burned">Calories Burned (optional)</Label>
          <Input
            id="calories-burned"
            type="number"
            min="0"
            placeholder="Enter calories burned"
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
            Optional: Enter the number of calories you burned during this
            workout
          </p>
        </div>

        <div className="mt-6 flex w-full flex-col gap-4 sm:flex-row">
          <Button
            variant="outline"
            className="flex-1"
            onClick={() => setIsExerciseSelectionOpen(true)}
            disabled={finishOngoingTrainingMutation.isPending}
          >
            <X className="mr-2 size-4" />
            Choose Exercise
          </Button>
          <ButtonWithLoading
            variant="default"
            className="flex-1"
            onClick={handleFinish}
            isLoading={finishOngoingTrainingMutation.isDelayedPending}
            disabled={finishOngoingTrainingMutation.isPending}
          >
            <CheckCircle2 className="mr-2 size-4" />
            Finish Workout
          </ButtonWithLoading>
        </div>

        <Button
          variant="ghost"
          className="mt-4"
          onClick={handleBack}
          disabled={finishOngoingTrainingMutation.isPending}
        >
          Back to Workout
        </Button>
      </Card>

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
