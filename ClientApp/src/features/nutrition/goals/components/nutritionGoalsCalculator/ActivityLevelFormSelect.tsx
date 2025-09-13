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
import { ActivityLevel } from "@/services/openapi";

function ActivityLevelFormSelect() {
  const { control } = useFormContext();
  return (
    <FormField
      control={control}
      name="activityLevel"
      render={({ field }) => (
        <FormItem>
          <FormLabel>Activity Level</FormLabel>
          <Select onValueChange={field.onChange} defaultValue={field.value}>
            <FormControl>
              <SelectTrigger>
                <SelectValue placeholder="Select activity level" />
              </SelectTrigger>
            </FormControl>
            <SelectContent>
              <SelectItem value={ActivityLevel.Sedentary}>
                Sedentary (little or no exercise)
              </SelectItem>
              <SelectItem value={ActivityLevel.LightlyActive}>
                Lightly active (exercise 1–3 days/week)
              </SelectItem>
              <SelectItem value={ActivityLevel.ModeratelyActive}>
                Moderately active (exercise 3–5 days/week)
              </SelectItem>
              <SelectItem value={ActivityLevel.Active}>
                Active (exercise 6–7 days/week)
              </SelectItem>
              <SelectItem value={ActivityLevel.ExtremelyActive}>
                Extremely active (hard exercise 6–7 days/week)
              </SelectItem>
            </SelectContent>
          </Select>
          <FormMessage />
        </FormItem>
      )}
    />
  );
}

export default ActivityLevelFormSelect;
