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

function HeightFormField({
  form,
}: {
  form: UseFormReturn<CalculateNutritionGoalsFormSchema>;
}) {
  return (
    <FormField
      control={form.control}
      name="height"
      render={({ field }) => (
        <FormItem>
          <FormLabel>Height (cm)</FormLabel>
          <FormControl>
            <Input type="number" placeholder="0" {...field} />
          </FormControl>
          <FormMessage />
        </FormItem>
      )}
    />
  );
}

export default HeightFormField;
