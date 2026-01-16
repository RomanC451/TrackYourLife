import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { ArrowRightIcon, CheckCircle2, SkipForwardIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { screensEnum } from "@/constants/tailwindSizes";
import { useAppGeneralStateContext } from "@/contexts/AppGeneralContextProvider";
import { useWorkoutTimerContext } from "@/features/trainings/common/components/workoutTimer/WorkoutTimerContext";
import { queryClient } from "@/queryClient";
import { OngoingTrainingDto } from "@/services/openapi";

import useNextOngoingTrainingMutation from "../../mutations/useNextOngoingTrainingMutation";
import useSkipExerciseMutation from "../../mutations/useSkipExerciseMutation";
import { ongoingTrainingsQueryOptions } from "../../queries/ongoingTrainingsQuery";
import ExerciseSelectionDialog from "../exerciseSelection/ExerciseSelectionDialog";

/**
 * Checks if all exercises in the training are completed or skipped
 */
function areAllExercisesCompletedOrSkipped(
  ongoingTraining: OngoingTrainingDto,
): { allCompleted: boolean; incompleteExercises: string[] } {
  const allExerciseIds =
    ongoingTraining.training?.exercises?.map((ex) => ex.id) || [];
  const completedIds = ongoingTraining.completedExerciseIds || [];
  const skippedIds = ongoingTraining.skippedExerciseIds || [];
  const completedOrSkippedIds = new Set([...completedIds, ...skippedIds]);

  const incompleteExercises = allExerciseIds.filter(
    (id) => !completedOrSkippedIds.has(id),
  );

  return {
    allCompleted: incompleteExercises.length === 0,
    incompleteExercises,
  };
}

function ExercisePreviewFooter({
  ongoingTraining,
}: {
  ongoingTraining: OngoingTrainingDto;
}) {
  const navigate = useNavigate();
  const [isExerciseSelectionOpen, setIsExerciseSelectionOpen] = useState(false);

  const nextOngoingTrainingMutation = useNextOngoingTrainingMutation();
  const skipExerciseMutation = useSkipExerciseMutation();

  const { isTimerPlaying, startTimer } = useWorkoutTimerContext();
  const { screenSize } = useAppGeneralStateContext();

  // Check if all exercises are completed or skipped
  const { allCompleted } = areAllExercisesCompletedOrSkipped(ongoingTraining);

  // Determine button size based on screen width (sm for smaller devices)
  const buttonSize = screenSize.width < screensEnum.lg ? "sm" : "default";

  const handleFinish = () => {
    navigate({
      to: "/trainings/ongoing-workout/finish-workout-confirmation/$ongoingTrainingId",
      params: { ongoingTrainingId: ongoingTraining.id },
    });
  };

  const handleNext = () => {
    if (ongoingTraining.isLastSet) {
      if (!ongoingTraining.isLastExercise) {
        startTimer();
      }
      navigate({
        to: "/trainings/ongoing-workout/adjust-exercise/$exerciseId",
        params: {
          exerciseId:
            ongoingTraining.training.exercises[ongoingTraining.exerciseIndex]
              .id,
        },
      });
      return;
    }
    nextOngoingTrainingMutation.mutate(
      {
        ongoingTraining,
      },
      {
        onSuccess: () => {
          startTimer();
        },
      },
    );
  };

  const handleSkip = () => {
    skipExerciseMutation.mutate(
      {
        ongoingTraining,
      },
      {
        onSuccess: () => {
          // Wait for the query to refetch after invalidation
          queryClient
            .fetchQuery(ongoingTrainingsQueryOptions.active)
            .then((updatedTraining) => {
              // Check if after the mutation there are no more exercises
              if (!updatedTraining.hasNext) {
                navigate({
                  to: "/trainings/ongoing-workout/finish-workout-confirmation/$ongoingTrainingId",
                  params: {
                    ongoingTrainingId: ongoingTraining.id,
                  },
                });
              }
            });
        },
      },
    );
  };

  const isAnyMutationPending =
    nextOngoingTrainingMutation.isPending || skipExerciseMutation.isPending;

  return (
    <>
      <div className="fixed bottom-0 left-0 right-0 z-10 mt-2 bg-background/95 p-4 shadow-lg backdrop-blur-sm lg:relative lg:left-auto lg:right-auto lg:z-auto lg:bg-transparent lg:p-0 lg:shadow-none">
        <div className="flex flex-col gap-2 lg:flex-row lg:justify-between">
          {/* Left side: Skip and Choose Exercise */}
          <div className="grid grid-cols-2 gap-2 lg:flex lg:flex-row lg:gap-2">
            <ButtonWithLoading
              variant="outline"
              size={buttonSize}
              className="w-full rounded-lg px-4 py-2"
              disabled={isAnyMutationPending || ongoingTraining.isLoading}
              isLoading={skipExerciseMutation.isDelayedPending}
              onClick={handleSkip}
            >
              <SkipForwardIcon className="mr-2 size-4" />
              Skip
            </ButtonWithLoading>
            <Button
              variant="outline"
              size={buttonSize}
              className="w-full rounded-lg px-4 py-2"
              disabled={isAnyMutationPending || ongoingTraining.isLoading}
              onClick={() => setIsExerciseSelectionOpen(true)}
            >
              Choose Exercise
            </Button>
          </div>
          {/* Right side: Next and Finish (conditionally) - stacked on mobile, side by side on desktop */}
          <div className="flex flex-col gap-2 lg:flex-row lg:gap-2">
            <ButtonWithLoading
              variant="default"
              size={buttonSize}
              className="w-full rounded-lg px-4 py-2 lg:w-auto"
              disabled={
                isAnyMutationPending ||
                ongoingTraining.isLoading ||
                isTimerPlaying
              }
              isLoading={nextOngoingTrainingMutation.isDelayedPending}
              onClick={handleNext}
            >
              {ongoingTraining.isLastSet ? "Next exercise" : "Next set"}
              <ArrowRightIcon className="ml-2 size-4" />
            </ButtonWithLoading>
            {allCompleted && (
              <Button
                variant="default"
                size={buttonSize}
                className="w-full rounded-lg bg-green-600 px-4 py-2 text-white hover:bg-green-700 lg:w-auto"
                disabled={isAnyMutationPending || ongoingTraining.isLoading}
                onClick={handleFinish}
              >
                <CheckCircle2 className="mr-2 size-4" />
                Finish Workout
              </Button>
            )}
          </div>
        </div>
      </div>
      <ExerciseSelectionDialog
        open={isExerciseSelectionOpen}
        onOpenChange={setIsExerciseSelectionOpen}
        ongoingTraining={ongoingTraining}
      />
    </>
  );
}

export default ExercisePreviewFooter;
