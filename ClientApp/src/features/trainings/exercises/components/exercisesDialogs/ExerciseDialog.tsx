import { useState } from "react";
import { UseMutationResult } from "@tanstack/react-query";
import { ErrorOption } from "react-hook-form";
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
import { ExerciseDto } from "@/services/openapi";

import { ExerciseFormSchema } from "../../data/exercisesSchemas";
import ExerciseForm from "./ExerciseForm";
import useExerciseDialog from "./useExerciseDialog";

export type ExerciseMutationVariables = {
  id?: string;
  request: ExerciseFormSchema;
  setError: (
    name: keyof ExerciseFormSchema,
    error: ErrorOption,
    options?: {
      shouldFocus: boolean;
    },
  ) => void;
};

function ExerciseDialog<TResponse>({
  buttonComponent,
  buttonVariant,
  submitButtonText,
  dialogButtonText,
  dialogTitle,
  dialogDescription,
  mutation,
  defaultValues,
  isPending,
  defaultOpen,
  onSuccess,
  onClose,
}: {
  buttonComponent?: React.ReactNode;
  buttonVariant?: ButtonVariants;
  submitButtonText: string;
  dialogButtonText: string;
  dialogTitle: string;
  dialogDescription: string;
  mutation: UseMutationResult<
    TResponse,
    Error | undefined,
    ExerciseMutationVariables,
    unknown
  >;
  defaultValues: ExerciseFormSchema;
  isPending: LoadingState;
  onSuccess?: (exercise: Partial<ExerciseDto>) => void;
  defaultOpen?: boolean;
  onClose?: () => void;
}) {
  const [dialogState, toggleDialogState] = useToggle(defaultOpen ?? false);

  const [tab, setTab] = useState("details");

  const { handleCustomSubmit, form } = useExerciseDialog({
    onSuccess: (exercise) => {
      form.reset(defaultValues);
      toggleDialogState();
      onSuccess?.(exercise);
    },
    mutation: mutation,
    defaultValues: defaultValues,
    setTab,
  });

  const resetDialog = () => {
    form.reset(defaultValues);
    setTab("details");
  };

  return (
    <Dialog
      open={dialogState}
      onOpenChange={(state) => {
        toggleDialogState();
        resetDialog();
        if (!state) {
          onClose?.();
        }
      }}
    >
      {!defaultOpen && (
        <DialogTrigger asChild>
          {buttonComponent ? (
            buttonComponent
          ) : (
            <Button variant={buttonVariant}>{dialogButtonText}</Button>
          )}
        </DialogTrigger>
      )}
      <DialogContent className="p-6" id="exercise-dialog">
        <DialogHeader>
          <DialogTitle className="mb-2">{dialogTitle}</DialogTitle>
          <DialogDescription hidden>{dialogDescription}</DialogDescription>
        </DialogHeader>
        <ExerciseForm
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

export default ExerciseDialog;
