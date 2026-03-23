import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { TrainingDto } from "@/services/openapi";

import WorkoutPlanForm, { WorkoutPlanFormValues } from "./WorkoutPlanForm";

type WorkoutPlanDialogProps = {
  title: string;
  description: string;
  submitButtonText: string;
  trainings: TrainingDto[];
  defaultValues: WorkoutPlanFormValues;
  isPending: boolean;
  onClose: () => void;
  onSubmit: (values: WorkoutPlanFormValues) => void;
};

function WorkoutPlanDialog({
  title,
  description,
  submitButtonText,
  trainings,
  defaultValues,
  isPending,
  onClose,
  onSubmit,
}: WorkoutPlanDialogProps) {
  return (
    <Dialog
      onOpenChange={(state) => {
        if (!state) {
          onClose();
        }
      }}
      defaultOpen={true}
    >
      <DialogContent className="sm:max-w-lg" withoutOverlay>
        <DialogHeader className="text-left">
          <DialogTitle className="text-xl font-semibold">{title}</DialogTitle>
          <DialogDescription>{description}</DialogDescription>
        </DialogHeader>

        <WorkoutPlanForm
          trainings={trainings}
          defaultValues={defaultValues}
          submitButtonText={submitButtonText}
          isPending={isPending}
          onCancel={onClose}
          onSubmit={onSubmit}
        />
      </DialogContent>
    </Dialog>
  );
}

export default WorkoutPlanDialog;
