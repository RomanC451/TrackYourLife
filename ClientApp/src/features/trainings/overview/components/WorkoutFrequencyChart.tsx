import { useState } from "react";
import { format, isSameDay } from "date-fns";
import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { colors } from "@/constants/tailwindColors";
import { OverviewType } from "@/services/openapi";

import { workoutFrequencyQueryOptions } from "../queries/useWorkoutFrequencyQuery";
import { ChartLoadingOverlay } from "@/components/common/ChartLoadingOverlay";
import WorkoutFrequencyOverviewTypeDropDownMenu from "./WorkoutFrequencyOverviewTypeDropDownMenu";
import { useOverviewDateRange } from "../contexts/OverviewDateRangeContext";

function WorkoutFrequencyChart() {
  const [overviewType, setOverviewType] = useState<OverviewType>("Weekly");
  const { startDate, endDate } = useOverviewDateRange();

  const { query: frequencyQuery, isDelayedFetching } = useCustomQuery(
    workoutFrequencyQueryOptions.byFilters(startDate, endDate, overviewType),
  );

  const chartData =
    frequencyQuery.data?.map((item) =>
    ({
      date: item.date,
      workouts: item.workoutCount,
      startDate: item.startDate
        ? new Date(item.startDate)
        : new Date(item.date),
      endDate: item.endDate
        ? new Date(item.endDate)
        : new Date(item.date),
    })
    ) ?? [];

  const formatTooltipDate = (startDate?: Date, endDate?: Date) => {
    if (!startDate || !endDate) return "";

    if (isSameDay(startDate, endDate)) {
      return format(startDate, "MMM dd, yyyy");
    }

    if (overviewType === "Weekly") {
      return `${format(startDate, "EEE, MMM dd")} - ${format(endDate, "EEE, MMM dd, yyyy")}`;
    }

    return `${format(startDate, "MMM dd")} - ${format(endDate, "MMM dd, yyyy")}`;
  };

  const formatXAxisTick = (value: string) => {
    const date = new Date(value);
    if (overviewType === "Weekly") {
      return format(date, "MMM dd");
    } else {
      return format(date, "MMM yyyy");
    }
  };

  return (
    <Card>
      <CardHeader>
        <div className="flex flex-col items-center justify-center gap-2 sm:flex-row sm:justify-between sm:items-start">
          <CardTitle className="text-xl">Workout Frequency</CardTitle>
          <div className="flex flex-col sm:items-end gap-2 lg:flex-row lg:items-center items-center">
            <WorkoutFrequencyOverviewTypeDropDownMenu
              overviewType={overviewType}
              setOverviewType={setOverviewType}
              loading={frequencyQuery.isFetching}
            />
          </div>
        </div>

      </CardHeader>
      <CardContent className="relative px-3 py-4">
        <ResponsiveContainer width="100%" height={300}>
          <BarChart data={chartData} margin={{ top: 0, right: 0, left: -30, bottom: 0 }}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis
              dataKey="date"
              tickFormatter={formatXAxisTick}
            />
            <YAxis
              domain={[0, (dataMax: number) => Math.max(3, Math.ceil(dataMax))]}
              allowDecimals={false}
            />
            <Tooltip
              contentStyle={{
                backgroundColor: "hsl(var(--background))",
                borderColor: "hsl(var(--border))",
                borderRadius: "var(--radius)",
                color: "hsl(var(--foreground))",
              }}
              labelFormatter={(_, data) => formatTooltipDate(data[0]?.payload?.startDate, data[0]?.payload?.endDate)
              }
            />
            <Bar
              dataKey="workouts"
              fill={colors.violet}
              name="Workouts"
            />
          </BarChart>
        </ResponsiveContainer>
        <ChartLoadingOverlay show={isDelayedFetching} />
      </CardContent>
    </Card>
  );
}

export default WorkoutFrequencyChart;
