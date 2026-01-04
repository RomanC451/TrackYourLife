import { useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useForm } from "react-hook-form";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Form, FormField, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import useUpdateNutritionGoalsMutation from "@/features/nutrition/goals/mutations/useUpdateNutritionGoalsMutation";
import { nutritionGoalsQueryOptions } from "@/features/nutrition/goals/queries/nutritionGoalsQuery";
import { dailyNutritionOverviewsQueryKeys } from "@/features/nutrition/overview/queries/useDailyNutritionOverviewsQuery";
import { getDateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";

import {
  UpdateNutritionGoalsFormSchema,
  updateNutritionGoalsFormSchema,
} from "../../data/nutritionGoalsSchemas";

function CalculateNutritionGoalsFormResults() {
  const [isEditing, setIsEditing] = useState(false);

  const { data: goals } = useSuspenseQuery(
    nutritionGoalsQueryOptions.byDate(getDateOnly(new Date())),
  );

  console.log(goals);

  const form = useForm<UpdateNutritionGoalsFormSchema>({
    resolver: zodResolver(updateNutritionGoalsFormSchema),
    defaultValues: {
      calories: goals?.calories.value ?? 0,
      proteins: goals?.proteins.value ?? 0,
      carbs: goals?.carbs.value ?? 0,
      fat: goals?.fat.value ?? 0,
    },
  });

  const updateNutritionGoalsMutation = useUpdateNutritionGoalsMutation();
  const toggleEdit = () => {
    form.setValue("calories", goals?.calories.value ?? 0);
    form.setValue("proteins", goals?.proteins.value ?? 0);
    form.setValue("carbs", goals?.carbs.value ?? 0);
    form.setValue("fat", goals?.fat.value ?? 0);
    form.reset();
    setIsEditing(!isEditing);
  };

  const onSubmit = (data: UpdateNutritionGoalsFormSchema) => {
    updateNutritionGoalsMutation.mutate(
      {
        calories: data.calories,
        protein: data.proteins,
        carbohydrates: data.carbs,
        fats: data.fat,
        force: true,
      },
      {
        onSuccess: () => {
          setIsEditing(false);
          queryClient.invalidateQueries({
            queryKey: dailyNutritionOverviewsQueryKeys.all,
          });
        },
      },
    );
  };

  if (!goals) return null;

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <div className="grid gap-4 py-4">
          {Object.entries(goals).map(([key]) => (
            <div
              key={key}
              className="grid grid-cols-4 items-center gap-4 gap-y-2"
            >
              <FormField
                control={form.control}
                name={key as keyof UpdateNutritionGoalsFormSchema}
                render={({ field }) => (
                  <>
                    <Label htmlFor={key} className="text-right capitalize">
                      {key}
                    </Label>
                    <Input
                      {...field}
                      type="number"
                      value={field.value == 0 ? "" : field.value}
                      onChange={(e) => field.onChange(Number(e.target.value))}
                      className="col-span-3"
                      disabled={!isEditing}
                    />
                    <div />
                    <FormMessage className="col-span-3" />
                  </>
                )}
              />
            </div>
          ))}
        </div>
        <div className="space-y-2">
          <div className="flex justify-end space-x-2">
            {isEditing ? (
              <>
                <ButtonWithLoading
                  isLoading={
                    updateNutritionGoalsMutation.pendingState.isDelayedPending
                  }
                  disabled={updateNutritionGoalsMutation.pendingState.isPending}
                  variant="default"
                  type="submit"
                >
                  Save Changes
                </ButtonWithLoading>
                <Button
                  type="button"
                  onClick={toggleEdit}
                  disabled={updateNutritionGoalsMutation.pendingState.isPending}
                  variant="outline"
                >
                  Cancel
                </Button>
              </>
            ) : (
              <Button onClick={toggleEdit} variant="default" type="button">
                Edit
              </Button>
            )}
          </div>
        </div>
      </form>
    </Form>
  );
}

export default CalculateNutritionGoalsFormResults;
