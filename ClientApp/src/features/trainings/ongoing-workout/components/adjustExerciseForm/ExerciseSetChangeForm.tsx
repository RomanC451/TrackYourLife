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
  ExerciseSetChangesSchema,
  exerciseSetChangesSchema,
} from "@/features/trainings/exercises/data/exercisesSchemas";
import { exerciseSetSchemaToApiExerciseSet } from "@/features/trainings/exercises/utils/exerciseSetsMappings";
import { ExerciseDto, ExerciseSetType } from "@/services/openapi";

import useAdjustExerciseMutation from "../../mutations/useAdjustExerciseMutation";
import useFinishOngoingTrainingMutation from "../../mutations/useFinishOngoingTrainingMutation";
import useNextOngoingTrainingMutation from "../../mutations/useNextOngoingTrainingMutation";
import { ongoingTrainingsQueryOptions } from "../../queries/ongoingTrainingsQuery";
import BodyweightBasedSetChangeForm from "./setChangesForms/BodyWeightBasedSetChangeForm";
import DistanceBasedSetChangeForm from "./setChangesForms/DistanceBasedSetChangeForm";
import TimeBasedSetChangeForm from "./setChangesForms/TimeBasedSetChangeForm";
import WeightBasedSetChangeForm from "./setChangesForms/WeightBasedSetChangeForm";

function ExerciseSetChangeForm({
  defaultValues,
  exercise,
}: {
  defaultValues: ExerciseSetChangesSchema;
  exercise: ExerciseDto;
}) {
  const form = useForm<ExerciseSetChangesSchema>({
    resolver: zodResolver(exerciseSetChangesSchema),
    defaultValues: defaultValues,
  });

  const { fields } = useFieldArray({
    control: form.control,
    name: "newSets",
  });

  const { data: activeOngoingTraining } = useSuspenseQuery(
    ongoingTrainingsQueryOptions.active,
  );

  const adjustExerciseMutation = useAdjustExerciseMutation();

  const nextOngoingTrainingMutation = useNextOngoingTrainingMutation();

  const finishOngoingTrainingMutation = useFinishOngoingTrainingMutation();

  const navigate = useNavigate();

  const onSubmit = (data: ExerciseSetChangesSchema) => {
    adjustExerciseMutation.mutate(
      {
        ongoingTrainingId: activeOngoingTraining.id,
        exerciseId: exercise.id,
        newSets: data.newSets.map(exerciseSetSchemaToApiExerciseSet),
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
      if (original.type === ExerciseSetType.Weight) {
        form.setValue(`newSets.${idx}.weight`, original.weight ?? 0);
        form.setValue(`newSets.${idx}.reps`, original.reps ?? 0);
      }
      if (original.type === ExerciseSetType.Time) {
        form.setValue(
          `newSets.${idx}.durationSeconds`,
          original.durationSeconds ?? 0,
        );
      }
      if (original.type === ExerciseSetType.Bodyweight) {
        form.setValue(`newSets.${idx}.reps`, original.reps ?? 0);
      }
      if (original.type === ExerciseSetType.Distance) {
        form.setValue(`newSets.${idx}.distance`, original.distance ?? 0);
      }
    }
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        {fields.map((field, idx) => {
          const set = exerciseSetSchemaToApiExerciseSet(field);
          return (
            <Card key={field.id} className="flex flex-col space-y-4 p-4">
              <div className="flex items-center justify-between">
                <h1 className="font-semibold">
                  Set {idx + 1} · {field?.name}
                </h1>
                <Button
                  variant="ghost"
                  type="button"
                  onClick={() => handleReset(idx, field.id)}
                >
                  <RotateCcw className="h-4 w-4" /> Reset
                </Button>
              </div>

              {field.type === ExerciseSetType.Weight && (
                <WeightBasedSetChangeForm
                  original={set}
                  field={field}
                  idx={idx}
                />
              )}
              {field.type === ExerciseSetType.Time && (
                <TimeBasedSetChangeForm
                  original={set}
                  field={field}
                  idx={idx}
                />
              )}
              {field.type === ExerciseSetType.Bodyweight && (
                <BodyweightBasedSetChangeForm
                  original={set}
                  field={field}
                  idx={idx}
                />
              )}
              {field.type === ExerciseSetType.Distance && (
                <DistanceBasedSetChangeForm
                  original={set}
                  field={field}
                  idx={idx}
                />
              )}
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
