import { TrainingDto } from "@/services/openapi";

import useUpdateTrainingMutation from "../../mutations/useUpdateTrainingMutation";
import TrainingDialog from "./TrainingDialog";

function EditTrainingDialog({
  training,
  onClose,
  onSuccess,
}: {
  training: TrainingDto;
  onClose?: () => void;
  onSuccess?: () => void;
}) {
  const updateTrainingMutation = useUpdateTrainingMutation();

  return (
    <TrainingDialog
      dialogType="edit"
      mutation={updateTrainingMutation}
      defaultValues={{
        id: training.id,
        name: training.name,
        muscleGroups: training.muscleGroups,
        difficulty: training.difficulty,
        description: training.description,
        duration: training.duration,
        restSeconds: training.restSeconds,
        exercises: training.exercises.map((exercise) => ({
          ...exercise,
          exerciseSets: exercise.exerciseSets,
          equipment: exercise.equipment,
        })),
      }}
      onClose={onClose}
      onSuccess={onSuccess}
    />
  );
}

export default EditTrainingDialog;
