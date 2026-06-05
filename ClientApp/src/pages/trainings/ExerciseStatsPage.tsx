import { useEffect, useMemo, useState } from "react";
import { format } from "date-fns";
import type { DateRange } from "react-day-picker";
import { TrendingUp } from "lucide-react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
  Line,
  LineChart,
} from "recharts";

import { DateRangeSelector } from "@/components/common/DateRangeSelector";
import PageCard from "@/components/common/PageCard";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { colors } from "@/constants/tailwindColors";
import { cn } from "@/lib/utils";
import type {
  ExerciseStatsChartMetric,
  ExerciseStatsDto,
  ExerciseStatsSearch,
} from "@/features/trainings/exercises/queries/exerciseStatsQuery";
import { getDateOnly, parseDateOnly } from "@/lib/date";

/** Matches Recharts tooltips across trainings overview charts (dark/light). */
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

const CHART_METRIC_OPTIONS: Array<{
  value: ExerciseStatsChartMetric;
  /** Y-axis + chart tooltip (short, avoids SVG clip). */
  label: string;
  /** Metric dropdown only. */
  dropdownLabel: string;
}> = [
  { value: "Volume", label: "Volume", dropdownLabel: "Volume" },
  {
    value: "TotalWeight",
    label: "Total weight",
    dropdownLabel: "Total weight (exercise)",
  },
  {
    value: "MaxWeight",
    label: "Max weight",
    dropdownLabel: "Max weight (set)",
  },
  {
    value: "MinWeight",
    label: "Min weight",
    dropdownLabel: "Min weight (set)",
  },
  {
    value: "TotalReps",
    label: "Total reps",
    dropdownLabel: "Total reps (exercise)",
  },
  {
    value: "MaxReps",
    label: "Max reps",
    dropdownLabel: "Max reps (set)",
  },
  {
    value: "MinReps",
    label: "Min reps",
    dropdownLabel: "Min reps (set)",
  },
];

type ExerciseStatsPageProps = {
  stats: ExerciseStatsDto;
  search: ExerciseStatsSearch;
  onChartMetricChange: (metric: ExerciseStatsChartMetric) => void;
  onCustomDateRangeChange: (
    startDate: ReturnType<typeof getDateOnly>,
    endDate: ReturnType<typeof getDateOnly>,
  ) => void;
  onClearCustomDates: () => void;
  /** DateRangeSelector "All time" preset — backend uses full history when range is All and dates are omitted. */
  onAllTimeRangeSelect: () => void;
  className?: string;
  popoverContainer?: HTMLElement | null;
};

export default function ExerciseStatsPage({
  stats,
  search,
  onChartMetricChange,
  onCustomDateRangeChange,
  onClearCustomDates,
  onAllTimeRangeSelect,
  className,
  popoverContainer,
}: ExerciseStatsPageProps) {
  const showUnsupportedState = stats.isSupportedExerciseType === false;

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

  const improvementData = useMemo(
    () =>
      stats.improvementTrend.map((point) => ({
        ...point,
        label: format(new Date(point.date), "MMM d"),
      })),
    [stats.improvementTrend],
  );

  const consistencyData = useMemo(
    () =>
      stats.consistencyTrend.map((point) => ({
        ...point,
        label: format(new Date(point.weekStartDate), "MMM d"),
      })),
    [stats.consistencyTrend],
  );

  /** Use server-echoed metric so axis matches `improvementTrend` while a new metric is refetching. */
  const metricAxisLabel = useMemo(() => {
    const found = CHART_METRIC_OPTIONS.find((o) => o.value === stats.chartMetric);
    return found?.label ?? stats.chartMetric;
  }, [stats.chartMetric]);

  return (
    <PageCard className={cn("space-y-4", className)}>
      <div className="flex w-full min-w-0 justify-end">
        <DateRangeSelector
          selectedRange={pickerRange}
          handleRangeSelect={handleDateRangeSelect}
          popoverContainer={popoverContainer}
        />
      </div>

      <Card>
        <CardHeader className="gap-4 sm:flex sm:flex-row sm:items-start sm:justify-between sm:gap-6">
          <CardTitle className="min-w-0 flex-1 text-balance text-2xl font-bold leading-tight tracking-tight text-foreground sm:text-3xl">
            {stats.exerciseName}
          </CardTitle>
          <div className="shrink-0 text-sm text-muted-foreground sm:pt-1 sm:text-right">
            {stats.summary.completedSessionsInRange} completed
            {stats.summary.skippedSessionsInRange > 0
              ? ` · ${stats.summary.skippedSessionsInRange} skipped`
              : ""}
          </div>
        </CardHeader>
        <CardContent className="grid gap-3 md:grid-cols-3">
          <StatsMetricCard
            label="Improvement (first vs last session volume)"
            value={`${stats.summary.improvementDeltaPercent.toFixed(1)}%`}
            muted={!stats.hasEnoughData}
          />
          <StatsMetricCard
            label="Avg volume (in range)"
            value={stats.summary.averageVolumeInRange.toFixed(1)}
          />
          <StatsMetricCard
            label="Total volume (in range)"
            value={stats.summary.totalVolumeInRange.toFixed(1)}
          />
        </CardContent>
        {!stats.hasEnoughData && stats.isSupportedExerciseType ? (
          <CardContent className="pt-0 text-xs text-muted-foreground">
            Add at least two completed sessions in this period to compare first vs last
            volume.
          </CardContent>
        ) : null}
      </Card>

      {showUnsupportedState ? (
        <Card>
          <CardContent className="py-8 text-center text-sm text-muted-foreground">
            Stats are currently available only for weight + reps exercises.
          </CardContent>
        </Card>
      ) : null}

      {stats.isSupportedExerciseType && improvementData.length === 0 ? (
        <Card>
          <CardContent className="py-8 text-center text-sm text-muted-foreground">
            No exercise history found for this range.
          </CardContent>
        </Card>
      ) : null}

      {stats.isSupportedExerciseType && improvementData.length > 0 ? (
        <Card>
          <CardHeader className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <CardTitle className="text-lg">Improvement</CardTitle>
            <div className="flex items-center gap-2">
              <Label htmlFor="chart-metric" className="text-sm whitespace-nowrap">
                Metric
              </Label>
              <Select
                value={search.chartMetric}
                onValueChange={(v) =>
                  onChartMetricChange(v as ExerciseStatsChartMetric)
                }
              >
                <SelectTrigger id="chart-metric" className="w-[220px]">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {CHART_METRIC_OPTIONS.map((opt) => (
                    <SelectItem key={opt.value} value={opt.value}>
                      {opt.dropdownLabel}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </CardHeader>
          <CardContent className="h-[300px]">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={improvementData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="label" />
                <YAxis
                  domain={([dataMin, dataMax]) => {
                    if (dataMin === dataMax) {
                      const pad =
                        dataMin === 0 ? 1 : Math.max(Math.abs(dataMin) * 0.02, 1e-6);
                      return [dataMin - pad, dataMax + pad];
                    }
                    return [dataMin, dataMax];
                  }}
                  label={{
                    value: metricAxisLabel,
                    angle: -90,
                    position: "insideLeft",
                  }}
                />
                <Tooltip
                  {...rechartsTooltipStyles}
                  formatter={(value: number) =>
                    Number.isInteger(value) ? String(value) : value.toFixed(2)
                  }
                />
                <Line
                  type="monotone"
                  dataKey="value"
                  name={metricAxisLabel}
                  stroke={colors.violet}
                  strokeWidth={2}
                  dot={{ r: 3 }}
                />
              </LineChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      ) : null}

      {stats.isSupportedExerciseType && consistencyData.length > 0 ? (
        <Card>
          <CardHeader>
            <CardTitle className="text-lg">Sessions per week</CardTitle>
          </CardHeader>
          <CardContent className="h-[300px]">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={consistencyData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="label" />
                <YAxis allowDecimals={false} />
                <Tooltip {...rechartsTooltipStyles} />
                <Bar
                  dataKey="completedSessionsCount"
                  stackId="sessions"
                  fill={colors.violet}
                  name="Completed"
                />
                <Bar
                  dataKey="skippedSessionsCount"
                  stackId="sessions"
                  fill="hsl(var(--muted-foreground) / 0.45)"
                  name="Skipped"
                />
              </BarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
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
