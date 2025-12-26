import { FieldArrayWithId } from "react-hook-form";

import {
  BodyweightExerciseSetSchema,
  DistanceExerciseSetSchema,
  TimeBasedExerciseSetSchema,
  WeightBasedExerciseSetSchema,
} from "@/features/trainings/exercises/data/exercisesSchemas";
import { ExerciseSet } from "@/services/openapi";

import SetChangeField from "../SetChangeField";

function DistanceBasedSetChangeForm({
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
        originalValue={original.distance ?? 0}
        field={field}
        idx={idx}
        changedPropName="distance"
        label="Distance"
        unit="m"
      />
    </div>
  );
}

export default DistanceBasedSetChangeForm;
