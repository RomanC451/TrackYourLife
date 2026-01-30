import { CircleMinus, CirclePlus } from "lucide-react";
import { ControllerRenderProps } from "react-hook-form";

import { Button } from "@/components/ui/button";
import { FormItem, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { ExerciseSetFormSchema } from "@/features/trainings/exercises/data/exercisesSchemas";

function SetChangeField({
  originalValue,
  field,
  label,
  unit,
  step,
}: {
  originalValue: number;
  field: ControllerRenderProps<
    {
      newSets: Array<ExerciseSetFormSchema>;
    },
    `newSets.${number}.count1` | `newSets.${number}.count2`
  >;
  label: string;
  unit: string;
  step: number;
}) {
  function handleChange(operation: "increment" | "decrement") {
    if (operation === "increment") {
      field.onChange((field.value ?? 0) + step);
    } else {
      field.onChange((field.value ?? 0) - step);
    }
  }

  const delta = (field.value ?? 0) - originalValue;

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

        <FormItem>
          <Input
            {...field}
            type="number"
            placeholder={label}
            className="max-w-20"
          />
          <FormMessage />
        </FormItem>
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
          {delta > 0 ? `+${delta.toFixed(2)}` : delta.toFixed(2)} {unit}
        </p>
      )}
    </div>
  );
}

export default SetChangeField;
