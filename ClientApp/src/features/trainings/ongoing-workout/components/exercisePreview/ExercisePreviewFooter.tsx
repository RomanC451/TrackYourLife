import { useNavigate } from "@tanstack/react-router";
import { ArrowLeftIcon, ArrowRightIcon } from "lucide-react";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import { useWorkoutTimerContext } from "@/features/trainings/common/components/workoutTimer/WorkoutTimerContext";
import { OngoingTrainingDto } from "@/services/openapi";

import useNextOngoingTrainingMutation from "../../mutations/useNextOngoingTrainingMutation";
import usePreviousOngoingTrainingMutation from "../../mutations/usePreviousOngoingTrainingMutation";

function ExercisePreviewFooter({
  ongoingTraining,
}: {
  ongoingTraining: OngoingTrainingDto;
}) {
  const navigate = useNavigate();

  const nextOngoingTrainingMutation = useNextOngoingTrainingMutation();
  const previousOngoingTrainingMutation = usePreviousOngoingTrainingMutation();

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

  const handlePrevious = () => {
    previousOngoingTrainingMutation.mutate({
      ongoingTraining,
    });
  };

  return (
    <div className="mt-2 flex justify-between">
      <div>
        {ongoingTraining.hasPrevious && (
          <ButtonWithLoading
            variant="outline"
            className="rounded-lg px-4 py-2"
            disabled={
              previousOngoingTrainingMutation.isPending ||
              ongoingTraining.isLoading ||
              isTimerPlaying
            }
            isLoading={previousOngoingTrainingMutation.isDelayedPending}
            onClick={handlePrevious}
          >
            <ArrowLeftIcon className="ml-2 size-4" />
            {ongoingTraining.isFirstSet ? "Previous exercise" : "Previous set"}
          </ButtonWithLoading>
        )}
      </div>
      <div>
        <ButtonWithLoading
          variant="default"
          className="rounded-lg px-4 py-2"
          disabled={
            nextOngoingTrainingMutation.isPending ||
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
  );
}

export default ExercisePreviewFooter;
