import { useState } from "react";
import { format, isSameDay } from "date-fns";
import {
  CartesianGrid,
  Legend,
  Bar,
  BarChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { colors } from "@/constants/tailwindColors";
import { AggregationType, OverviewType, OverviewType2 } from "@/services/openapi";

import { ChartLoadingOverlay } from "@/components/common/ChartLoadingOverlay";
import WorkoutFrequencyOverviewTypeDropDownMenu from "./WorkoutFrequencyOverviewTypeDropDownMenu";
import AggregationTypeDropDownMenu from "./AggregationTypeDropDownMenu";
import { caloriesBurnedHistoryQueryOptions } from "../queries/useCaloriesBurnedHistoryQuery";
import { useOverviewDateRange } from "../contexts/OverviewDateRangeContext";

function CaloriesChart() {
  const [overviewType, setOverviewType] = useState<OverviewType>("Weekly");
  const [aggregationType, setAggregationType] =
    useState<AggregationType>("Sum");
  const { startDate, endDate } = useOverviewDateRange();

  const { query: caloriesQuery, isDelayedFetching } = useCustomQuery(
    caloriesBurnedHistoryQueryOptions.byFilters(
      startDate,
      endDate,
      overviewType as OverviewType2,
      aggregationType,
    ),
  );

  const chartData =
    caloriesQuery.data?.map((item) => ({
      date: item.date,
      calories: item.value,
      startDate: item.startDate
        ? new Date(item.startDate)
        : new Date(item.date),
      endDate: item.endDate ? new Date(item.endDate) : new Date(item.date),
    })) ?? [];

  const formatTooltipDate = (startDate?: Date, endDate?: Date) => {
    if (!startDate || !endDate) return "";
    if (isSameDay(startDate, endDate)) {
      return format(startDate, "MMM dd, yyyy");
    }
    if (overviewType === "Daily" || overviewType === "Weekly") {
      return `${format(startDate, "EEE, MMM dd")} - ${format(endDate, "EEE, MMM dd, yyyy")}`;
    }
    return `${format(startDate, "MMM dd")} - ${format(endDate, "MMM dd, yyyy")}`;
  };

  const formatXAxisTick = (value: string) => {
    const date = new Date(value);
    if (overviewType === "Daily") {
      return format(date, "MMM dd");
    }
    if (overviewType === "Weekly") {
      return format(date, "MMM dd");
    }
    return format(date, "MMM yyyy");
  };

  return (
    <Card>
      <CardHeader>
        <div className="flex flex-col items-center justify-center gap-2 sm:flex-row sm:justify-between sm:items-start">
          <CardTitle className="text-xl">Calories Burned</CardTitle>
          <div className="flex gap-2 flex-row items-center">
            <WorkoutFrequencyOverviewTypeDropDownMenu
              overviewType={overviewType}
              setOverviewType={setOverviewType}
              loading={caloriesQuery.isFetching}
            />
            <AggregationTypeDropDownMenu
              aggregationType={aggregationType}
              setAggregationType={setAggregationType}
              disabled={overviewType === "Daily"}
              loading={caloriesQuery.isFetching}
            />
          </div>
        </div>
      </CardHeader>
      <CardContent className="relative px-3 py-4">
        <ResponsiveContainer width="100%" height={300}>
          <BarChart data={chartData}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis
              dataKey="date"
              tickFormatter={formatXAxisTick}
            />
            <YAxis />
            <Tooltip
              contentStyle={{
                backgroundColor: "hsl(var(--background))",
                borderColor: "hsl(var(--border))",
                borderRadius: "var(--radius)",
                color: "hsl(var(--foreground))",
              }}
              labelFormatter={(_, data) =>
                formatTooltipDate(data[0]?.payload?.startDate, data[0]?.payload?.endDate)
              }
            />
            <Legend />
            <Bar dataKey="calories" fill={colors.red} name="Calories" />
          </BarChart>
        </ResponsiveContainer>
        <ChartLoadingOverlay show={isDelayedFetching} />
      </CardContent>
    </Card>
  );
}

export default CaloriesChart;
