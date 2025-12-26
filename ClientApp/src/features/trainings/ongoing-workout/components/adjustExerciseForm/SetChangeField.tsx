import { CircleMinus, CirclePlus } from "lucide-react";
import { FieldArrayWithId, useFormContext } from "react-hook-form";

import { Button } from "@/components/ui/button";
import { FormField, FormItem, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  ExerciseSetChangesSchema,
  ExerciseSetSchema,
} from "@/features/trainings/exercises/data/exercisesSchemas";

function SetChangeField({
  originalValue,
  idx,
  changedPropName,
  label,
  unit,
}: {
  originalValue: number;
  field: FieldArrayWithId<
    {
      newSets: Array<ExerciseSetSchema>;
    },
    "newSets",
    "id"
  >;
  idx: number;
  changedPropName: "weight" | "reps" | "durationSeconds" | "distance";
  label: string;
  unit: string;
}) {
  const { control, watch, setValue, getValues } =
    useFormContext<ExerciseSetChangesSchema>();

  let delta = 0;

  if (changedPropName === "durationSeconds") {
    //!TODO: implement duration change
  } else {
    delta = watch(`newSets.${idx}.${changedPropName}`) - originalValue;
  }

  function handleChange(operation: "increment" | "decrement") {
    const value = getValues(`newSets.${idx}.${changedPropName}`);
    if (changedPropName === "durationSeconds") {
      //!TODO: implement duration change
      setValue(
        `newSets.${idx}.${changedPropName}`,
        value + (operation === "increment" ? 5 : -5),
      );
    } else {
      setValue(
        `newSets.${idx}.${changedPropName}`,
        value + (operation === "increment" ? 1 : -1),
      );
    }
  }

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
            handleChange("decrement");
          }}
        >
          <CircleMinus className="h-4 w-4" />
        </Button>
        <FormField
          control={control}
          name={`newSets.${idx}.${changedPropName}`}
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
            handleChange("increment");
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
