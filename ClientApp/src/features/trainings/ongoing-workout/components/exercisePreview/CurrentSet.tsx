import { Target, Weight } from "lucide-react";

import { capitalizeFirstLetter } from "@/lib/stringUtils";
import { ExerciseSet } from "@/services/openapi";

function CurrentSet({
  currentSet,
  index,
}: {
  currentSet: ExerciseSet;
  index: number;
}) {
  return (
    <div>
      <div className="space-y-2">
        <div className="flex items-center gap-2">
          <div className="font-semibold">Current Set:</div>
          <p className="text-md inline-block w-auto rounded-xl border bg-secondary px-2 font-semibold">
            {index + 1}. {capitalizeFirstLetter(currentSet.name)}
          </p>
        </div>

        <div className="inline-flex w-full gap-4 rounded-md bg-secondary/60 p-2">
          <div className="inline-flex items-center gap-2">
            <Target className="size-4" />
            <p>{currentSet.reps} reps</p>
          </div>
          <div className="inline-flex items-center gap-2">
            <Weight className="size-4" />
            <p>{currentSet.weight} kg</p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default CurrentSet;
