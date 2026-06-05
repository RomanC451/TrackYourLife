import { useEffect, useMemo, useState } from "react";
import { format, isSameDay } from "date-fns";
import type { DateRange } from "react-day-picker";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import { Clock, Play, TrendingUp } from "lucide-react";

import { DateRangeSelector } from "@/components/common/DateRangeSelector";
import PageCard from "@/components/common/PageCard";
import { Badge } from "@/components/ui/badge";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import AggregationTypeDropDownMenu from "@/features/trainings/overview/components/AggregationTypeDropDownMenu";
import { WorkoutHistoryCard } from "@/features/trainings/history/components/WorkoutHistoryCard";
import { WorkoutSessionDetailsDialog } from "@/features/trainings/history/components/WorkoutSessionDetailsDialog";
import { getDifficultyColor } from "@/features/trainings/exercises/utils/exercisesUtils";
import useCreateOngoingTrainingMutation from "@/features/trainings/ongoing-workout/mutations/useCreateOngoingTrainingMutation";
import type {
  TrainingStatsChartAggregation,
  TrainingStatsDto,
  TrainingStatsSearch,
} from "@/features/trainings/trainings/queries/trainingStatsQuery";
import { MuscleGroupWorkoutIcon } from "@/features/trainings/utils/muscleGroupWorkoutIcon";
import { colors } from "@/constants/tailwindColors";
import { getDateOnly, parseDateOnly, type DateOnly } from "@/lib/date";
import { formatDuration, formatDurationMs } from "@/lib/time";
import { cn } from "@/lib/utils";
import type { WorkoutHistoryDto } from "@/services/openapi";
import type { AggregationType } from "@/services/openapi";
import { useNavigate } from "@tanstack/react-router";

const rechartsTooltipStyles = {
  contentStyle: {
    backgroundColor: "hsl(var(--background))",
    borderColor: "hsl(var(--border))",
    borderRadius: "var(--radius)",
    color: "hsl(var(--foreground))",
  },
  itemStyle: {
    color: "hsl(var(--foreground))",
  },
  labelStyle: {
    color: "hsl(var(--foreground))",
  },
} as const;

type TrainingStatsPageProps = {
  stats: TrainingStatsDto;
  trainingId: string;
  search: TrainingStatsSearch;
  isChartRefetching?: boolean;
  onChartAggregationChange: (aggregation: TrainingStatsChartAggregation) => void;
  onCustomDateRangeChange: (startDate: DateOnly, endDate: DateOnly) => void;
  onAllTimeRangeSelect: () => void;
  onClearCustomDates: () => void;
};

export default function TrainingStatsPage({
  stats,
  trainingId,
  search,
  isChartRefetching = false,
  onChartAggregationChange,
  onCustomDateRangeChange,
  onAllTimeRangeSelect,
  onClearCustomDates,
}: TrainingStatsPageProps) {
  const navigate = useNavigate();
  const createOngoingTrainingMutation = useCreateOngoingTrainingMutation();
  const [sessionDetailWorkout, setSessionDetailWorkout] =
    useState<WorkoutHistoryDto | null>(null);

  const [pickerRange, setPickerRange] = useState<DateRange | undefined>(() =>
    search.startDate && search.endDate
      ? {
          from: parseDateOnly(search.startDate),
          to: parseDateOnly(search.endDate),
        }
      : undefined,
  );

  useEffect(() => {
    if (search.startDate && search.endDate) {
      setPickerRange({
        from: parseDateOnly(search.startDate),
        to: parseDateOnly(search.endDate),
      });
    } else {
      setPickerRange(undefined);
    }
  }, [search.startDate, search.endDate]);

  const handleDateRangeSelect = (range: DateRange | undefined) => {
    setPickerRange(range);
    if (range === undefined) {
      onAllTimeRangeSelect();
      return;
    }
    if (!range.from || !range.to) {
      onClearCustomDates();
      return;
    }
    onCustomDateRangeChange(getDateOnly(range.from), getDateOnly(range.to));
  };

  const hasSessions = stats.summary.sessionsCompleted > 0;
  const estimatedDurationMinutes = Math.round(stats.estimatedDurationSeconds / 60);
  const isAverageAggregation = stats.chartAggregationType === "Average";
  const durationSeriesLabel = isAverageAggregation ? "Avg duration" : "Total duration";
  const caloriesSeriesLabel = isAverageAggregation ? "Avg calories" : "Total calories";

  const handleAggregationChange = (
    value: AggregationType | ((prev: AggregationType) => AggregationType),
  ) => {
    const next =
      typeof value === "function"
        ? value(search.chartAggregation as AggregationType)
        : value;
    onChartAggregationChange(next as TrainingStatsChartAggregation);
  };

  const durationChartData = useMemo(
    () =>
      stats.durationTrend.map((item) => ({
        date: item.date,
        duration: item.value,
        startDate: item.startDate
          ? new Date(item.startDate)
          : new Date(item.date),
        endDate: item.endDate ? new Date(item.endDate) : new Date(item.date),
      })),
    [stats.durationTrend],
  );

  const frequencyChartData = useMemo(
    () =>
      stats.frequencyTrend.map((item) => ({
        date: item.date,
        workouts: item.workoutCount,
        startDate: item.startDate
          ? new Date(item.startDate)
          : new Date(item.date),
        endDate: item.endDate ? new Date(item.endDate) : new Date(item.date),
      })),
    [stats.frequencyTrend],
  );

  const caloriesChartData = useMemo(
    () =>
      stats.caloriesTrend.map((item) => ({
        date: item.date,
        calories: item.value,
        startDate: item.startDate
          ? new Date(item.startDate)
          : new Date(item.date),
        endDate: item.endDate ? new Date(item.endDate) : new Date(item.date),
      })),
    [stats.caloriesTrend],
  );

  const formatWeeklyTooltipDate = (startDate?: Date, endDate?: Date) => {
    if (!startDate || !endDate) return "";
    if (isSameDay(startDate, endDate)) {
      return format(startDate, "MMM dd, yyyy");
    }
    return `${format(startDate, "EEE, MMM dd")} - ${format(endDate, "EEE, MMM dd, yyyy")}`;
  };

  const formatWeeklyXAxisTick = (value: string) =>
    format(new Date(value), "MMM dd");

  const handleStartWorkout = () => {
    createOngoingTrainingMutation.mutate(
      { trainingId },
      {
        onSuccess: () => {
          navigate({ to: "/trainings/ongoing-workout" });
        },
      },
    );
  };

  return (
    <PageCard className="space-y-4">
      <div className="flex w-full min-w-0 justify-end">
        <DateRangeSelector
          selectedRange={pickerRange}
          handleRangeSelect={handleDateRangeSelect}
        />
      </div>

      <Card>
        <CardHeader className="gap-4 sm:flex sm:flex-row sm:items-start sm:justify-between sm:gap-6">
          <div className="min-w-0 flex-1 space-y-3">
            <CardTitle className="text-balance text-2xl font-bold leading-tight tracking-tight text-foreground sm:text-3xl">
              {stats.trainingName}
            </CardTitle>
            <div className="flex flex-wrap items-center gap-3 text-sm text-muted-foreground">
              <div className="flex items-center text-primary">
                <MuscleGroupWorkoutIcon
                  muscleGroups={stats.muscleGroups}
                  className="mr-1 size-5 shrink-0"
                />
                {stats.muscleGroups.join(", ")}
              </div>
              <Badge
                variant="outline"
                className={cn(getDifficultyColor(stats.difficulty), "font-normal")}
              >
                {stats.difficulty}
              </Badge>
              {estimatedDurationMinutes > 0 ? (
                <div className="flex items-center gap-1">
                  <Clock className="size-4 shrink-0" />
                  {formatDuration(estimatedDurationMinutes)} estimated
                </div>
              ) : null}
              <span>{stats.exerciseCount} exercises</span>
            </div>
          </div>
          {hasSessions ? (
            <div className="shrink-0 text-sm text-muted-foreground sm:pt-1 sm:text-right">
              {stats.summary.sessionsCompleted} completed
              {stats.summary.withSkippedCount > 0
                ? ` · ${stats.summary.withSkippedCount} with skips`
                : ""}
            </div>
          ) : null}
        </CardHeader>

        {hasSessions ? (
          <CardContent className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
            <StatsMetricCard
              label="Completion rate"
              value={`${stats.summary.completionRate.toFixed(0)}%`}
            />
            <StatsMetricCard
              label="Avg duration (in range)"
              value={formatDurationMs(stats.summary.averageDurationSeconds * 1000)}
            />
            <StatsMetricCard
              label="Total duration (in range)"
              value={formatDurationMs(stats.summary.totalDurationSeconds * 1000)}
            />
            <StatsMetricCard
              label="Last performed"
              value={
                stats.summary.lastPerformedOnUtc
                  ? format(new Date(stats.summary.lastPerformedOnUtc), "MMM d, yyyy")
                  : "—"
              }
            />
            {stats.summary.averageCaloriesBurned != null ? (
              <StatsMetricCard
                label="Avg calories (in range)"
                value={`${stats.summary.averageCaloriesBurned.toLocaleString()} kcal`}
              />
            ) : null}
            {stats.summary.totalCaloriesBurned != null ? (
              <StatsMetricCard
                label="Total calories (in range)"
                value={`${stats.summary.totalCaloriesBurned.toLocaleString()} kcal`}
              />
            ) : null}
          </CardContent>
        ) : (
          <CardContent className="space-y-4 py-6 text-center">
            <p className="text-sm text-muted-foreground">
              No completed sessions in this period yet.
            </p>
            <ButtonWithLoading
              className="gap-2"
              onClick={handleStartWorkout}
              isLoading={createOngoingTrainingMutation.isDelayedPending}
            >
              <Play className="h-4 w-4" />
              Start workout
            </ButtonWithLoading>
          </CardContent>
        )}
      </Card>

      {hasSessions ? (
        <>
          <Card>
            <CardHeader>
              <div className="flex flex-col items-center justify-center gap-2 sm:flex-row sm:items-start sm:justify-between">
                <CardTitle className="text-xl">Duration over time</CardTitle>
                <AggregationTypeDropDownMenu
                  aggregationType={search.chartAggregation as AggregationType}
                  setAggregationType={handleAggregationChange}
                  loading={isChartRefetching}
                />
              </div>
            </CardHeader>
            <CardContent className="px-3 py-4">
              <ResponsiveContainer width="100%" height={300}>
                <LineChart data={durationChartData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="date" tickFormatter={formatWeeklyXAxisTick} />
                  <YAxis
                    tickFormatter={(value) => formatDurationMs(value * 1000)}
                  />
                  <Tooltip
                    {...rechartsTooltipStyles}
                    labelFormatter={(_, data) =>
                      formatWeeklyTooltipDate(
                        data[0]?.payload?.startDate,
                        data[0]?.payload?.endDate,
                      )
                    }
                    formatter={(value: number) => [
                      formatDurationMs(value * 1000),
                      durationSeriesLabel,
                    ]}
                  />
                  <Line
                    type="monotone"
                    dataKey="duration"
                    stroke={colors.blue}
                    name={durationSeriesLabel}
                  />
                </LineChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="text-xl">Session frequency</CardTitle>
            </CardHeader>
            <CardContent className="px-3 py-4">
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={frequencyChartData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="date" tickFormatter={formatWeeklyXAxisTick} />
                  <YAxis
                    allowDecimals={false}
                    domain={[0, (dataMax: number) => Math.max(3, Math.ceil(dataMax))]}
                  />
                  <Tooltip
                    {...rechartsTooltipStyles}
                    labelFormatter={(_, data) =>
                      formatWeeklyTooltipDate(
                        data[0]?.payload?.startDate,
                        data[0]?.payload?.endDate,
                      )
                    }
                  />
                  <Bar dataKey="workouts" fill={colors.violet} name="Sessions" />
                </BarChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <div className="flex flex-col items-center justify-center gap-2 sm:flex-row sm:items-start sm:justify-between">
                <CardTitle className="text-xl">Calories over time</CardTitle>
                <AggregationTypeDropDownMenu
                  aggregationType={search.chartAggregation as AggregationType}
                  setAggregationType={handleAggregationChange}
                  loading={isChartRefetching}
                />
              </div>
            </CardHeader>
            <CardContent className="px-3 py-4">
              {caloriesChartData.some((point) => point.calories > 0) ? (
                <ResponsiveContainer width="100%" height={300}>
                  <BarChart data={caloriesChartData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="date" tickFormatter={formatWeeklyXAxisTick} />
                    <YAxis />
                    <Tooltip
                      {...rechartsTooltipStyles}
                      labelFormatter={(_, data) =>
                        formatWeeklyTooltipDate(
                          data[0]?.payload?.startDate,
                          data[0]?.payload?.endDate,
                        )
                      }
                      formatter={(value: number) => [
                        isAverageAggregation
                          ? value.toFixed(1)
                          : Math.round(value).toLocaleString(),
                        caloriesSeriesLabel,
                      ]}
                    />
                    <Bar
                      dataKey="calories"
                      fill={colors.red}
                      name={caloriesSeriesLabel}
                    />
                  </BarChart>
                </ResponsiveContainer>
              ) : (
                <p className="py-8 text-center text-sm text-muted-foreground">
                  No calorie data recorded for sessions in this period.
                </p>
              )}
            </CardContent>
          </Card>

          {stats.recentSessions.length > 0 ? (
            <div className="space-y-3">
              <h2 className="text-lg font-semibold">Recent sessions</h2>
              <div className="space-y-3">
                {stats.recentSessions.map((workout, index) => (
                  <WorkoutHistoryCard
                    key={workout.id}
                    workout={workout}
                    muscleGroups={stats.muscleGroups}
                    isNewest={index === 0}
                    onViewSessionDetails={setSessionDetailWorkout}
                  />
                ))}
              </div>
            </div>
          ) : null}
        </>
      ) : null}

      {sessionDetailWorkout ? (
        <WorkoutSessionDetailsDialog
          workout={sessionDetailWorkout}
          onClose={() => setSessionDetailWorkout(null)}
        />
      ) : null}
    </PageCard>
  );
}

function StatsMetricCard({
  label,
  value,
  muted,
}: {
  label: string;
  value: string;
  muted?: boolean;
}) {
  return (
    <div className="rounded-lg border border-border/70 bg-secondary/30 p-4">
      <p className="text-xs text-muted-foreground">{label}</p>
      <div className="mt-1 flex items-center gap-2">
        <TrendingUp
          className={`h-4 w-4 ${muted ? "text-muted-foreground" : "text-primary"}`}
        />
        <p
          className={`text-lg font-semibold ${muted ? "text-muted-foreground" : ""}`}
        >
          {value}
        </p>
      </div>
    </div>
  );
}
