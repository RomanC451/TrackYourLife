import { UseFormReturn, useWatch } from "react-hook-form";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Form,
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
  handleCustomSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  pendingState: MutationPendingState;
}

function YoutubeSettingsForm({
  form,
  handleCustomSubmit,
  pendingState,
}: YoutubeSettingsFormProps) {
  const settingsChangeFrequency = useWatch({
    control: form.control,
    name: "settingsChangeFrequency",
  });

  const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const result = await form.trigger();
    if (!result) {
      return;
    }
    form.clearErrors();
    handleCustomSubmit(event);
  };

  return (
    <Form {...form}>
      <form onSubmit={onSubmit} className="space-y-6">
        <FormField
          control={form.control}
          name="maxEntertainmentVideosPerDay"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Max Divertissment Videos Per Day</FormLabel>
              <FormDescription>
                Maximum number of divertissment videos you can watch per day.
                Set to 0 for unlimited.
              </FormDescription>
              <FormControl>
                <Input
                  type="number"
                  min="0"
                  {...field}
                  onChange={(e) => {
                    const value = Number.parseInt(e.target.value, 10);
                    field.onChange(Number.isNaN(value) ? 0 : value);
                  }}
                  value={field.value ?? 0}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

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
                  <SelectItem
                    value={SettingsChangeFrequency.SpecificDayOfMonth}
                  >
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
                  <Input
                    type="number"
                    min="1"
                    {...field}
                    onChange={(e) => {
                      const value = parseInt(e.target.value, 10);
                      field.onChange(isNaN(value) ? undefined : value);
                    }}
                    value={field.value ?? ""}
                  />
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
                  <Input
                    type="number"
                    min="1"
                    max="31"
                    {...field}
                    onChange={(e) => {
                      const value = parseInt(e.target.value, 10);
                      field.onChange(isNaN(value) ? undefined : value);
                    }}
                    value={field.value ?? ""}
                  />
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
      </form>
    </Form>
  );
}

export default YoutubeSettingsForm;
