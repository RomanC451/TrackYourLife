import { TrainingDto } from "@/services/openapi";

import useUpdateTrainingMutation from "../../mutations/useUpdateTrainingMutation";
import TrainingDialog from "../common/TrainingDialog";

function EditTrainingDialog({ training }: { training: TrainingDto }) {
  const { updateTrainingMutation, isPending } = useUpdateTrainingMutation();

  return (
    <TrainingDialog
      dialogButtonText="Edit"
      submitButtonText="Save"
      buttonVariant="outline"
      dialogTitle="Edit Workout"
      dialogDescription="Edit the details of this workout"
      mutation={updateTrainingMutation}
      defaultValues={{
        id: training.id,
        name: training.name,
        description: training.description,
        duration: training.duration,
        restSeconds: training.restSeconds,
        exercises: training.exercises.map((exercise) => ({
          ...exercise,
          exerciseSets: exercise.exerciseSets.map((set) => ({
            ...set,
          })),
          equipment: exercise.equipment,
        })),
      }}
      isPending={isPending}
    />
  );
}

export default EditTrainingDialog;
