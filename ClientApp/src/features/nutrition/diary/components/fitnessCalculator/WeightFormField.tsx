import { UseFormReturn } from "react-hook-form";

import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";

import { CalculateNutritionGoalsFormSchema } from "../../data/schemas";

function WeightFormField({
  form,
}: {
  form: UseFormReturn<CalculateNutritionGoalsFormSchema>;
}) {
  return (
    <FormField
      control={form.control}
      name="weight"
      render={({ field }) => (
        <FormItem>
          <FormLabel>Weight (kg)</FormLabel>
          <FormControl>
            <Input type="number" placeholder="0" {...field} />
          </FormControl>
          <FormMessage />
        </FormItem>
      )}
    />
  );
}

export default WeightFormField;
