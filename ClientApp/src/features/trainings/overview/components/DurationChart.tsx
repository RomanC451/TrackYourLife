import { useMemo } from "react";
import {
  CartesianGrid,
  Legend,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { formatDurationMs } from "@/lib/time";
import { colors } from "@/constants/tailwindColors";

import { workoutHistoryQueryOptions } from "../queries/useWorkoutHistoryQuery";
import { DateRangeSelector } from "@/components/common/DateRangeSelector";
import { useDateRange } from "../hooks/useDateRange";

function DurationChart() {
  const { selectedRange, setSelectedRange, startDate, endDate } = useDateRange();

  const { query: historyQuery } = useCustomQuery(
    workoutHistoryQueryOptions.byDateRange(
      startDate,
      endDate,
    ),
  );

  const chartData = useMemo(() => {
    if (!Array.isArray(historyQuery.data)) {
      return [];
    }
    return historyQuery.data
      .map((workout) => ({
        date: workout.finishedOnUtc,
        duration: workout.durationSeconds,
        durationFormatted: formatDurationMs(workout.durationSeconds * 1000),
      }))
      .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
  }, [historyQuery.data]);

  return (
    <Card>
      <CardHeader>
        <div className="flex flex-col items-center justify-center gap-2 sm:flex-row sm:justify-between">
          <CardTitle className="text-xl">Workout Duration</CardTitle>
          <DateRangeSelector
            handleRangeSelect={setSelectedRange}
            selectedRange={selectedRange}
          />
        </div>
      </CardHeader>
      <CardContent className="px-3 py-4">
        <ResponsiveContainer width="100%" height={300}>
          <LineChart data={chartData}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis
              dataKey="date"
              tickFormatter={(value) => new Date(value).toLocaleDateString()}
            />
            <YAxis
              tickFormatter={(value) => formatDurationMs(value * 1000)}
            />
            <Tooltip
              contentStyle={{
                backgroundColor: "hsl(var(--background))",
                borderColor: "hsl(var(--border))",
                borderRadius: "var(--radius)",
                color: "hsl(var(--foreground))",
              }}
              labelFormatter={(value) => new Date(value).toLocaleDateString()}
              formatter={(value: number) => {
                return [formatDurationMs(value * 1000), "Duration"];
              }}
            />
            <Legend />
            <Line
              type="monotone"
              dataKey="duration"
              stroke={colors.blue}
              name="Duration"
            />
          </LineChart>
        </ResponsiveContainer>
      </CardContent>
    </Card>
  );
}

export default DurationChart;
