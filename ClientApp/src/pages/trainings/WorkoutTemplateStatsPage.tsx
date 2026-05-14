import { useMemo, useState } from "react";
import { useQuery, useSuspenseQuery } from "@tanstack/react-query";
import { format } from "date-fns";
import type { DateRange } from "react-day-picker";
import {
  ArrowLeft,
  BarChart3,
  Calendar,
  CheckCircle2,
  Clock,
  Flame,
  Layers,
} from "lucide-react";
import { Link } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { DateRangeSelector } from "@/components/common/DateRangeSelector";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { getDifficultyColor } from "@/features/trainings/exercises/utils/exercisesUtils";
import { workoutHistoryQueryOptions } from "@/features/trainings/history/queries/useWorkoutHistoryQuery";
import { trainingTemplatesUsageQueryOptions } from "@/features/trainings/overview/queries/useTrainingTemplatesUsageQuery";
import { trainingsQueryOptions } from "@/features/trainings/trainings/queries/trainingsQueries";
import { DateOnly, getDateOnly } from "@/lib/date";
import { formatDurationMs } from "@/lib/time";
import { cn } from "@/lib/utils";
import type { WorkoutHistoryDto } from "@/services/openapi";

type ApiDateRange = {
  startDate: DateOnly | null;
  endDate: DateOnly | null;
};

function TemplateUsageSkeleton() {
  return (
    <div className="grid gap-3 sm:grid-cols-2">
      {Array.from({ length: 4 }).map((_, i) => (
        <Skeleton key={i} className="h-24 w-full rounded-lg" />
      ))}
    </div>
  );
}

function WorkoutTemplateStatsPage({ workoutId }: { workoutId: string }) {
  const { data: trainings } = useSuspenseQuery(trainingsQueryOptions.all);
  const training = useMemo(
    () => trainings.find((t) => t.id === workoutId),
    [trainings, workoutId],
  );

  const [apiRange, setApiRange] = useState<ApiDateRange>(() => ({
    startDate: getDateOnly(new Date(Date.now() - 30 * 24 * 60 * 60 * 1000)),
    endDate: getDateOnly(new Date()),
  }));

  const [pickerRange, setPickerRange] = useState<DateRange | undefined>(() => ({
    from: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000),
    to: new Date(),
  }));

  const usageQuery = useQuery(
    trainingTemplatesUsageQueryOptions.byDateRange(
      apiRange.startDate,
      apiRange.endDate,
    ),
  );

  const historyQuery = useQuery(
    workoutHistoryQueryOptions.byDateRange(
      apiRange.startDate,
      apiRange.endDate,
    ),
  );

  const usageRow = useMemo(() => {
    const list = usageQuery.data;
    if (!Array.isArray(list)) {
      return undefined;
    }
    return list.find((row) => row.trainingId === workoutId);
  }, [usageQuery.data, workoutId]);

  const sessionsForTemplate = useMemo(() => {
    const list = historyQuery.data;
    if (!Array.isArray(list)) {
      return [];
    }
    return list
      .filter((w) => w.trainingId === workoutId)
      .sort(
        (a, b) =>
          new Date(b.finishedOnUtc).getTime() -
          new Date(a.finishedOnUtc).getTime(),
      );
  }, [historyQuery.data, workoutId]);

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

  const blueprintStats = useMemo(() => {
    if (!training) {
      return null;
    }
    const exercises = training.exercises ?? [];
    const totalSets = exercises.reduce(
      (sum, ex) => sum + (ex.exerciseSets?.length ?? 0),
      0,
    );
    return { exerciseCount: exercises.length, totalSets };
  }, [training]);

  if (!training || !blueprintStats) {
    return (
      <PageCard className="max-w-2xl">
        <PageTitle title="Workout stats" />
        <p className="mt-4 text-muted-foreground">
          This workout template was not found. It may have been deleted.
        </p>
        <Button className="mt-6" variant="secondary" asChild>
          <Link to="/trainings/workouts">
            <ArrowLeft className="mr-2 h-4 w-4" />
            Back to workouts
          </Link>
        </Button>
      </PageCard>
    );
  }

  return (
    <PageCard className="max-w-3xl">
      <div className="sticky top-0 z-10 mb-6 flex flex-col gap-4 border-b border-border bg-background pb-4 sm:flex-row sm:items-center sm:justify-between">
        <PageTitle title="Workout stats" />
        <DateRangeSelector
          selectedRange={pickerRange}
          handleRangeSelect={handleRangeSelect}
        />
      </div>

      <div className="mb-6 flex flex-wrap items-start justify-between gap-4">
        <div>
          <h2 className="text-2xl font-semibold tracking-tight">{training.name}</h2>
          {training.description ? (
            <p className="mt-1 max-w-xl text-sm text-muted-foreground">
              {training.description}
            </p>
          ) : null}
        </div>
        <Button variant="outline" size="sm" asChild>
          <Link
            to="/trainings/workouts/edit/$workoutId"
            params={{ workoutId: training.id }}
          >
            Edit template
          </Link>
        </Button>
      </div>

      <section className="mb-8">
        <h3 className="mb-3 flex items-center gap-2 text-sm font-semibold uppercase tracking-wide text-muted-foreground">
          <BarChart3 className="h-4 w-4" />
          Usage in selected range
        </h3>
        {usageQuery.isPending ? (
          <TemplateUsageSkeleton />
        ) : usageQuery.isError ? (
          <p className="text-sm text-destructive">Could not load usage statistics.</p>
        ) : (
          <div className="grid gap-3 sm:grid-cols-2">
            <Card className="border-border/60">
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Sessions completed
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-2xl font-bold tabular-nums">
                  {usageRow?.totalCompleted ?? 0}
                </p>
                <p className="mt-1 text-xs text-muted-foreground">
                  Fully finished: {usageRow?.totalFullyCompleted ?? 0} · With skips:{" "}
                  {usageRow?.totalWithSkippedExercises ?? 0}
                </p>
              </CardContent>
            </Card>
            <Card className="border-border/60">
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Full completion rate
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-2xl font-bold tabular-nums">
                  {(usageRow?.completionRate ?? 0).toFixed(1)}%
                </p>
              </CardContent>
            </Card>
            <Card className="border-border/60">
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Avg duration
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-2xl font-bold tabular-nums">
                  {usageRow && usageRow.averageDurationSeconds > 0
                    ? formatDurationMs(usageRow.averageDurationSeconds * 1000)
                    : "—"}
                </p>
              </CardContent>
            </Card>
            <Card className="border-border/60">
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Avg calories
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-2xl font-bold tabular-nums">
                  {typeof usageRow?.averageCaloriesBurned === "number"
                    ? `${usageRow.averageCaloriesBurned.toLocaleString()} kcal`
                    : "—"}
                </p>
              </CardContent>
            </Card>
          </div>
        )}
        {!usageQuery.isPending && !usageRow ? (
          <p className="mt-3 text-sm text-muted-foreground">
            No completed sessions for this template in this date range (or only templates with
            sessions appear in usage totals).
          </p>
        ) : null}
      </section>

      <section className="mb-8">
        <h3 className="mb-3 text-sm font-semibold uppercase tracking-wide text-muted-foreground">
          Template blueprint
        </h3>
        <Card className="border-border/60">
          <CardContent className="flex flex-wrap gap-3 pt-6">
            <Badge
              variant="outline"
              className={cn(getDifficultyColor(training.difficulty), "font-normal")}
            >
              {training.difficulty}
            </Badge>
            <span className="flex items-center gap-1.5 text-sm text-muted-foreground">
              <Clock className="h-4 w-4" />
              {training.duration ? `${training.duration} min planned` : "No duration set"}
            </span>
            <span className="flex items-center gap-1.5 text-sm text-muted-foreground">
              <Layers className="h-4 w-4" />
              {blueprintStats.exerciseCount} exercises · {blueprintStats.totalSets} sets
            </span>
            {training.muscleGroups?.length ? (
              <span className="text-sm text-muted-foreground">
                {training.muscleGroups.join(", ")}
              </span>
            ) : null}
          </CardContent>
        </Card>
      </section>

      <section>
        <h3 className="mb-3 flex items-center gap-2 text-sm font-semibold uppercase tracking-wide text-muted-foreground">
          <Calendar className="h-4 w-4" />
          Sessions in range
        </h3>
        {historyQuery.isPending ? (
          <div className="space-y-2">
            <Skeleton className="h-16 w-full" />
            <Skeleton className="h-16 w-full" />
          </div>
        ) : historyQuery.isError ? (
          <p className="text-sm text-destructive">Could not load workout history.</p>
        ) : sessionsForTemplate.length === 0 ? (
          <p className="text-sm text-muted-foreground">
            No sessions for this template in the selected range.
          </p>
        ) : (
          <ul className="space-y-2">
            {sessionsForTemplate.map((session) => (
              <SessionRow key={session.id} session={session} />
            ))}
          </ul>
        )}
      </section>

      <div className="mt-8">
        <Button variant="secondary" asChild>
          <Link to="/trainings/workouts">
            <ArrowLeft className="mr-2 h-4 w-4" />
            Workouts
          </Link>
        </Button>
      </div>
    </PageCard>
  );
}

function SessionRow({ session }: { session: WorkoutHistoryDto }) {
  const finishedAt = new Date(session.finishedOnUtc);
  return (
    <li className="flex flex-wrap items-center justify-between gap-3 rounded-lg border border-border/60 bg-secondary/20 px-4 py-3">
      <div className="min-w-0 space-y-1">
        <p className="text-sm font-medium">
          {format(finishedAt, "MMM d, yyyy")} at {format(finishedAt, "h:mm a")}
        </p>
        <div className="flex flex-wrap items-center gap-3 text-xs text-muted-foreground">
          <span className="flex items-center gap-1">
            <CheckCircle2 className="h-3.5 w-3.5" />
            {session.completedExercisesCount}/{session.totalExercisesCount}
          </span>
          <span className="flex items-center gap-1">
            <Clock className="h-3.5 w-3.5" />
            {formatDurationMs(session.durationSeconds * 1000)}
          </span>
          {session.caloriesBurned != null ? (
            <span className="flex items-center gap-1">
              <Flame className="h-3.5 w-3.5 text-primary" />
              {session.caloriesBurned} kcal
            </span>
          ) : null}
        </div>
      </div>
      <Button variant="outline" size="sm" asChild>
        <Link
          to="/trainings/workout-session-stats/$ongoingTrainingId"
          params={{ ongoingTrainingId: session.id }}
        >
          Session stats
        </Link>
      </Button>
    </li>
  );
}

export default WorkoutTemplateStatsPage;
