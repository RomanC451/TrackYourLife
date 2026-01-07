import { zodResolver } from "@hookform/resolvers/zod";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { RotateCcw } from "lucide-react";
import { useFieldArray, useForm } from "react-hook-form";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Card } from "@/components/ui/card";
import { Form, FormField } from "@/components/ui/form";
import {
  ExerciseSetChangesSchema,
  exerciseSetChangesSchema,
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
  defaultValues: ExerciseSetChangesSchema;
  exercise: ExerciseDto;
}) {
  const form = useForm<ExerciseSetChangesSchema>({
    resolver: zodResolver(exerciseSetChangesSchema),
    defaultValues: defaultValues,
  });

  const { fields: fieldsArray } = useFieldArray({
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
        newSets: data.newSets,
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
    if (!original) return;

    form.setValue(`newSets.${idx}.count1`, original.count1);
    form.setValue(`newSets.${idx}.count2`, original.count2);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        {fieldsArray.map((fieldElement, idx) => {
          const actualSetId = form.getValues(`newSets.${idx}.id`);

          return (
            <Card key={fieldElement.id} className="flex flex-col space-y-4 p-4">
              <div className="flex items-center justify-between">
                <h1 className="font-semibold">{fieldElement?.name}</h1>
                <Button
                  variant="ghost"
                  type="button"
                  onClick={() => {
                    handleReset(idx, actualSetId);
                  }}
                >
                  <RotateCcw className="h-4 w-4" /> Reset
                </Button>
              </div>

              <div className="flex justify-around gap-4">
                <FormField
                  control={form.control}
                  name={`newSets.${idx}.count1`}
                  render={({ field }) => (
                    <SetChangeField
                      field={field}
                      label={fieldElement.unit1}
                      unit={fieldElement.unit1}
                      originalValue={fieldElement.count1}
                      step={1}
                    />
                  )}
                />
                {fieldElement.count2 !== undefined &&
                fieldElement.unit2 !== undefined ? (
                  <FormField
                    control={form.control}
                    name={`newSets.${idx}.count2`}
                    render={({ field }) => (
                      <SetChangeField
                        field={field}
                        label={fieldElement.unit2!}
                        unit={fieldElement.unit2!}
                        originalValue={fieldElement.count2!}
                        step={1}
                      />
                    )}
                  />
                ) : null}
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
