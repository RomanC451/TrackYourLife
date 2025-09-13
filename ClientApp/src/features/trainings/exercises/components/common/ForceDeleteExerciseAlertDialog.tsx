import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";

import useDeleteExerciseMutation from "../../mutations/useDeleteExerciseMutation";

function ForceDeleteExerciseAlertDialog({
  id,
  name,
  onSuccess,
  onCancel,
}: {
  id: string;
  name: string;
  onSuccess: () => void;
  onCancel: () => void;
}) {
  const deleteExerciseMutation = useDeleteExerciseMutation();

  return (
    <AlertDialog
      defaultOpen={true}
      onOpenChange={(state) => {
        if (!state) {
          onCancel();
        }
      }}
    >
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Delete Exercise</AlertDialogTitle>
          <AlertDialogDescription>
            This exercise is being used in your workouts. If you delete it, it
            will be removed from all your workouts. Are you sure you want to
            continue?
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel
            type="button"
            onClick={(e) => {
              e.stopPropagation();
              onCancel();
            }}
          >
            Cancel
          </AlertDialogCancel>
          <AlertDialogAction
            className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            onClick={() => {
              deleteExerciseMutation.mutate(
                {
                  id: id,
                  forceDelete: true,
                  name: name,
                },
                {
                  onSuccess: () => {
                    onSuccess();
                  },
                },
              );
            }}
          >
            Delete Exercise
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

export default ForceDeleteExerciseAlertDialog;
