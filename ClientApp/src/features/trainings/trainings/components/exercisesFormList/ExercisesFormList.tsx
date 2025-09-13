import { useState } from "react";

import { MutationPendingState } from "@/hooks/useCustomMutation";

import OrderExercisesFormStep from "./orderExercisesFormStep/OrderExercisesFormStep";
import SelectExercisesFormStep from "./selectExercisesFormStep/SelectExercisesFormStep";

export type ExercisesFormStep = "select" | "order";

export default function ExercisesFormList({
  pendingState,
  onCancel,
  submitButtonText,
}: {
  pendingState: MutationPendingState;
  onCancel: () => void;
  submitButtonText: string;
}) {
  const [step, setStep] = useState<ExercisesFormStep>("select");

  return step === "select" ? (
    <SelectExercisesFormStep setStep={setStep} onCancel={onCancel} />
  ) : (
    <OrderExercisesFormStep
      setStep={setStep}
      onCancel={onCancel}
      pendingState={pendingState}
      submitButtonText={submitButtonText}
    />
  );
}
