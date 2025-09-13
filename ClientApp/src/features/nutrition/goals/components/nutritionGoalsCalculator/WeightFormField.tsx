import { useFormContext } from "react-hook-form";

import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";

function WeightFormField() {
  const { control } = useFormContext();

  return (
    <FormField
      control={control}
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
