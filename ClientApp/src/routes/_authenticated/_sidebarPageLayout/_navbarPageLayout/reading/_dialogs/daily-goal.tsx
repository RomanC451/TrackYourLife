import { createFileRoute } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";
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
import useSetReadingDailyGoalMutation from "@/features/reading/goals/mutations/useSetReadingDailyGoalMutation";
import { readingDailyGoalQueryOptions } from "@/features/reading/goals/queries/readingDailyGoalQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/reading/_dialogs/daily-goal",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const navigateBack = useNavigateBackOrDefault({
    to: "/reading/dashboard",
  });

  const { data: goal, isPending: isGoalLoading } = useQuery(
    readingDailyGoalQueryOptions.current(),
  );
  const setGoalMutation = useSetReadingDailyGoalMutation();

  const [value, setValue] = useState("");
  const [error, setError] = useState<string | undefined>();

  useEffect(() => {
    if (goal?.value != null) {
      setValue(String(goal.value));
    }
  }, [goal]);

  const onSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError(undefined);

    const parsed = Number.parseInt(value, 10);
    if (!Number.isFinite(parsed) || parsed < 1 || parsed > 500) {
      setError("Enter a number between 1 and 500.");
      return;
    }

    setGoalMutation.mutate(
      { value: parsed, existingGoalId: goal?.id },
      { onSuccess: navigateBack },
    );
  };

  return (
    <Dialog open onOpenChange={(open) => !open && navigateBack()}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Daily reading goal</DialogTitle>
          <DialogDescription>
            How many pages do you want to read per day?
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={onSubmit} className="space-y-4">
          <div>
            <Label htmlFor="pages">Pages per day</Label>
            <Input
              id="pages"
              type="number"
              min={1}
              max={500}
              value={value}
              onChange={(e) => setValue(e.target.value)}
              disabled={isGoalLoading}
            />
            {error && (
              <p className="text-destructive mt-1 text-sm">{error}</p>
            )}
          </div>
          <DialogFooter>
            <ButtonWithLoading
              type="submit"
              isLoading={setGoalMutation.isPending}
            >
              Save goal
            </ButtonWithLoading>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
