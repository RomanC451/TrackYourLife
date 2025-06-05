import { useState } from "react";
import { UseMutationResult } from "@tanstack/react-query";
import { useToggle } from "usehooks-ts";

import { Button, ButtonVariants } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { LoadingState } from "@/hooks/useDelayedLoading";

import { TrainingFormSchema } from "../../data/trainingsSchemas";
import { TrainingMutationVariables } from "../../mutations/useCreateTrainingMutation";
import TrainingForm from "./TrainingForm";
import useTrainingDialog from "./useTrainingDialog";

export default function TrainingDialog<TResponse>({
  buttonVariant,
  submitButtonText,
  dialogButtonText,
  dialogTitle,
  dialogDescription,
  mutation,
  defaultValues,
  isPending,
}: {
  buttonVariant?: ButtonVariants;
  submitButtonText: string;
  dialogButtonText: string;
  dialogTitle: string;
  dialogDescription: string;
  mutation: UseMutationResult<
    TResponse,
    Error | undefined,
    TrainingMutationVariables,
    unknown
  >;
  defaultValues: TrainingFormSchema;
  isPending: LoadingState;
}) {
  const [dialogState, toggleDialogState] = useToggle(false);

  const [tab, setTab] = useState("details");

  function resetDialog() {
    form.reset(defaultValues);
    setTab("details");
  }

  const { handleCustomSubmit, form } = useTrainingDialog({
    onSuccess: () => {
      toggleDialogState();
      resetDialog();
    },
    mutation: mutation,
    defaultValues: defaultValues,
    setTab,
  });

  return (
    <Dialog
      open={dialogState}
      onOpenChange={() => {
        toggleDialogState();
        resetDialog();
      }}
      // modal={false}
    >
      <DialogTrigger asChild className="w-full text-left">
        <Button variant={buttonVariant}>{dialogButtonText}</Button>
      </DialogTrigger>
      <DialogContent className="p-6">
        <DialogHeader>
          <DialogTitle className="mb-2">{dialogTitle}</DialogTitle>
          <DialogDescription hidden>{dialogDescription}</DialogDescription>
        </DialogHeader>
        <TrainingForm
          onCancel={() => {
            resetDialog();
            toggleDialogState();
          }}
          tab={tab}
          setTab={setTab}
          form={form}
          handleCustomSubmit={handleCustomSubmit}
          submitButtonText={submitButtonText}
          isPending={isPending}
        />
      </DialogContent>
    </Dialog>
  );
}
