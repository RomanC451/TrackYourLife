import { useMemo } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { colors } from "@/constants/tailwindColors";

import { ChartLoadingOverlay } from "@/components/common/ChartLoadingOverlay";
import { trainingTemplatesUsageQueryOptions } from "../queries/useTrainingTemplatesUsageQuery";
import { useOverviewDateRange } from "../contexts/OverviewDateRangeContext";

function TrainingTemplatesChart() {
  const { startDate, endDate } = useOverviewDateRange();

  const { query: templatesQuery, isDelayedFetching } = useCustomQuery(
    trainingTemplatesUsageQueryOptions.byDateRange(startDate, endDate),
  );

  const chartData = useMemo(() => {
    if (!Array.isArray(templatesQuery.data)) {
      return [];
    }
    return templatesQuery.data.map((template) => ({
      name: template.trainingName,
      fullyCompleted: template.totalFullyCompleted,
      withSkippedExercises: template.totalWithSkippedExercises,
      completionRate: template.completionRate,
    }));
  }, [templatesQuery.data]);

  return (
    <Card>
      <CardHeader>
        <div className="flex flex-col items-center justify-center gap-2 sm:flex-row sm:justify-between">
          <CardTitle className="text-xl">Trainings Completion Rate</CardTitle>
        </div>
      </CardHeader>
      <CardContent className="relative px-3 py-4">
        <ResponsiveContainer width="100%" height={400}>
          <BarChart data={chartData}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis
              dataKey="name"
              angle={-45}
              textAnchor="end"
              height={100}
              tick={{ fontSize: 12 }}
            />
            <YAxis />
            <Tooltip
              contentStyle={{
                backgroundColor: "hsl(var(--background))",
                borderColor: "hsl(var(--border))",
                borderRadius: "var(--radius)",
                color: "hsl(var(--foreground))",
              }}
            />
            <Legend />
            <Bar
              dataKey="fullyCompleted"
              fill={colors.green}
              name="Fully completed"
            />
            <Bar
              dataKey="withSkippedExercises"
              fill={colors.blue}
              name="With skipped exercises"
            />
          </BarChart>
        </ResponsiveContainer>
        <ChartLoadingOverlay show={isDelayedFetching} />
      </CardContent>
    </Card>
  );
}

export default TrainingTemplatesChart;
