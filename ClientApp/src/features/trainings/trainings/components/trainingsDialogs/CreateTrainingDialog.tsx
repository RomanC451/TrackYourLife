import useCreateTrainingMutation from "../../mutations/useCreateTrainingMutation";
import TrainingDialog from "./TrainingDialog";

function CreateTrainingDialog({
  onClose,
  onSuccess,
}: {
  onClose?: () => void;
  onSuccess?: () => void;
}) {
  const createTrainingMutation = useCreateTrainingMutation();

  return (
    <TrainingDialog
      dialogType="create"
      mutation={createTrainingMutation}
      defaultValues={{
        name: "",
        muscleGroups: [],
        difficulty: "Easy",
        description: "",
        duration: 0,
        restSeconds: 30,
        exercises: [],
      }}
      onClose={onClose}
      onSuccess={onSuccess}
    />
  );
}

export default CreateTrainingDialog;
