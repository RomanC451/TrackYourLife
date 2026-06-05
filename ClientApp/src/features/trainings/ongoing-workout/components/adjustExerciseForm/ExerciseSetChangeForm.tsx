import { zodResolver } from "@hookform/resolvers/zod";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { RotateCcw } from "lucide-react";
import { useFieldArray, useForm } from "react-hook-form";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Card, CardContent } from "@/components/ui/card";
import { Form, FormField } from "@/components/ui/form";
import {
  ExerciseSetChangesSchema,
  exerciseSetChangesSchema,
} from "@/features/trainings/exercises/data/exercisesSchemas";
import { cn } from "@/lib/utils";
import { queryClient } from "@/queryClient";
import { ExerciseDto } from "@/services/openapi";

import useAdjustExerciseMutation from "../../mutations/useAdjustExerciseMutation";
import useNextOngoingTrainingMutation from "../../mutations/useNextOngoingTrainingMutation";
import { ongoingTrainingsQueryOptions } from "../../queries/ongoingTrainingsQuery";
import SetChangeField from "./SetChangeField";

function getStepForUnit(unit: string) {
  return unit === "kg" ? 0.25 : 1;
}

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

  const watchedSets = form.watch("newSets");

  const { data: activeOngoingTraining } = useSuspenseQuery(
    ongoingTrainingsQueryOptions.active,
  );

  const adjustExerciseMutation = useAdjustExerciseMutation();

  const nextOngoingTrainingMutation = useNextOngoingTrainingMutation();

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
                  queryClient
                    .fetchQuery(ongoingTrainingsQueryOptions.active)
                    .then((updatedTraining) => {
                      if (!updatedTraining.hasNext) {
                        navigate({
                          to: "/trainings/ongoing-workout/finish-workout-confirmation/$ongoingTrainingId",
                          params: {
                            ongoingTrainingId: activeOngoingTraining.id,
                          },
                        });
                        return;
                      }
                      navigate({
                        to: "/trainings/ongoing-workout",
                      });
                    })
                    .catch(() => {
                      navigate({
                        to: "/trainings/ongoing-workout",
                      });
                    });
                },
              },
            );
          } else {
            navigate({
              to: "/trainings/ongoing-workout/finish-workout-confirmation/$ongoingTrainingId",
              params: { ongoingTrainingId: activeOngoingTraining.id },
            });
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

  const setHasChanges = (idx: number) => {
    const watched = watchedSets[idx];
    const original = exercise.exerciseSets[idx];
    if (!watched || !original) return false;

    return (
      watched.count1 !== original.count1 ||
      (original.count2 !== undefined &&
        watched.count2 !== undefined &&
        watched.count2 !== original.count2)
    );
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-3">
        {fieldsArray.map((fieldElement, idx) => {
          const actualSetId = form.getValues(`newSets.${idx}.id`);
          const hasChanges = setHasChanges(idx);

          return (
            <Card
              key={fieldElement.id}
              className={cn(
                "border-border/50 bg-card/80 backdrop-blur-sm transition-all",
                hasChanges && "ring-1 ring-primary/30",
              )}
            >
              <CardContent className="p-4">
                <div className="mb-4 flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className="flex h-7 w-7 items-center justify-center rounded-lg bg-primary/15 text-sm font-bold text-primary">
                      {idx + 1}
                    </div>
                    <span className="font-semibold">{fieldElement?.name}</span>
                    {hasChanges ? (
                      <Badge
                        variant="outline"
                        className="ml-2 border-primary/30 text-xs text-primary"
                      >
                        Modified
                      </Badge>
                    ) : null}
                  </div>
                  <Button
                    variant="ghost"
                    size="sm"
                    type="button"
                    onClick={() => {
                      handleReset(idx, actualSetId);
                    }}
                    className="h-8 gap-1.5 text-xs text-muted-foreground hover:text-foreground"
                    disabled={!hasChanges}
                  >
                    <RotateCcw className="h-3.5 w-3.5" />
                    Reset
                  </Button>
                </div>

                <div
                  className={cn(
                    "grid gap-4",
                    fieldElement.count2 !== undefined &&
                      fieldElement.unit2 !== undefined
                      ? "grid-cols-2"
                      : "grid-cols-1",
                  )}
                >
                  <FormField
                    control={form.control}
                    name={`newSets.${idx}.count1`}
                    render={({ field }) => (
                      <SetChangeField
                        field={field}
                        label={fieldElement.unit1}
                        unit={fieldElement.unit1}
                        originalValue={fieldElement.count1}
                        step={getStepForUnit(fieldElement.unit1)}
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
                          step={getStepForUnit(fieldElement.unit2!)}
                        />
                      )}
                    />
                  ) : null}
                </div>
              </CardContent>
            </Card>
          );
        })}
        <ButtonWithLoading
          type="submit"
          size="lg"
          className="w-full font-semibold shadow-lg shadow-primary/25"
          isLoading={
            adjustExerciseMutation.pendingState.isDelayedPending ||
            nextOngoingTrainingMutation.pendingState.isDelayedPending
          }
          disabled={
            adjustExerciseMutation.pendingState.isPending ||
            nextOngoingTrainingMutation.pendingState.isPending
          }
        >
          Save adjustments
        </ButtonWithLoading>
      </form>
    </Form>
  );
}

export default ExerciseSetChangeForm;
