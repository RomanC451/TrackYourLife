import { createContext, useContext, useState } from "react";

import Assert from "@/lib/assert";
import { ExerciseDto } from "@/services/openapi";

import ForceDeleteExerciseAlertDialog from "../../exercises/components/common/ForceDeleteExerciseAlertDialog";
import CreateExerciseDialog from "../../exercises/components/exercisesDialogs/CreateExerciseDialog";
import EditExerciseDialog from "../../exercises/components/exercisesDialogs/EditExerciseDialog";
import ShowExerciseInfoDialog from "../../exercises/components/exercisesDialogs/ShowExerciseInfoDialog";

interface TrainingsDialogsContextType {
  setExerciseToEdit: (
    exercise: {
      exercise: ExerciseDto;
      onSuccessEdit: (exercise: Partial<ExerciseDto>) => void;
      onCancelEdit: () => void;
    } | null,
  ) => void;
  setExerciseToForceDelete: (
    exercise: {
      exercise: ExerciseDto;
      onSuccessDelete: () => void;
      onCancelDelete: () => void;
    } | null,
  ) => void;
  setExerciseToView: (open: ExerciseDto | null) => void;
  setCreateExerciseDialogOpen: (open: boolean) => void;
}
const TrainingsDialogsContext = createContext<TrainingsDialogsContextType>({
  setExerciseToEdit: () => {},
  setExerciseToForceDelete: () => {},
  setExerciseToView: () => {},
  setCreateExerciseDialogOpen: () => {},
});

export const TrainingsDialogsContextProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const [exerciseToEdit, setExerciseToEdit] = useState<{
    exercise: ExerciseDto;
    onSuccessEdit: (exercise: Partial<ExerciseDto>) => void;
    onCancelEdit: () => void;
  } | null>(null);

  const [exerciseToForceDelete, setExerciseToForceDelete] = useState<{
    exercise: ExerciseDto;
    onSuccessDelete: () => void;
    onCancelDelete: () => void;
  } | null>(null);

  const [exerciseToView, setExerciseToView] = useState<ExerciseDto | null>(
    null,
  );

  const [createExerciseDialogOpen, setCreateExerciseDialogOpen] =
    useState(false);

  return (
    <TrainingsDialogsContext.Provider
      value={{
        setExerciseToEdit,
        setExerciseToForceDelete,
        setExerciseToView,
        setCreateExerciseDialogOpen,
      }}
    >
      {exerciseToForceDelete && (
        <ForceDeleteExerciseAlertDialog
          id={exerciseToForceDelete.exercise.id}
          name={exerciseToForceDelete.exercise.name}
          onSuccess={() => {
            setExerciseToForceDelete(null);
            exerciseToForceDelete.onSuccessDelete();
          }}
          onCancel={() => {
            setExerciseToForceDelete(null);
            exerciseToForceDelete.onCancelDelete();
          }}
        />
      )}

      {exerciseToEdit && (
        <EditExerciseDialog
          exercise={exerciseToEdit.exercise}
          onSuccess={(exercise) => {
            exerciseToEdit.onSuccessEdit(exercise);
            setExerciseToEdit(null);
          }}
          onClose={() => {
            setExerciseToEdit(null);
          }}
          defaultOpen={true}
        />
      )}
      {exerciseToView && (
        <ShowExerciseInfoDialog
          exercise={exerciseToView}
          onClose={() => {
            setExerciseToView(null);
          }}
        />
      )}
      {createExerciseDialogOpen && (
        <CreateExerciseDialog
          onSuccess={(exercise) => {
            setCreateExerciseDialogOpen(false);
            exerciseToEdit?.onSuccessEdit(exercise);
          }}
          onClose={() => {
            setCreateExerciseDialogOpen(false);
          }}
          defaultOpen={true}
        />
      )}
      {children}
    </TrainingsDialogsContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useTrainingsDialogsContext = () => {
  const context = useContext(TrainingsDialogsContext);

  Assert.isNotUndefined(
    context,
    "TrainingsDialogsContext must be used within a TrainingsDialogsContextProvider",
  );
  Assert.isNotEmptyObject(
    context,
    "TrainingsDialogsContext must be used within a TrainingsDialogsContextProvider",
  );

  return context;
};
