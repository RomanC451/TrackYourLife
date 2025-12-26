import { Card } from "@/components/ui/card";
import {
  BodyweightExerciseSetSchema,
  DistanceExerciseSetSchema,
  TimeBasedExerciseSetSchema,
  WeightBasedExerciseSetSchema,
} from "@/features/trainings/exercises/data/exercisesSchemas";
import { capitalizeFirstLetter } from "@/lib/stringUtils";
import { ExerciseSetType } from "@/services/openapi";

import WeightBasedCurrentSet from "./currentSets/WeightBasedCurrentSet";

function CurrentSet({
  currentSet,
  index,
}: {
  currentSet:
    | WeightBasedExerciseSetSchema
    | TimeBasedExerciseSetSchema
    | BodyweightExerciseSetSchema
    | DistanceExerciseSetSchema;
  index: number;
}) {
  return (
    <Card className="w-full rounded-xl p-4 shadow-lg">
      <div className="space-y-2">
        <div className="flex items-center gap-2">
          <div className="font-semibold">Current Set:</div>
          <p className="text-md inline-block w-auto rounded-xl border bg-secondary px-2 font-semibold">
            {index + 1}. {capitalizeFirstLetter(currentSet.name)}
          </p>
        </div>

        <div className="inline-flex w-full gap-4 rounded-md bg-secondary/60 p-2">
          {currentSet.type === ExerciseSetType.Weight && (
            <WeightBasedCurrentSet currentSet={currentSet} />
          )}
        </div>
      </div>
    </Card>
  );
}

export default CurrentSet;
