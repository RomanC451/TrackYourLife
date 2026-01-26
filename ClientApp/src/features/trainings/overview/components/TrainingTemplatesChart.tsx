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

import { trainingTemplatesUsageQueryOptions } from "../queries/useTrainingTemplatesUsageQuery";

function TrainingTemplatesChart() {
  const { query: templatesQuery } = useCustomQuery(
    trainingTemplatesUsageQueryOptions.all,
  );

  const chartData = useMemo(() => {
    if (!Array.isArray(templatesQuery.data)) {
      return [];
    }
    return templatesQuery.data.map((template) => ({
      name: template.trainingName,
      started: template.totalStarted,
      completed: template.totalCompleted,
      completionRate: template.completionRate,
    }));
  }, [templatesQuery.data]);

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-xl">Training Templates Usage</CardTitle>
      </CardHeader>
      <CardContent className="px-3 py-4">
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
            <Bar dataKey="started" fill={colors.blue} name="Started" />
            <Bar dataKey="completed" fill={colors.green} name="Completed" />
          </BarChart>
        </ResponsiveContainer>
      </CardContent>
    </Card>
  );
}

export default TrainingTemplatesChart;
