import { FieldArrayWithId } from "react-hook-form";

import {
  BodyweightExerciseSetSchema,
  DistanceExerciseSetSchema,
  TimeBasedExerciseSetSchema,
  WeightBasedExerciseSetSchema,
} from "@/features/trainings/exercises/data/exercisesSchemas";
import { ExerciseSet } from "@/services/openapi";

import SetChangeField from "../SetChangeField";

function TimeBasedSetChangeForm({
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
        //!TODO: implement duration change
        originalValue={original.durationSeconds ?? 0}
        field={field}
        idx={idx}
        changedPropName="durationSeconds"
        label="Duration"
        unit="seconds"
      />
    </div>
  );
}

export default TimeBasedSetChangeForm;
