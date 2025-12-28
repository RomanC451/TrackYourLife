import { Trash } from "lucide-react";
import { useFormContext, UseFormReturn } from "react-hook-form";

import { FormField, FormItem, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";

import {
  ExerciseFormSchema,
  exerciseSetType,
} from "../../../data/exercisesSchemas";

interface ExerciseSetRowProps {
  index: number;
  form: UseFormReturn<ExerciseFormSchema>;
}

export const exerciseSetTypes = [
  { label: "Weight", value: exerciseSetType.Weight },
  { label: "Time", value: exerciseSetType.Time },
  { label: "Bodyweight", value: exerciseSetType.Bodyweight },
  { label: "Distance", value: exerciseSetType.Distance },
];

function ExerciseSetRow({ index }: ExerciseSetRowProps) {
  "use no memo";

  const { control, getValues, setValue } = useFormContext<ExerciseFormSchema>();

  const currentSet = getValues("exerciseSets")[index];

  return (
    <div className="space-y-2 rounded-lg border border-border bg-card p-4">
      <div className="flex justify-end gap-2"></div>
      <div className="flex items-center gap-4">
        <div className="flex w-full justify-between gap-4">
          <FormField
            control={control}
            name={`exerciseSets.${index}.name`}
            render={({ field }) => (
              <FormItem className="w-full">
                <Input
                  {...field}
                  id={`set-${index}-name`}
                  placeholder="Type"
                  className="w-full bg-background/50"
                />
                <FormMessage />
              </FormItem>
            )}
          />
          {index > 0 && (
            <button
              type="button"
              onClick={() => {
                const sets = getValues("exerciseSets");
                sets.splice(index, 1);
                setValue("exerciseSets", sets);
              }}
              className="text-destructive hover:text-destructive/90"
            >
              <Trash className="h-4 w-4" />
            </button>
          )}
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={control}
          name={`exerciseSets.${index}.count1`}
          render={({ field }) => (
            <FormItem>
              <Input
                id={`set-${index}-reps`}
                type="number"
                value={field.value || ""}
                onChange={(e) => {
                  const value = e.target.value;

                  if (value === "") {
                    field.onChange(0);
                  } else {
                    // Remove all leading zeros except for a single zero if the value is just zeros
                    const trimmedValue = value.replace(/^0+/, "") || "0";
                    field.onChange(Number(trimmedValue));
                  }
                }}
                onBlur={field.onBlur}
                name={field.name}
                ref={field.ref}
                // onBlur={() => form.trigger(`exerciseSets.${index}.reps`)}
                placeholder={currentSet.unit1}
                className="bg-background/50 [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
              />
              <FormMessage />
            </FormItem>
          )}
        />
        {currentSet.count2 !== undefined && currentSet.unit2 !== undefined && (
          <FormField
            control={control}
            name={`exerciseSets.${index}.count2`}
            render={({ field }) => (
              <FormItem>
                <Input
                  id={`set-${index}-weight`}
                  type="number"
                  value={field.value || ""}
                  onChange={(e) => {
                    const value = e.target.value;

                    if (field.value || "") {
                      field.onChange("");
                    } else {
                      // Remove all leading zeros except for a single zero if the value is just zeros
                      const trimmedValue = value.replace(/^0+/, "") || "0";
                      field.onChange(Number(trimmedValue));
                    }
                  }}
                  onBlur={field.onBlur}
                  name={field.name}
                  ref={field.ref}
                  // onBlur={() => form.trigger(`exerciseSets.${index}.weight`)}
                  placeholder={currentSet.unit2}
                  className="bg-background/50 [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
                />
                <FormMessage />
              </FormItem>
            )}
          />
        )}
      </div>
    </div>
  );
}

export default ExerciseSetRow;
