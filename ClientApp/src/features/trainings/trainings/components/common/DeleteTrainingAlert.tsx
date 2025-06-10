import { useState } from "react";
import { Trash } from "lucide-react";

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
import { TrainingDto } from "@/services/openapi";

import useDeleteTrainingMutation from "../../mutations/useDeleteTrainingMutation";

function DeleteTrainingAlert({
  training,
  force = false,
}: {
  training: TrainingDto;
  force?: boolean;
}) {
  const { deleteTrainingMutation, isPending } = useDeleteTrainingMutation();

  const [isOpen, setIsOpen] = useState(false);

  return (
    <AlertDialog open={isOpen} onOpenChange={setIsOpen}>
      <AlertDialogTrigger asChild>
        <Button variant="destructive">
          <Trash className="mr-1 h-4 w-4" /> Delete
        </Button>
      </AlertDialogTrigger>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Delete Training</AlertDialogTitle>
          <AlertDialogDescription>
            {force ? (
              <strong>
                The training "{training.name}" is currently active.
              </strong>
            ) : (
              ""
            )}
            <br />
            Are you sure you want to delete it? This action cannot be undone.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Cancel</AlertDialogCancel>
          <AlertDialogAction asChild>
            <ButtonWithLoading
              variant="destructive"
              className="min-w-[100px]"
              isLoading={isPending.isLoading}
              disabled={!isPending.isLoaded}
              onClick={(e) => {
                e.stopPropagation();
                e.preventDefault();
                deleteTrainingMutation.mutate(
                  {
                    id: training.id,
                    name: training.name,
                    force,
                  },
                  {
                    onSuccess: () => {
                      setIsOpen(false);
                    },
                  },
                );
              }}
            >
              Delete training
            </ButtonWithLoading>
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

export default DeleteTrainingAlert;
