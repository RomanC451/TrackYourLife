import { Dumbbell } from "lucide-react";

import { Button } from "@/components/ui/button";

type WorkoutHistoryEmptyStateProps = {
  onStartWorkout: () => void;
};

export function WorkoutHistoryEmptyState({
  onStartWorkout,
}: WorkoutHistoryEmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center rounded-lg border border-dashed border-border bg-muted/30 px-6 py-16 text-center">
      <div className="mb-4 flex h-14 w-14 items-center justify-center rounded-full bg-muted">
        <Dumbbell className="h-7 w-7 text-muted-foreground" />
      </div>
      <h3 className="mb-1 text-lg font-semibold">No workouts in this period</h3>
      <p className="mb-6 max-w-sm text-sm text-muted-foreground">
        Try widening the date range, or start a workout to see it here once you
        finish.
      </p>
      <Button type="button" onClick={onStartWorkout}>
        Start a workout
      </Button>
    </div>
  );
}
