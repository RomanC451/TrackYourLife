import { zodResolver } from "@hookform/resolvers/zod";
import { RotateCcw } from "lucide-react";
import { useFieldArray, useForm } from "react-hook-form";

import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Form } from "@/components/ui/form";
import {
  exerciseSetChangeFormSchema,
  ExerciseSetChangeFormSchema,
} from "@/features/trainings/exercises/data/exercisesSchemas";
import { ExerciseSet } from "@/services/openapi";

import useAdjustExerciseMutation from "../../mutations/useAdjustExerciseMutation";
import SetChangeField from "./SetChangeField";

function ExerciseSetChangeForm({
  defaultValues,
  exerciseSets,
  exerciseId,
  onSuccess,
  ongoingTrainingId,
}: {
  defaultValues: ExerciseSetChangeFormSchema["changes"];
  exerciseSets: ExerciseSet[];
  exerciseId: string;
  onSuccess: () => void;
  ongoingTrainingId: string;
}) {
  const form = useForm<ExerciseSetChangeFormSchema>({
    resolver: zodResolver(exerciseSetChangeFormSchema),
    defaultValues: { changes: defaultValues },
  });

  const { fields } = useFieldArray({
    control: form.control,
    name: "changes",
  });

  const { adjustExerciseMutation } = useAdjustExerciseMutation();

  const onSubmit = (data: ExerciseSetChangeFormSchema) => {
    // For now, just log the data
    console.log("Submitted changes:", data);

    adjustExerciseMutation.mutate(
      {
        ongoingTrainingId: ongoingTrainingId,
        exerciseId: exerciseId,
        changes: data.changes.map((change) => ({
          setId: change.setId,
          weightChange:
            change.newWeight -
            exerciseSets.find((set) => set.id === change.setId)!.weight,
          repsChange:
            change.newReps -
            exerciseSets.find((set) => set.id === change.setId)!.reps,
        })),
      },
      {
        onSuccess: () => {
          onSuccess();
        },
      },
    );
  };

  // Helper to reset a set to its original values
  const handleReset = (idx: number, setId: string) => {
    const original = exerciseSets.find((set) => set.id === setId);
    if (original) {
      form.setValue(`changes.${idx}.newWeight`, original.weight);
      form.setValue(`changes.${idx}.newReps`, original.reps);
    }
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        {fields.map((field, idx) => {
          const original = exerciseSets.find((set) => set.id === field.setId);
          if (!original) return null;
          return (
            <Card key={field.id} className="flex flex-col space-y-4 p-4">
              <div className="flex items-center justify-between">
                <h1 className="font-semibold">{original?.name}</h1>
                <Button
                  variant="ghost"
                  type="button"
                  onClick={() => handleReset(idx, field.setId)}
                >
                  <RotateCcw className="h-4 w-4" /> Reset
                </Button>
              </div>
              <div className="flex gap-20">
                <SetChangeField
                  original={original}
                  field={field}
                  idx={idx}
                  type="reps"
                />
                <SetChangeField
                  original={original}
                  field={field}
                  idx={idx}
                  type="weight"
                />
              </div>
            </Card>
          );
        })}
        <div className="flex justify-end">
          <Button type="submit">Save adjustments</Button>
        </div>
      </form>
    </Form>
  );
}

export default ExerciseSetChangeForm;
