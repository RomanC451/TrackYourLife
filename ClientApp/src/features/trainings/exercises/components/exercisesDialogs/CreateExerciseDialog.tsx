import { ExerciseSetType } from "@/services/openapi";

import useCreateExerciseMutation from "../../mutations/useCreateExerciseMutation";
import { createDefaultExerciseSet } from "../../utils/exerciseSetsMappings";
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
        name: "",
        muscleGroups: [],
        difficulty: "Easy",
        description: "",
        equipment: "",
        videoUrl: "",
        pictureUrl: "",
        exerciseSets: [createDefaultExerciseSet(ExerciseSetType.Weight, 0)],
      }}
      pendingState={createExerciseMutation.pendingState}
      onSuccess={onSuccess}
      onClose={onClose}
    />
  );
};

export default CreateExerciseDialog;
