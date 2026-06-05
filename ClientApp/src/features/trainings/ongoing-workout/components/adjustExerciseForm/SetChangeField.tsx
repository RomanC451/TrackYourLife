import { Minus, Plus } from "lucide-react";
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
    const currentValue = field.value ?? 0;
    if (operation === "increment") {
      field.onChange(currentValue + step);
    } else {
      field.onChange(Math.max(0, currentValue - step));
    }
  }

  return (
    <div className="min-w-0 space-y-2">
      <label className="text-xs font-medium text-muted-foreground">
        {label}{" "}
        <span className="text-muted-foreground/60">
          (current: {originalValue} {unit})
        </span>
      </label>
      <div className="flex items-center gap-2">
        <Button
          variant="outline"
          size="icon"
          type="button"
          onClick={() => {
            handleChange("decrement");
          }}
          className="h-10 w-10 shrink-0 border-border/50 bg-secondary/50 hover:bg-secondary"
        >
          <Minus className="h-4 w-4" />
        </Button>

        <FormItem className="flex-1">
          <Input
            {...field}
            type="number"
            placeholder={label}
            className="h-10 border-border/50 bg-input/50 text-center font-mono text-sm font-medium"
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
          className="h-10 w-10 shrink-0 border-border/50 bg-secondary/50 hover:bg-secondary"
        >
          <Plus className="h-4 w-4" />
        </Button>
      </div>
    </div>
  );
}

export default SetChangeField;
