import { Trash } from "lucide-react";
import { useFormContext, UseFormReturn } from "react-hook-form";

import { FormField, FormItem, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { ExerciseSetType } from "@/services/openapi/api";

import { ExerciseFormSchema } from "../../../data/exercisesSchemas";
import BodyweightExerciseSet from "./BodyweightExerciseSet";
import DistanceExerciseSet from "./DistanceExerciseSet";
import TimeBasedExerciseSet from "./TimeBasedExerciseSet";
import WeightBasedExerciseSet from "./WeightBasedExerciseSet";

interface ExerciseSetRowProps {
  index: number;
  form: UseFormReturn<ExerciseFormSchema>;
}

export const exerciseSetTypes = [
  { label: "Weight", value: ExerciseSetType.Weight },
  { label: "Time", value: ExerciseSetType.Time },
  { label: "Bodyweight", value: ExerciseSetType.Bodyweight },
  { label: "Distance", value: ExerciseSetType.Distance },
];

function ExerciseSetRow({ index }: ExerciseSetRowProps) {
  "use no memo";

  const { control, getValues, setValue } = useFormContext<ExerciseFormSchema>();

  function renderSetRow() {
    switch (getValues(`exerciseSets.${index}.type`)) {
      case ExerciseSetType.Weight:
        return <WeightBasedExerciseSet index={index} />;
      case ExerciseSetType.Time:
        return <TimeBasedExerciseSet index={index} />;
      case ExerciseSetType.Bodyweight:
        return <BodyweightExerciseSet index={index} />;
      case ExerciseSetType.Distance:
        return <DistanceExerciseSet index={index} />;
    }
  }

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

      {renderSetRow()}
    </div>
  );
}

export default ExerciseSetRow;
