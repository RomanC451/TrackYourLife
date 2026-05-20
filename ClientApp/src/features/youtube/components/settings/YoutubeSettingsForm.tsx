import { UseFormReturn, useWatch } from "react-hook-form";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { MutationPendingState } from "@/hooks/useCustomMutation";
import { DayOfWeek, SettingsChangeFrequency } from "@/services/openapi";

import { YoutubeSettingsFormSchema } from "../../data/youtubeSettingsSchemas";

interface YoutubeSettingsFormProps {
  form: UseFormReturn<YoutubeSettingsFormSchema>;
  pendingState: MutationPendingState;
}

function YoutubeSettingsForm({ form, pendingState }: YoutubeSettingsFormProps) {
  const settingsChangeFrequency = useWatch({
    control: form.control,
    name: "settingsChangeFrequency",
  });

  return (
    <div className="space-y-6">
      <FormField
        control={form.control}
        name="settingsChangeFrequency"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Settings Change Frequency</FormLabel>
            <FormDescription>
              How often you can change these settings.
            </FormDescription>
            <Select onValueChange={field.onChange} value={field.value}>
              <FormControl>
                <SelectTrigger>
                  <SelectValue placeholder="Select frequency" />
                </SelectTrigger>
              </FormControl>
              <SelectContent>
                <SelectItem value={SettingsChangeFrequency.OnceEveryFewDays}>
                  Once Every Few Days
                </SelectItem>
                <SelectItem value={SettingsChangeFrequency.SpecificDayOfWeek}>
                  Specific Day of Week
                </SelectItem>
                <SelectItem value={SettingsChangeFrequency.SpecificDayOfMonth}>
                  Specific Day of Month
                </SelectItem>
              </SelectContent>
            </Select>
            <FormMessage />
          </FormItem>
        )}
      />

      {settingsChangeFrequency ===
        SettingsChangeFrequency.OnceEveryFewDays && (
        <FormField
          control={form.control}
          name="daysBetweenChanges"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Days Between Changes</FormLabel>
              <FormDescription>
                Minimum number of days that must pass before you can change
                settings again.
              </FormDescription>
              <FormControl>
                <Input type="number" min="1" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      )}

      {settingsChangeFrequency ===
        SettingsChangeFrequency.SpecificDayOfWeek && (
        <FormField
          control={form.control}
          name="specificDayOfWeek"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Day of Week</FormLabel>
              <FormDescription>
                The day of the week when you can change settings.
              </FormDescription>
              <Select onValueChange={field.onChange} value={field.value}>
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder="Select day of week" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {Object.values(DayOfWeek).map((day) => (
                    <SelectItem key={day} value={day}>
                      {day}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />
      )}

      {settingsChangeFrequency ===
        SettingsChangeFrequency.SpecificDayOfMonth && (
        <FormField
          control={form.control}
          name="specificDayOfMonth"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Day of Month</FormLabel>
              <FormDescription>
                The day of the month (1-31) when you can change settings.
              </FormDescription>
              <FormControl>
                <Input type="number" min="1" max="31" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      )}

      <div className="flex justify-end gap-2">
        <ButtonWithLoading
          type="submit"
          isLoading={pendingState.isPending}
          disabled={pendingState.isPending}
        >
          Save Settings
        </ButtonWithLoading>
      </div>
    </div>
  );
}

export default YoutubeSettingsForm;
