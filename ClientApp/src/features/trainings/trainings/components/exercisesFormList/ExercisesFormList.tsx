import { useState } from "react";
import { UseFormReturn } from "react-hook-form";

import { LoadingState } from "@/hooks/useDelayedLoading";
import { ExerciseDto } from "@/services/openapi";

import { TrainingFormSchema } from "../../data/trainingsSchemas";
import OrderExercisesFormStep from "./orderExercisesFormStep/OrderExercisesFormStep";
import SelectExercisesFormStep from "./selectExercisesFormStep/SelectExercisesFormStep";

export type ExercisesFormStep = "select" | "order";

export default function ExercisesFormList({
  form,
  isPending,
  onCancel,
}: {
  form: UseFormReturn<TrainingFormSchema>;
  isPending: LoadingState;
  onCancel: () => void;
}) {
  const [step, setStep] = useState<ExercisesFormStep>("select");

  const selectedExercises = form.watch("exercises") || [];

  const removeExerciseFromForm = (exerciseId: string) => {
    const currentExercises = form.getValues("exercises") || [];
    form.setValue(
      "exercises",
      currentExercises.filter((e) => e.id !== exerciseId),
    );
  };

  const updateExerciseInForm = (exercise: Partial<ExerciseDto>) => {
    const currentExercises = form.getValues("exercises") || [];

    if (exercise.id == undefined) {
      return;
    }

    form.setValue(
      "exercises",
      currentExercises.map((e) =>
        e.id === exercise.id ? { ...e, ...exercise } : e,
      ),
    );
  };

  if (step === "order") {
    return (
      <OrderExercisesFormStep
        selectedExercises={selectedExercises as ExerciseDto[]}
        setStep={setStep}
        setFormExercises={(value) => form.setValue("exercises", value)}
        removeExerciseFromForm={removeExerciseFromForm}
        onCancel={onCancel}
        isPending={isPending}
      />
    );
  }

  return (
    <SelectExercisesFormStep
      selectedExercises={selectedExercises as ExerciseDto[]}
      removeExerciseFromForm={removeExerciseFromForm}
      updateExerciseInForm={updateExerciseInForm}
      setFormExercises={(value) => form.setValue("exercises", value)}
      getFormExercises={() =>
        (form.getValues("exercises") as ExerciseDto[]) || []
      }
      onCancel={onCancel}
      setStep={setStep}
    />
  );
}
