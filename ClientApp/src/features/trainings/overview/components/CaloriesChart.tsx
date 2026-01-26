import { useMemo } from "react";
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

import { workoutHistoryQueryOptions } from "../queries/useWorkoutHistoryQuery";
import { DateRangeSelector } from "@/components/common/DateRangeSelector";
import { useDateRange } from "../hooks/useDateRange";

function CaloriesChart() {
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
      .filter((workout) => workout.caloriesBurned != null)
      .map((workout) => ({
        date: workout.finishedOnUtc,
        calories: workout.caloriesBurned,
      }))
      .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
  }, [historyQuery.data]);

  return (
    <Card>
      <CardHeader>
        <div className="flex flex-col items-center justify-center gap-2 sm:flex-row sm:justify-between">
          <CardTitle className="text-xl">Calories Burned</CardTitle>
          <DateRangeSelector
            handleRangeSelect={setSelectedRange}
            selectedRange={selectedRange}
          />
        </div>
      </CardHeader>
      <CardContent className="px-3 py-4">
        <ResponsiveContainer width="100%" height={300}>
          <BarChart data={chartData}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis
              dataKey="date"
              tickFormatter={(value) => new Date(value).toLocaleDateString()}
            />
            <YAxis />
            <Tooltip
              contentStyle={{
                backgroundColor: "hsl(var(--background))",
                borderColor: "hsl(var(--border))",
                borderRadius: "var(--radius)",
                color: "hsl(var(--foreground))",
              }}
              labelFormatter={(value) => new Date(value).toLocaleDateString()}
            />
            <Legend />
            <Bar dataKey="calories" fill={colors.red} name="Calories" />
          </BarChart>
        </ResponsiveContainer>
      </CardContent>
    </Card>
  );
}

export default CaloriesChart;
