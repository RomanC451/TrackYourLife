import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import type { DateRange } from "react-day-picker";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { DateRangeSelector } from "@/components/common/DateRangeSelector";
import HandleQuery from "@/components/handle-query";
import { EditWorkoutHistoryDialog } from "@/features/trainings/history/components/EditWorkoutHistoryDialog";
import { WorkoutHistoryCard } from "@/features/trainings/history/components/WorkoutHistoryCard";
import { WorkoutSessionDetailsDialog } from "@/features/trainings/history/components/WorkoutSessionDetailsDialog";
import { WorkoutHistoryCardSkeleton } from "@/features/trainings/history/components/WorkoutHistoryCardSkeleton";
import { WorkoutHistoryEmptyState } from "@/features/trainings/history/components/WorkoutHistoryEmptyState";
import { groupWorkoutHistoryByDate } from "@/features/trainings/history/groupWorkoutHistoryByDate";
import { workoutHistoryQueryOptions } from "@/features/trainings/history/queries/useWorkoutHistoryQuery";
import { trainingsQueryOptions } from "@/features/trainings/trainings/queries/trainingsQueries";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { DateOnly, getDateOnly } from "@/lib/date";
import type { WorkoutHistoryDto } from "@/services/openapi";

type ApiDateRange = {
  startDate: DateOnly | null;
  endDate: DateOnly | null;
};

function WorkoutHistoryPage() {
  const navigate = useNavigate();
  const [editingWorkout, setEditingWorkout] = useState<WorkoutHistoryDto | null>(
    null,
  );
  const [sessionDetailWorkout, setSessionDetailWorkout] =
    useState<WorkoutHistoryDto | null>(null);
  const [apiRange, setApiRange] = useState<ApiDateRange>(() => ({
    startDate: getDateOnly(new Date(Date.now() - 30 * 24 * 60 * 60 * 1000)),
    endDate: getDateOnly(new Date()),
  }));

  const [pickerRange, setPickerRange] = useState<DateRange | undefined>(() => ({
    from: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000),
    to: new Date(),
  }));

  const { query } = useCustomQuery(
    workoutHistoryQueryOptions.byDateRange(
      apiRange.startDate,
      apiRange.endDate,
    ),
  );

  const handleRangeSelect = (range: DateRange | undefined) => {
    setPickerRange(range);

    if (!range?.from || !range.to) {
      setApiRange({
        startDate: null,
        endDate: null,
      });
      return;
    }

    setApiRange({
      startDate: getDateOnly(range.from),
      endDate: getDateOnly(range.to),
    });
  };

  return (
    <PageCard>
      {editingWorkout ? (
        <EditWorkoutHistoryDialog
          key={editingWorkout.id}
          workout={editingWorkout}
          onClose={() => setEditingWorkout(null)}
        />
      ) : null}

      {sessionDetailWorkout ? (
        <WorkoutSessionDetailsDialog
          key={sessionDetailWorkout.id}
          workout={sessionDetailWorkout}
          onClose={() => setSessionDetailWorkout(null)}
        />
      ) : null}

      <div className="flex flex-col gap-4 border-b border-border pb-2 pt-2 sm:flex-row sm:items-center sm:justify-between">
        <PageTitle title="Workout history" />
        <DateRangeSelector
          selectedRange={pickerRange}
          handleRangeSelect={handleRangeSelect}
        />
      </div>

      <div className="mt-4">
        <HandleQuery
          query={query}
          pending={WorkoutHistoryListSkeleton}
          success={(data) => (
            <WorkoutHistoryList
              workouts={data}
              onStartWorkout={() =>
                navigate({ to: "/trainings/workouts" })
              }
              onEditWorkout={setEditingWorkout}
              onViewSessionDetails={setSessionDetailWorkout}
            />
          )}
        />
      </div>
    </PageCard>
  );
}

function WorkoutHistoryListSkeleton() {
  return (
    <div className="space-y-4">
      {Array.from({ length: 5 }).map((_, i) => (
        <WorkoutHistoryCardSkeleton key={i} />
      ))}
    </div>
  );
}

function WorkoutHistoryList({
  workouts,
  onStartWorkout,
  onEditWorkout,
  onViewSessionDetails,
}: {
  workouts: WorkoutHistoryDto[];
  onStartWorkout: () => void;
  onEditWorkout: (workout: WorkoutHistoryDto) => void;
  onViewSessionDetails: (workout: WorkoutHistoryDto) => void;
}) {
  const { data: trainings = [] } = useQuery(trainingsQueryOptions.all);

  const muscleGroupsByTrainingId = useMemo(() => {
    const map = new Map<string, string[]>();
    for (const t of trainings) {
      map.set(t.id, t.muscleGroups);
    }
    return map;
  }, [trainings]);

  if (workouts.length === 0) {
    return <WorkoutHistoryEmptyState onStartWorkout={onStartWorkout} />;
  }

  const groupedWorkouts = groupWorkoutHistoryByDate(workouts);
  const newestWorkoutId = [...workouts].sort(
    (a, b) =>
      new Date(b.finishedOnUtc).getTime() -
      new Date(a.finishedOnUtc).getTime(),
  )[0]?.id;

  return (
    <div className="space-y-8">
      {groupedWorkouts.map((group) => (
        <div key={group.label} className="space-y-3">
          <h3 className="text-sm font-medium uppercase tracking-wider text-muted-foreground">
            {group.label}
          </h3>
          <div className="space-y-3">
            {group.workouts.map((workout) => (
              <WorkoutHistoryCard
                key={workout.id}
                workout={workout}
                muscleGroups={muscleGroupsByTrainingId.get(workout.trainingId)}
                isNewest={workout.id === newestWorkoutId}
                onEditWorkout={onEditWorkout}
                onViewSessionDetails={onViewSessionDetails}
              />
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}

export default WorkoutHistoryPage;
