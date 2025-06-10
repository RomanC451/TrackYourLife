import { Badge } from "@/components/ui/badge";
import { CardDescription } from "@/components/ui/card";
import { ExerciseDto } from "@/services/openapi";

import ExerciseSetChangeForm from "./ExerciseSetChangeForm";

function AdjustExercise({
  exercise,
  onSuccess,
  ongoingTrainingId,
}: {
  exercise: ExerciseDto;
  onSuccess: () => void;
  ongoingTrainingId: string;
}) {
  // Prepare initial changes: one for each set, defaulting to 0 changes
  const initialChanges = exercise.exerciseSets.map((set) => ({
    setId: set.id,
    newWeight: set.weight,
    newReps: set.reps,
  }));

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <CardDescription>Adjust sets for next week</CardDescription>
        <Badge className="text-sm font-bold">{exercise.name}</Badge>
      </div>
      <ExerciseSetChangeForm
        defaultValues={initialChanges}
        exerciseSets={exercise.exerciseSets}
        exerciseId={exercise.id}
        onSuccess={onSuccess}
        ongoingTrainingId={ongoingTrainingId}
      />
    </div>
  );
}

export default AdjustExercise;
