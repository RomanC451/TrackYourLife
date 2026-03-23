import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import type { WorkoutHistoryDto } from "@/services/openapi";

import useUpdateOngoingTrainingMutation from "../mutations/useUpdateOngoingTrainingMutation";

const formSchema = z.object({
  caloriesBurned: z.coerce.number().int().min(0),
  durationMinutes: z.coerce.number().int().min(0),
});

export type EditWorkoutHistoryFormValues = z.infer<typeof formSchema>;

function defaultValuesFromWorkout(workout: WorkoutHistoryDto): EditWorkoutHistoryFormValues {
  return {
    caloriesBurned: workout.caloriesBurned ?? 0,
    durationMinutes: Math.max(0, Math.round(workout.durationSeconds / 60)),
  };
}

type EditWorkoutHistoryDialogProps = {
  workout: WorkoutHistoryDto;
  onClose: () => void;
};

export function EditWorkoutHistoryDialog({
  workout,
  onClose,
}: EditWorkoutHistoryDialogProps) {
  const updateMutation = useUpdateOngoingTrainingMutation();

  const form = useForm<EditWorkoutHistoryFormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: defaultValuesFromWorkout(workout),
  });

  const onSubmit = (values: EditWorkoutHistoryFormValues) => {
    updateMutation.mutate(
      {
        ongoingTrainingId: workout.id,
        request: {
          caloriesBurned: values.caloriesBurned,
          durationMinutes: values.durationMinutes,
        },
      },
      {
        onSuccess: () => {
          onClose();
        },
      },
    );
  };

  return (
    <Dialog
      open
      onOpenChange={(open) => {
        if (!open) {
          onClose();
        }
      }}
    >
      <DialogContent className="sm:max-w-md" onClick={(e) => e.stopPropagation()}>
        <DialogHeader>
          <DialogTitle>Edit workout</DialogTitle>
          <DialogDescription>
            Update calories and duration for &quot;{workout.trainingName}&quot;. Duration
            is in minutes; end time is recalculated from your start time.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="caloriesBurned"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Calories burned (kcal)</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      min={0}
                      step={1}
                      inputMode="numeric"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="durationMinutes"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Duration (minutes)</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      min={0}
                      step={1}
                      inputMode="numeric"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <DialogFooter className="gap-2 sm:gap-0">
              <Button
                type="button"
                variant="outline"
                onClick={onClose}
                disabled={updateMutation.isPending}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={updateMutation.isPending}>
                {updateMutation.isPending ? "Saving…" : "Save"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
