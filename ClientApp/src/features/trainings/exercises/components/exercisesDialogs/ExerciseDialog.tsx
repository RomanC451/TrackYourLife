import { useState } from "react";
import { UseMutationResult } from "@tanstack/react-query";
import { ErrorOption } from "react-hook-form";

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { MutationPendingState } from "@/hooks/useCustomMutation";

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

type DialogType = "create" | "edit";

const dialogTexts: Record<
  DialogType,
  {
    title: string;
    description: string;
    submitButtonText: string;
  }
> = {
  create: {
    title: "Create New Exercise",
    description: "Create a new exercise",
    submitButtonText: "Create",
  },
  edit: {
    title: "Edit Exercise",
    description: "Edit the details of this exercise",
    submitButtonText: "Save",
  },
};

function ExerciseDialog<TResponse>({
  dialogType,
  mutation,
  defaultValues,
  pendingState,
  onSuccess,
  onClose,
}: {
  dialogType: DialogType;
  mutation: UseMutationResult<
    TResponse,
    Error | undefined,
    ExerciseMutationVariables,
    unknown
  >;
  defaultValues: ExerciseFormSchema;
  pendingState: MutationPendingState;
  onSuccess?: () => void;
  onClose?: () => void;
}) {
  const [tab, setTab] = useState("details");

  const { handleCustomSubmit, form } = useExerciseDialog({
    onSuccess: () => {
      form.reset(defaultValues);
      onSuccess?.();
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
      onOpenChange={(state) => {
        if (!state) {
          resetDialog();
          onClose?.();
        }
      }}
      defaultOpen={true}
    >
      <DialogContent className="p-6" withoutOverlay>
        <DialogHeader>
          <DialogTitle className="mb-2">
            {dialogTexts[dialogType].title}
          </DialogTitle>
          <DialogDescription hidden>
            {dialogTexts[dialogType].description}
          </DialogDescription>
        </DialogHeader>
        <ExerciseForm
          tab={tab}
          setTab={setTab}
          form={form}
          handleCustomSubmit={handleCustomSubmit}
          submitButtonText={dialogTexts[dialogType].submitButtonText}
          pendingState={pendingState}
        />
      </DialogContent>
    </Dialog>
  );
}

export default ExerciseDialog;
