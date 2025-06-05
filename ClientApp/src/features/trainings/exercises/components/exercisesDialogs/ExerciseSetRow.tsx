import { Trash } from "lucide-react";
import { UseFormReturn } from "react-hook-form";

import { FormField, FormItem, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";

import { ExerciseFormSchema } from "../../data/exercisesSchemas";

interface ExerciseSetRowProps {
  index: number;
  form: UseFormReturn<ExerciseFormSchema>;
}

function ExerciseSetRow({ index, form }: ExerciseSetRowProps) {
  return (
    <div className="space-y-2 rounded-lg border border-border bg-card p-4">
      <div className="flex items-center gap-4">
        <div className="flex-1">
          <FormField
            control={form.control}
            name={`exerciseSets.${index}.name`}
            render={({ field }) => (
              <FormItem>
                <Input
                  {...field}
                  id={`set-${index}-name`}
                  placeholder="Type"
                  className="bg-background/50"
                />
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
        <button
          type="button"
          onClick={() => {
            const sets = form.getValues("exerciseSets");
            sets.splice(index, 1);
            form.setValue("exerciseSets", sets);
            form.trigger("exerciseSets");
          }}
          className="text-destructive hover:text-destructive/90"
        >
          <Trash className="h-4 w-4" />
        </button>
      </div>
      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={form.control}
          name={`exerciseSets.${index}.reps`}
          render={({ field }) => (
            <FormItem>
              <Input
                {...field}
                value={field.value === 0 ? "" : field.value}
                id={`set-${index}-reps`}
                type="number"
                onChange={(e) => {
                  const value = e.target.value;
                  field.onChange(value === "" ? 0 : Number(value));
                }}
                // onBlur={() => form.trigger(`exerciseSets.${index}.reps`)}
                placeholder="Reps"
                className="bg-background/50 [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
              />
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name={`exerciseSets.${index}.weight`}
          render={({ field }) => (
            <FormItem>
              <Input
                {...field}
                value={field.value === 0 ? "" : field.value}
                id={`set-${index}-weight`}
                type="number"
                onChange={(e) => {
                  const value = e.target.value;
                  field.onChange(value === "" ? 0 : Number(value));
                }}
                // onBlur={() => form.trigger(`exerciseSets.${index}.weight`)}
                placeholder="Weight (kg)"
                className="bg-background/50 [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
              />
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
    </div>
  );
}

export default ExerciseSetRow;
