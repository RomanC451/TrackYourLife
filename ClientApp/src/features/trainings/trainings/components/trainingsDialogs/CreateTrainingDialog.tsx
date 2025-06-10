import useCreateTrainingMutation from "../../mutations/useCreateTrainingMutation";
import TrainingDialog from "../common/TrainingDialog";

function CreateTrainingDialog() {
  const { createTrainingMutation, isPending } = useCreateTrainingMutation();

  return (
    <TrainingDialog
      buttonVariant="default"
      dialogButtonText="Create New Workout"
      dialogTitle="Create New Workout"
      dialogDescription="Create a new workout"
      submitButtonText="Create"
      mutation={createTrainingMutation}
      defaultValues={{
        name: "",
        description: "",
        duration: 0,
        restSeconds: 30,
        exercises: [],
      }}
      isPending={isPending}
    />
  );
}

export default CreateTrainingDialog;
