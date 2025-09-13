import { Badge } from "@/components/ui/badge";
import { CardDescription } from "@/components/ui/card";
import WorkoutTimer from "@/features/trainings/common/components/workoutTimer/WorkoutTimer";
import { ExerciseDto } from "@/services/openapi";

import AdjustmentsHistory from "../adjustmentsHistory/AdjustmentsHistory";
import ExerciseSetChangeForm from "./ExerciseSetChangeForm";

function AdjustExercise({ exercise }: { exercise: ExerciseDto }) {
  // Prepare initial changes: one for each set, defaulting to 0 changes

  const initialChanges = exercise.exerciseSets.map((set) => ({
    setId: set.id,
    newWeight: set.weight,
    newReps: set.reps,
  }));

  return (
    <div className="flex flex-col gap-4">
      <WorkoutTimer />
      <AdjustmentsHistory exerciseId={exercise.id} />
      <div className="flex items-center justify-between">
        <CardDescription className="">
          Adjust sets for next workout session{" "}
        </CardDescription>
        <Badge className="text-sm font-bold">{exercise.name}</Badge>
      </div>
      <ExerciseSetChangeForm
        defaultValues={initialChanges}
        exercise={exercise}
      />
    </div>
  );
}

export default AdjustExercise;
