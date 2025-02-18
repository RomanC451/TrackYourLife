import { UseFormReturn } from "react-hook-form";

import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { FitnessGoal } from "@/services/openapi";

import { CalculateNutritionGoalsFormSchema } from "../../data/schemas";

function FitnessGoalFormSelect({
  form,
}: {
  form: UseFormReturn<CalculateNutritionGoalsFormSchema>;
}) {
  return (
    <FormField
      key="fitnessGoal"
      control={form.control}
      name="fitnessGoal"
      render={({ field }) => (
        <FormItem>
          <FormLabel>Goal</FormLabel>
          <Select
            onValueChange={field.onChange}
            defaultValue={field.value}
            key="fitnessGoalInput"
          >
            <FormControl>
              <SelectTrigger>
                <SelectValue placeholder="Select fitness goal" />
              </SelectTrigger>
            </FormControl>
            <SelectContent>
              <SelectItem value={FitnessGoal.WeightLoss}>
                Weight Loss
              </SelectItem>
              <SelectItem value={FitnessGoal.MuscleGain}>
                Muscle Gain
              </SelectItem>
              <SelectItem value={FitnessGoal.Maintain}>Maintenance</SelectItem>
            </SelectContent>
          </Select>
          <FormMessage />
        </FormItem>
      )}
    />
  );
}

export default FitnessGoalFormSelect;
