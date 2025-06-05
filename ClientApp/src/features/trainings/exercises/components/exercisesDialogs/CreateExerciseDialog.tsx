import { ExerciseDto } from "@/services/openapi";

import useCreateExerciseMutation from "../../mutations/useCreateExerciseMutation";
import ExerciseDialog from "./ExerciseDialog";

type CreateExerciseDialogProps =
  | {
      buttonComponent?: React.ReactNode;
      onSuccess: (exercise: Partial<ExerciseDto>) => void;
      onClose: () => void;
      defaultOpen?: undefined;
    }
  | {
      buttonComponent?: undefined;
      onSuccess: (exercise: Partial<ExerciseDto>) => void;
      onClose: () => void;
      defaultOpen: boolean;
    };

const CreateExerciseDialog = ({
  buttonComponent,
  onSuccess,
  onClose,
  defaultOpen,
}: CreateExerciseDialogProps) => {
  const { createExerciseMutation, isPending } = useCreateExerciseMutation();

  return (
    <ExerciseDialog
      buttonComponent={buttonComponent}
      buttonVariant="default"
      dialogButtonText="Create New Exercise"
      dialogTitle="Create New Exercise"
      dialogDescription="Create a new exercise"
      submitButtonText="Create"
      mutation={createExerciseMutation}
      defaultValues={{
        name: "",
        description: "",
        equipment: "",
        videoUrl: "",
        pictureUrl: "",
        exerciseSets: [],
      }}
      isPending={isPending}
      onSuccess={onSuccess}
      onClose={onClose}
      defaultOpen={defaultOpen}
    />
  );
};

export default CreateExerciseDialog;
