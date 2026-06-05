import WorkoutTimer from "@/features/trainings/common/components/workoutTimer/WorkoutTimer";
import { ExerciseSetChangesSchema } from "@/features/trainings/exercises/data/exercisesSchemas";
import { ExerciseDto } from "@/services/openapi";

import AdjustmentsHistory from "../adjustmentsHistory/AdjustmentsHistory";
import ExerciseSetChangeForm from "./ExerciseSetChangeForm";

function AdjustExercise({ exercise }: { exercise: ExerciseDto }) {
  const initialChanges: ExerciseSetChangesSchema = {
    newSets: exercise.exerciseSets,
  };

  return (
    <div className="space-y-6">
      <WorkoutTimer />
      <AdjustmentsHistory exerciseId={exercise.id} />
      <div className="space-y-4">
        <div className="space-y-1">
          <h2 className="text-lg font-semibold">{exercise.name}</h2>
          <p className="text-sm text-primary">
            Adjust sets for next workout session
          </p>
        </div>
        <ExerciseSetChangeForm
          defaultValues={initialChanges}
          exercise={exercise}
        />
      </div>
    </div>
  );
}

export default AdjustExercise;
