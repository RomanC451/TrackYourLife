import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";

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
import ButtonWithLoading from "@/components/ui/button-with-loading";

import useDeleteOngoingTrainingMutation from "../mutations/useDeleteOngoingTrainingMutation";
import { ongoingTrainingsQueryOptions } from "../queries/ongoingTrainingsQuery";

type CancelTrainingAlertDialogProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

function CancelTrainingAlertDialog({
  open,
  onOpenChange,
}: CancelTrainingAlertDialogProps) {
  const navigate = useNavigate();
  const ongoingTrainingQuery = useSuspenseQuery(
    ongoingTrainingsQueryOptions.active,
  );
  const deleteOngoingTrainingMutation = useDeleteOngoingTrainingMutation();

  const ongoingTraining = ongoingTrainingQuery.data;

  const handleCancel = () => {
    deleteOngoingTrainingMutation.mutate(
      {
        ongoingTrainingId: ongoingTraining.training.id,
      },
      {
        onSuccess: () => {
          onOpenChange(false);
          navigate({ to: "/trainings/workouts" });
        },
      },
    );
  };

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Cancel Training</AlertDialogTitle>
          <AlertDialogDescription>
            Are you sure you want to cancel this training? All progress will be
            lost and this action cannot be undone.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Keep Training</AlertDialogCancel>
          <AlertDialogAction asChild>
            <ButtonWithLoading
              variant="destructive"
              className="min-w-[100px]"
              isLoading={deleteOngoingTrainingMutation.isDelayedPending}
              disabled={deleteOngoingTrainingMutation.isPending}
              onClick={(e) => {
                e.stopPropagation();
                e.preventDefault();
                handleCancel();
              }}
            >
              Cancel Training
            </ButtonWithLoading>
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

export default CancelTrainingAlertDialog;
