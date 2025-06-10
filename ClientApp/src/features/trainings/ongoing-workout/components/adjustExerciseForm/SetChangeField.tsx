import { CircleMinus, CirclePlus } from "lucide-react";
import { FieldArrayWithId, useFormContext } from "react-hook-form";

import { Button } from "@/components/ui/button";
import { FormField, FormItem, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { ExerciseSetChangeFormSchema } from "@/features/trainings/exercises/data/exercisesSchemas";
import { ExerciseSet } from "@/services/openapi";

function SetChangeField({
  original,
  idx,
  type,
}: {
  original: ExerciseSet;
  field: FieldArrayWithId<
    {
      changes: {
        setId: string;
        newWeight: number;
        newReps: number;
      }[];
    },
    "changes",
    "id"
  >;
  idx: number;
  type: "weight" | "reps";
}) {
  const { control, watch, setValue, getValues } =
    useFormContext<ExerciseSetChangeFormSchema>();

  const name = type === "reps" ? "newReps" : "newWeight";

  const label = type === "reps" ? "Reps" : "Weight";
  const unit = type === "reps" ? "reps" : "kg";
  const originalValue = type === "reps" ? original.reps : original.weight;

  const delta = watch(`changes.${idx}.${name}`) - originalValue;

  return (
    <div className="space-y-2">
      <p className="text-sm">
        {label} (current: {originalValue} {unit})
      </p>
      <div className="flex items-center gap-2">
        <Button
          variant="outline"
          size="icon"
          type="button"
          onClick={() => {
            const value = getValues(`changes.${idx}.${name}`);
            setValue(`changes.${idx}.${name}`, Math.max(0, value - 1));
          }}
        >
          <CircleMinus className="h-4 w-4" />
        </Button>
        <FormField
          control={control}
          name={`changes.${idx}.${name}`}
          render={({ field }) => (
            <FormItem>
              <Input
                {...field}
                type="number"
                value={field.value === 0 ? "" : field.value}
                onChange={(e) => {
                  const value = e.target.value;
                  field.onChange(value === "" ? 0 : Number(value));
                }}
                placeholder={label}
                className="max-w-20"
              />
              <FormMessage />
            </FormItem>
          )}
        />
        <Button
          variant="outline"
          size="icon"
          type="button"
          onClick={() => {
            const value = getValues(`changes.${idx}.${name}`);
            setValue(`changes.${idx}.${name}`, value + 1);
          }}
        >
          <CirclePlus className="h-4 w-4" />
        </Button>
      </div>
      {delta !== 0 && (
        <p className="text-sm">
          {delta > 0 ? `+${delta}` : delta} {unit}
        </p>
      )}
    </div>
  );
}

export default SetChangeField;
