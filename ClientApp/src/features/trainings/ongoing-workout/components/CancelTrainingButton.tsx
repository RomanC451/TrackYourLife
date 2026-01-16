import { useState } from "react";
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
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";

import useDeleteOngoingTrainingMutation from "../mutations/useDeleteOngoingTrainingMutation";
import { ongoingTrainingsQueryOptions } from "../queries/ongoingTrainingsQuery";

function CancelTrainingButton() {
  const [isOpen, setIsOpen] = useState(false);
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
          setIsOpen(false);
          navigate({ to: "/trainings/workouts" });
        },
      },
    );
  };

  return (
    <AlertDialog open={isOpen} onOpenChange={setIsOpen}>
      <AlertDialogTrigger asChild>
        <Button
          variant="destructive"
          disabled={ongoingTraining.isLoading}
          size="sm"
        >
          {/* <X className="mr-1 size-2" /> */}
          Cancel Training
        </Button>
      </AlertDialogTrigger>
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

export default CancelTrainingButton;
