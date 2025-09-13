import { useFormContext } from "react-hook-form";

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

function FitnessGoalFormSelect() {
  const { control } = useFormContext();
  return (
    <FormField
      key="fitnessGoal"
      control={control}
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
