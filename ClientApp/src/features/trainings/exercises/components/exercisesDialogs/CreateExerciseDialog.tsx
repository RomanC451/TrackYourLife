import useCreateExerciseMutation from "../../mutations/useCreateExerciseMutation";
import ExerciseDialog from "./ExerciseDialog";

type CreateExerciseDialogProps = {
  onSuccess?: () => void;
  onClose?: () => void;
};

const CreateExerciseDialog = ({
  onSuccess,
  onClose,
}: CreateExerciseDialogProps) => {
  const createExerciseMutation = useCreateExerciseMutation();

  return (
    <ExerciseDialog
      dialogType="create"
      mutation={createExerciseMutation}
      defaultValues={{
        id: "",
        name: "",
        muscleGroups: [],
        difficulty: "Easy",
        description: "",
        equipment: "",
        videoUrl: "",
        pictureUrl: "",
        exerciseSets: [],
      }}
      pendingState={createExerciseMutation.pendingState}
      onSuccess={onSuccess}
      onClose={onClose}
    />
  );
};

export default CreateExerciseDialog;
