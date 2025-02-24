import { useEffect } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Form } from "@/components/ui/form";
import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";

import {
  calculateNutritionGoalsFormSchema,
  CalculateNutritionGoalsFormSchema,
} from "../../data/schemas";
import useCalculateNutritionGoalsMutation from "../../mutations/useCalculateNutritionGoalsMutation";
import ActivityLevelFormSelect from "./ActivityLevelFormSelect";
import AgeFormField from "./AgeFormField";
import FitnessGoalFormSelect from "./FitnessGoalFormSelect";
import GenderFormSelect from "./GenderFormSelect";
import HeightFormField from "./HeightFormField";
import WeightFormField from "./WeightFormField";

function CalculateNutritionGoalsForm({ onSuccess }: { onSuccess: () => void }) {
  const { userData } = useAuthenticationContext();
  const storageKey = `nutrition-calculator-form-${userData?.id}`;

  const lastSubmittedKey = `${storageKey}-last-submitted`;

  const form = useForm<CalculateNutritionGoalsFormSchema>({
    resolver: zodResolver(calculateNutritionGoalsFormSchema),
    defaultValues: JSON.parse(localStorage.getItem(storageKey) ?? "null") || {
      age: 0,
      weight: 0,
      height: 0,
      gender: undefined,
      activityLevel: undefined,
      fitnessGoal: undefined,
    },
  });
  const { calculateNutritionGoalsMutation, isPending } =
    useCalculateNutritionGoalsMutation();

  // Save form values to localStorage when they change
  useEffect(() => {
    if (userData?.id) {
      const subscription = form.watch((value) => {
        localStorage.setItem(storageKey, JSON.stringify(value));
      });
      return () => subscription.unsubscribe();
    }
  }, [form, userData?.id, storageKey]);

  function onSubmit(values: CalculateNutritionGoalsFormSchema) {
    calculateNutritionGoalsMutation.mutate(
      { ...values, force: true },
      {
        onSuccess: () => {
          localStorage.setItem(lastSubmittedKey, JSON.stringify(values));
          onSuccess();
        },
      },
    );
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        <AgeFormField form={form} />
        <WeightFormField form={form} />
        <HeightFormField form={form} />
        <GenderFormSelect form={form} />
        <ActivityLevelFormSelect form={form} />
        <FitnessGoalFormSelect form={form} />
        <ButtonWithLoading
          type="submit"
          className="w-full"
          isLoading={isPending.isLoading}
          disabled={!isPending.isLoaded}
        >
          Calculate
        </ButtonWithLoading>

        <Button
          onClick={onSuccess}
          disabled={!isPending.isLoaded}
          variant="outline"
          className="w-full"
        >
          Results
        </Button>
      </form>
    </Form>
  );
}

export default CalculateNutritionGoalsForm;
