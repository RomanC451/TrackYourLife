import { useQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import useSetWorkoutsWeeklyGoalMutation from "@/features/trainings/workoutPlans/mutations/useSetWorkoutsWeeklyGoalMutation";
import { workoutsWeeklyGoalQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutsWeeklyGoalQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/_dialogs/workouts-goal",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/trainings/workouts",
  });

  const { data: goal, isPending: isGoalLoading } = useQuery(
    workoutsWeeklyGoalQueryOptions.current(),
  );
  const setGoalMutation = useSetWorkoutsWeeklyGoalMutation();

  const [value, setValue] = useState("");
  const [error, setError] = useState<string | undefined>(undefined);

  useEffect(() => {
    if (goal?.value != null) {
      setValue(String(goal.value));
    }
  }, [goal]);

  const onSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError(undefined);

    const parsed = Number.parseInt(value, 10);
    if (!Number.isFinite(parsed) || parsed < 1 || parsed > 100) {
      setError("Enter a number between 1 and 100.");
      return;
    }

    setGoalMutation.mutate(
      {
        value: parsed,
        existingGoalId: goal?.id,
      },
      {
        onSuccess: () => {
          navigateBackOrDefault();
        },
      },
    );
  };

  return (
    <Dialog
      onOpenChange={(state) => {
        if (!state) {
          navigateBackOrDefault();
        }
      }}
      defaultOpen={true}
    >
      <DialogContent className="sm:max-w-md" withoutOverlay>
        <form onSubmit={onSubmit}>
          <DialogHeader className="text-left">
            <DialogTitle className="text-xl font-semibold">Weekly workout target</DialogTitle>
            <DialogDescription>
              How many workouts do you want to complete each week? Progress on the workouts page
              uses Monday–Sunday.
            </DialogDescription>
          </DialogHeader>

          <div className="grid gap-2 py-4">
            <Label htmlFor="weekly-workouts-goal">Workouts per week</Label>
            <Input
              id="weekly-workouts-goal"
              type="number"
              inputMode="numeric"
              min={1}
              max={100}
              value={value}
              onChange={(ev) => setValue(ev.target.value)}
              disabled={isGoalLoading}
              autoComplete="off"
            />
            {error ? <p className="text-sm text-destructive">{error}</p> : null}
          </div>

          <DialogFooter className="gap-2 sm:gap-0">
            <ButtonWithLoading
              type="submit"
              disabled={setGoalMutation.isPending || isGoalLoading}
              isLoading={setGoalMutation.isDelayedPending}
            >
              Save
            </ButtonWithLoading>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
