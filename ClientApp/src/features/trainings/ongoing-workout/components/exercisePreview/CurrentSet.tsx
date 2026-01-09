import { Repeat, Weight } from "lucide-react";

import { capitalizeFirstLetter } from "@/lib/stringUtils";
import { ExerciseSet } from "@/services/openapi";

function CurrentSet({
  equipment,
  currentSet,
}: {
  equipment?: string;
  currentSet: ExerciseSet;
}) {
  return (
    <div className="w-full rounded-lg border border-border bg-card p-6">
      {/* Equipment title */}
      {equipment && (
        <h2 className="mb-4 text-xl font-semibold text-foreground">
          Equipment: {equipment}
        </h2>
      )}

      {/* Current set */}
      <div className="mb-4">
        <span className="text-sm text-muted-foreground">Current Set: </span>
        <span className="text-sm font-medium text-foreground">
          {capitalizeFirstLetter(currentSet.name)}
        </span>
      </div>

      {/* Weight and reps inline */}
      <div className="flex items-center gap-6">
        {/* First value (usually weight or reps) */}
        <div className="flex items-center gap-2 text-foreground">
          <Weight className="h-5 w-5 text-muted-foreground" strokeWidth={2} />
          <span className="font-medium">
            {currentSet.count1} {currentSet.unit1}
          </span>
        </div>

        {/* Second value (if exists) */}
        {currentSet.count2 && currentSet.unit2 && (
          <div className="flex items-center gap-2 text-foreground">
            <Repeat className="h-5 w-5 text-muted-foreground" strokeWidth={2} />
            <span className="font-medium">
              {currentSet.count2} {currentSet.unit2}
            </span>
          </div>
        )}
      </div>
    </div>
  );
}

export default CurrentSet;
