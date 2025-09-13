import { useSuspenseQuery } from "@tanstack/react-query";
import { useNavigate, useRouter, useSearch } from "@tanstack/react-router";

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { exercisesQueryOptions } from "@/features/trainings/exercises/queries/exercisesQuery";
import { UseCustomMutationResult } from "@/hooks/useCustomMutation";

import { TrainingFormSchema } from "../../data/trainingsSchemas";
import { TrainingMutationVariables } from "../../mutations/useCreateTrainingMutation";
import TrainingForm from "./TrainingForm";
import useTrainingDialog from "./useTrainingDialog";

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
    title: "Create New Workout",
    description: "Create a new workout",
    submitButtonText: "Create",
  },
  edit: {
    title: "Edit Workout",
    description: "Edit the details of this workout",
    submitButtonText: "Save",
  },
};

export default function TrainingDialog<TResponse>({
  dialogType,
  mutation,
  defaultValues,
  onClose,
  onSuccess,
}: {
  dialogType: DialogType;
  mutation: UseCustomMutationResult<
    TResponse,
    Error | undefined,
    TrainingMutationVariables,
    unknown
  >;
  defaultValues: TrainingFormSchema;
  onClose?: () => void;
  onSuccess?: () => void;
}) {
  const { tab } = useSearch({
    from:
      dialogType === "edit"
        ? "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/_dialogs/edit/$workoutId"
        : "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/_dialogs/create",
  });

  const navigate = useNavigate();

  const router = useRouter();

  const setTab = (tab: "details" | "exercises") => {
    navigate({
      to: router.history.location.pathname,
      search: { tab: tab },
      replace: true,
    });
  };

  const { handleCustomSubmit, form } = useTrainingDialog({
    onSuccess: () => {
      onSuccess?.();
      form.resetSessionStorage();
    },
    mutation: mutation,
    defaultValues: defaultValues,
    setTab: setTab,
  });

  useSuspenseQuery(exercisesQueryOptions.all);

  return (
    <Dialog
      onOpenChange={(state) => {
        if (!state) onClose?.();
      }}
      defaultOpen={true}
    >
      <DialogContent className="p-6" withoutOverlay>
        <DialogHeader>
          <DialogTitle className="mb-2">
            {dialogTexts[dialogType].title} {form.isDirty ? "*" : ""}
          </DialogTitle>
          <DialogDescription hidden>
            {dialogTexts[dialogType].description}
          </DialogDescription>
        </DialogHeader>
        <TrainingForm
          onCancel={() => {
            form.resetSessionStorage();
            onClose?.();
          }}
          tab={tab}
          setTab={setTab}
          form={form}
          handleCustomSubmit={handleCustomSubmit}
          submitButtonText={dialogTexts[dialogType].submitButtonText}
          pendingState={mutation.pendingState}
        />
      </DialogContent>
    </Dialog>
  );
}
