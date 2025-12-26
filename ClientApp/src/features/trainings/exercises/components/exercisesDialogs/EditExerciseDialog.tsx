import { ExerciseDto } from "@/services/openapi";

import { ExerciseSetSchema } from "../../data/exercisesSchemas";
import useUpdateExerciseMutation from "../../mutations/useUpdateExerciseMutation";
import ExerciseDialog from "./ExerciseDialog";

function EditExerciseDialog({
  exercise,
  onSuccess,
  onClose,
}: {
  exercise: ExerciseDto;
  onSuccess?: () => void;
  onClose?: () => void;
}) {
  const updateExerciseMutation = useUpdateExerciseMutation();

  return (
    <ExerciseDialog
      dialogType="edit"
      mutation={updateExerciseMutation}
      defaultValues={{
        ...exercise,
        exerciseSets: exercise.exerciseSets as ExerciseSetSchema[],
      }}
      pendingState={updateExerciseMutation.pendingState}
      onSuccess={onSuccess}
      onClose={onClose}
    />
  );
}

export default EditExerciseDialog;
