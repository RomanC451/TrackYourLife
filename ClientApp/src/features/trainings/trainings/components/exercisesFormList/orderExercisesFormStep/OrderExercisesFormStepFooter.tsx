import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { MutationPendingState } from "@/hooks/useCustomMutation";

import { ExercisesFormStep } from "../ExercisesFormList";

function OrderExercisesFormStepFooter({
  onCancel,
  setStep,
  submitButtonText,
  pendingState,
}: {
  onCancel: () => void;
  setStep: (step: ExercisesFormStep) => void;
  submitButtonText: string;
  pendingState: MutationPendingState;
}) {
  return (
    <div className="flex justify-end gap-2 pt-4">
      <Button variant="secondary" type="button" onClick={onCancel}>
        Cancel
      </Button>
      <Button variant="outline" onClick={() => setStep("select")} type="button">
        Back to Selection
      </Button>
      <ButtonWithLoading
        type="submit"
        disabled={pendingState.isPending}
        isLoading={pendingState.isDelayedPending}
        className="min-w-[100px]"
      >
        {submitButtonText}
      </ButtonWithLoading>
    </div>
  );
}

export default OrderExercisesFormStepFooter;
