import { FieldArrayWithId } from "react-hook-form";

import {
  BodyweightExerciseSetSchema,
  DistanceExerciseSetSchema,
  TimeBasedExerciseSetSchema,
  WeightBasedExerciseSetSchema,
} from "@/features/trainings/exercises/data/exercisesSchemas";
import { ExerciseSet } from "@/services/openapi";

import SetChangeField from "../SetChangeField";

function WeightBasedSetChangeForm({
  original,
  field,
  idx,
}: {
  original: ExerciseSet;
  field: FieldArrayWithId<
    {
      newSets: (
        | WeightBasedExerciseSetSchema
        | TimeBasedExerciseSetSchema
        | BodyweightExerciseSetSchema
        | DistanceExerciseSetSchema
      )[];
    },
    "newSets",
    "id"
  >;
  idx: number;
}) {
  return (
    <div className="flex gap-20">
      <SetChangeField
        originalValue={original.reps ?? 0}
        field={field}
        idx={idx}
        changedPropName="reps"
        label="Reps"
        unit="reps"
      />
      <SetChangeField
        originalValue={original.weight ?? 0}
        field={field}
        idx={idx}
        changedPropName="weight"
        label="Weight"
        unit="kg"
      />
    </div>
  );
}

export default WeightBasedSetChangeForm;
