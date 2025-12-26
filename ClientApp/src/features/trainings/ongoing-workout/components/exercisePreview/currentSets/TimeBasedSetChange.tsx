import { Timer } from "lucide-react";

import { TimeBasedExerciseSetSchema } from "@/features/trainings/exercises/data/exercisesSchemas";

function TimeBasedSetChange({
  currentSet,
}: {
  currentSet: TimeBasedExerciseSetSchema;
}) {
  return (
    <div className="inline-flex items-center gap-2">
      <Timer className="size-4" />
      <p>{currentSet.durationSeconds} seconds</p>
    </div>
  );
}

export default TimeBasedSetChange;
