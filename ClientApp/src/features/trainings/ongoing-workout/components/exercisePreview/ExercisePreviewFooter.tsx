import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { ArrowRightIcon, SkipForwardIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { useWorkoutTimerContext } from "@/features/trainings/common/components/workoutTimer/WorkoutTimerContext";
import { OngoingTrainingDto } from "@/services/openapi";

import useNextOngoingTrainingMutation from "../../mutations/useNextOngoingTrainingMutation";
import useSkipExerciseMutation from "../../mutations/useSkipExerciseMutation";
import ExerciseSelectionDialog from "../exerciseSelection/ExerciseSelectionDialog";

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
          startTimer();
        },
      },
    );
  };

  const isAnyMutationPending =
    nextOngoingTrainingMutation.isPending || skipExerciseMutation.isPending;

  return (
    <>
      <div className="mt-2 flex flex-col gap-2 lg:flex-row lg:justify-between">
        {/* Skip and Choose Exercise - First row on mobile, Left on desktop */}
        <div className="grid grid-cols-2 gap-2 lg:flex lg:flex-row lg:gap-2">
          <ButtonWithLoading
            variant="outline"
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
            className="w-full rounded-lg px-4 py-2"
            disabled={isAnyMutationPending || ongoingTraining.isLoading}
            onClick={() => setIsExerciseSelectionOpen(true)}
          >
            Choose Exercise
          </Button>
        </div>
        {/* Next - Second row on mobile, Right on desktop */}
        <div className="grid grid-cols-1 gap-2 lg:flex lg:flex-row lg:gap-2">
          <ButtonWithLoading
            variant="default"
            className="w-full rounded-lg px-4 py-2"
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
