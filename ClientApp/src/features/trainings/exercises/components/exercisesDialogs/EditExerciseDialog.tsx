import { ExerciseDto } from "@/services/openapi";

import useUpdateExerciseMutation from "../../mutations/useUpdateExerciseMutation";
import ExerciseDialog from "./ExerciseDialog";

function EditExerciseDialog({
  exercise,
  onSuccess,
  onClose,
  defaultOpen,
}: {
  exercise: ExerciseDto;
  onSuccess: (exercise: Partial<ExerciseDto>) => void;
  defaultOpen?: boolean;
  onClose?: () => void;
}) {
  const { updateExerciseMutation, isPending } = useUpdateExerciseMutation();

  return (
    <ExerciseDialog
      submitButtonText={"Save"}
      dialogButtonText={"Edit"}
      dialogTitle={"Edit Exercise"}
      dialogDescription={"Edit the exercise"}
      mutation={updateExerciseMutation}
      defaultValues={exercise}
      isPending={isPending}
      onSuccess={onSuccess}
      defaultOpen={defaultOpen}
      onClose={onClose}
    />
  );
}

export default EditExerciseDialog;
