import { useState } from "react";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { DateRangeSelector } from "@/components/common/DateRangeSelector";
import type { DateRange } from "react-day-picker";
import HandleQuery from "@/components/handle-query";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { DateOnly, getDateOnly } from "@/lib/date";
import { formatDurationMs } from "@/lib/time";
import { WorkoutHistoryDto } from "@/services/openapi";

import { workoutHistoryQueryOptions } from "@/features/trainings/history/queries/useWorkoutHistoryQuery";

type ApiDateRange = {
  startDate: DateOnly | null;
  endDate: DateOnly | null;
};

function WorkoutHistoryPage() {
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
      <div className="flex flex-col gap-4 border-b border-border pb-2 pt-2 sm:flex-row sm:items-center sm:justify-between">
        <PageTitle title="Workout history" />
        <DateRangeSelector
          selectedRange={pickerRange}
          handleRangeSelect={handleRangeSelect}
        />
      </div>

      <div className="mt-4 space-y-4">
        <HandleQuery
          query={query}
          pending={WorkoutHistoryListSkeleton}
          success={WorkoutHistoryList}
        />
      </div>
    </PageCard>
  );
}

function WorkoutHistoryListSkeleton() {
  return (
    <div className="space-y-3">
      {["a", "b", "c", "d"].map((key) => (
        <Card key={key}>
          <CardHeader>
            <Skeleton className="h-4 w-32" />
          </CardHeader>
          <CardContent className="space-y-2">
            <Skeleton className="h-4 w-40" />
            <Skeleton className="h-4 w-24" />
            <Skeleton className="h-4 w-20" />
          </CardContent>
        </Card>
      ))}
    </div>
  );
}

function WorkoutHistoryList(data: WorkoutHistoryDto[]) {
  if (data.length === 0) {
    return (
      <p className="text-sm text-muted-foreground">
        No workouts found for the selected period.
      </p>
    );
  }

  return (
    <div className="space-y-3">
      {data.map((workout) => (
        <Card key={workout.id}>
          <CardHeader>
            <CardTitle className="flex flex-col gap-1 text-base sm:flex-row sm:items-center sm:justify-between">
              <span>{workout.trainingName}</span>
              <span className="text-xs font-normal text-muted-foreground">
                {new Date(workout.startedOnUtc).toLocaleString()}
              </span>
            </CardTitle>
          </CardHeader>
          <CardContent className="grid gap-2 text-sm sm:grid-cols-3">
            <div>
              <p className="text-xs text-muted-foreground">Duration</p>
              <p>{formatDurationMs(workout.durationSeconds * 1000)}</p>
            </div>
            <div>
              <p className="text-xs text-muted-foreground">Finished at</p>
              <p>{new Date(workout.finishedOnUtc).toLocaleString()}</p>
            </div>
            {workout.caloriesBurned !== null && workout.caloriesBurned !== undefined && (
              <div>
                <p className="text-xs text-muted-foreground">Calories</p>
                <p>{`${workout.caloriesBurned.toLocaleString()} kcal`}</p>
              </div>
            )}
          </CardContent>
        </Card>
      ))}
    </div>
  );
}

export default WorkoutHistoryPage;

