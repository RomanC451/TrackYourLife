import { zodResolver } from "@hookform/resolvers/zod";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { RotateCcw } from "lucide-react";
import { useFieldArray, useForm } from "react-hook-form";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Card } from "@/components/ui/card";
import { Form } from "@/components/ui/form";
import {
  exerciseSetChangeFormSchema,
  ExerciseSetChangeFormSchema,
} from "@/features/trainings/exercises/data/exercisesSchemas";
import { ExerciseDto } from "@/services/openapi";

import useAdjustExerciseMutation from "../../mutations/useAdjustExerciseMutation";
import useFinishOngoingTrainingMutation from "../../mutations/useFinishOngoingTrainingMutation";
import useNextOngoingTrainingMutation from "../../mutations/useNextOngoingTrainingMutation";
import { ongoingTrainingsQueryOptions } from "../../queries/ongoingTrainingsQuery";
import SetChangeField from "./SetChangeField";

function ExerciseSetChangeForm({
  defaultValues,
  exercise,
}: {
  defaultValues: ExerciseSetChangeFormSchema["changes"];
  exercise: ExerciseDto;
}) {
  const form = useForm<ExerciseSetChangeFormSchema>({
    resolver: zodResolver(exerciseSetChangeFormSchema),
    defaultValues: { changes: defaultValues },
  });

  const { fields } = useFieldArray({
    control: form.control,
    name: "changes",
  });

  const { data: activeOngoingTraining } = useSuspenseQuery(
    ongoingTrainingsQueryOptions.active,
  );

  const adjustExerciseMutation = useAdjustExerciseMutation();

  const nextOngoingTrainingMutation = useNextOngoingTrainingMutation();

  const finishOngoingTrainingMutation = useFinishOngoingTrainingMutation();

  const navigate = useNavigate();

  const onSubmit = (data: ExerciseSetChangeFormSchema) => {
    adjustExerciseMutation.mutate(
      {
        ongoingTrainingId: activeOngoingTraining.id,
        exerciseId: exercise.id,
        changes: data.changes.map((change) => ({
          setId: change.setId,
          weightChange:
            change.newWeight -
            exercise.exerciseSets.find((set) => set.id === change.setId)!
              .weight,
          repsChange:
            change.newReps -
            exercise.exerciseSets.find((set) => set.id === change.setId)!.reps,
        })),
      },
      {
        onSuccess: () => {
          if (activeOngoingTraining.hasNext) {
            nextOngoingTrainingMutation.mutate(
              {
                ongoingTraining: activeOngoingTraining,
              },
              {
                onSuccess: () => {
                  navigate({
                    to: "/trainings/ongoing-workout",
                  });
                },
              },
            );
          } else {
            finishOngoingTrainingMutation.mutate(
              {
                ongoingTraining: activeOngoingTraining,
              },
              {
                onSuccess: () => {
                  navigate({
                    to: "/trainings/ongoing-workout/workout-finished/$ongoingTrainingId",
                    params: { ongoingTrainingId: activeOngoingTraining.id },
                  });
                },
              },
            );
          }
        },
      },
    );
  };

  const handleReset = (idx: number, setId: string) => {
    const original = exercise.exerciseSets.find((set) => set.id === setId);
    if (original) {
      form.setValue(`changes.${idx}.newWeight`, original.weight);
      form.setValue(`changes.${idx}.newReps`, original.reps);
    }
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        {fields.map((field, idx) => {
          const original = exercise.exerciseSets.find(
            (set) => set.id === field.setId,
          );
          if (!original) return null;
          return (
            <Card key={field.id} className="flex flex-col space-y-4 p-4">
              <div className="flex items-center justify-between">
                <h1 className="font-semibold">
                  Set {idx + 1} · {original?.name}
                </h1>
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
          <ButtonWithLoading
            type="submit"
            isLoading={
              adjustExerciseMutation.pendingState.isDelayedPending ||
              finishOngoingTrainingMutation.pendingState.isDelayedPending ||
              nextOngoingTrainingMutation.pendingState.isDelayedPending
            }
            disabled={
              adjustExerciseMutation.pendingState.isPending ||
              finishOngoingTrainingMutation.pendingState.isPending ||
              nextOngoingTrainingMutation.pendingState.isPending
            }
          >
            Save adjustments
          </ButtonWithLoading>
        </div>
      </form>
    </Form>
  );
}

export default ExerciseSetChangeForm;
