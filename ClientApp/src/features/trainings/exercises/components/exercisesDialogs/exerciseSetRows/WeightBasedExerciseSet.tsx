import { useFormContext } from "react-hook-form";

import { FormField, FormItem, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";

import { ExerciseFormSchema } from "../../../data/exercisesSchemas";

function WeightBasedExerciseSet({ index }: { index: number }) {
  const { control } = useFormContext<ExerciseFormSchema>();

  return (
    <div className="grid grid-cols-2 gap-4">
      <FormField
        control={control}
        name={`exerciseSets.${index}.reps`}
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
              placeholder="Reps"
              className="bg-background/50 [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
            />
            <FormMessage />
          </FormItem>
        )}
      />
      <FormField
        control={control}
        name={`exerciseSets.${index}.weight`}
        render={({ field }) => (
          <FormItem>
            <Input
              id={`set-${index}-weight`}
              type="number"
              value={field.value}
              onChange={(e) => {
                const value = e.target.value;

                if (value === "") {
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
              placeholder="Weight (kg)"
              className="bg-background/50 [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
            />
            <FormMessage />
          </FormItem>
        )}
      />
    </div>
  );
}

export default WeightBasedExerciseSet;
