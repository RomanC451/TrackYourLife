import { useNavigate } from "@tanstack/react-router";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { OngoingTrainingDto } from "@/services/openapi";

import useFinishOngoingTrainingMutation from "../../mutations/useFinishOngoingTrainingMutation";
import useNextOngoingTrainingMutation from "../../mutations/useNextOngoingTrainingMutation";
import usePreviousOngoingTrainingMutation from "../../mutations/usePreviousOngoingTrainingMutation";

function ExercisePreviewFooter({
  ongoingTraining,
}: {
  ongoingTraining: OngoingTrainingDto;
}) {
  const { nextOngoingTrainingMutation, isPending: isNextPending } =
    useNextOngoingTrainingMutation();
  const { previousOngoingTrainingMutation, isPending: isPreviousPending } =
    usePreviousOngoingTrainingMutation();

  const { finishOngoingTrainingMutation, isPending: isFinishPending } =
    useFinishOngoingTrainingMutation();

  const navigate = useNavigate();

  return (
    <div className="mt-2 flex justify-between">
      <div>
        {ongoingTraining.hasPrevious && (
          <ButtonWithLoading
            variant="secondary"
            className="rounded-lg px-4 py-2"
            disabled={!isPreviousPending.isLoaded || ongoingTraining.isLoading}
            isLoading={isPreviousPending.isLoading}
            onClick={() => {
              previousOngoingTrainingMutation.mutate({
                ongoingTraining,
              });
            }}
          >
            &lt;{" "}
            {ongoingTraining.isFirstSet ? "Previous exercise" : "Previous set"}
          </ButtonWithLoading>
        )}
      </div>
      <div>
        {ongoingTraining.hasNext ? (
          <ButtonWithLoading
            variant="default"
            className="rounded-lg px-4 py-2"
            disabled={!isNextPending.isLoaded || ongoingTraining.isLoading}
            isLoading={isNextPending.isLoading}
            onClick={() => {
              nextOngoingTrainingMutation.mutate({
                ongoingTraining,
              });
            }}
          >
            {ongoingTraining.isLastSet ? "Next exercise" : "Next set"} &gt;
          </ButtonWithLoading>
        ) : (
          <Button
            variant="default"
            className="rounded-lg px-4 py-2"
            disabled={ongoingTraining.isLoading || isFinishPending.isLoading}
            onClick={() => {
              finishOngoingTrainingMutation.mutate(
                {
                  ongoingTraining,
                },
                {
                  onSuccess: () => {
                    navigate({
                      to: "/trainings/workout-finished/$ongoingTrainingId",
                      params: { ongoingTrainingId: ongoingTraining.id },
                    });
                  },
                },
              );
            }}
          >
            Finish
          </Button>
        )}
      </div>
    </div>
  );
}

export default ExercisePreviewFooter;
