import { Target, Weight } from "lucide-react";

import { WeightBasedExerciseSetSchema } from "@/features/trainings/exercises/data/exercisesSchemas";

function WeightBasedCurrentSet({
  currentSet,
}: {
  currentSet: WeightBasedExerciseSetSchema;
}) {
  return (
    <>
      <div className="inline-flex items-center gap-2">
        <Target className="size-4" />
        <p>{currentSet.reps} reps</p>
      </div>
      <div className="inline-flex items-center gap-2">
        <Weight className="size-4" />
        <p>{currentSet.weight} kg</p>
      </div>
    </>
  );
}

export default WeightBasedCurrentSet;
