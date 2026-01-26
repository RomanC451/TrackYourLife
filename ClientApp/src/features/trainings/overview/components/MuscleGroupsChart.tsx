import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip, Legend } from "recharts";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { colors } from "@/constants/tailwindColors";

import { trainingsOverviewQueryOptions } from "../queries/useTrainingsOverviewQuery";

const COLORS = [
  colors.violet,
  colors.blue,
  colors.green,
  colors.yellow,
  colors.turquoise,
  colors.red,
  "#8884d8",
  "#82ca9d",
];

function MuscleGroupsChart() {
  const { query: overviewQuery } = useCustomQuery(
    trainingsOverviewQueryOptions.byDateRange(null, null),
  );

  const chartData =
    overviewQuery.data?.muscleGroupDistribution?.map((item) => ({
      name: item.muscleGroup,
      value: item.workoutCount,
      percentage: item.percentage,
    })) ?? [];

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-xl">Muscle Groups Distribution</CardTitle>
      </CardHeader>
      <CardContent className="px-3 py-4">
        <ResponsiveContainer width="100%" height={300}>
          <PieChart>
            <Pie
              data={chartData}
              cx="50%"
              cy="50%"
              labelLine={false}
              label={({ name, percentage }) =>
                `${name}: ${percentage.toFixed(1)}%`
              }
              outerRadius={80}
              fill="#8884d8"
              dataKey="value"
            >
              {chartData.map((entry, index) => (
                <Cell
                  key={entry.name}
                  fill={COLORS[index % COLORS.length]}
                />
              ))}
            </Pie>
            <Tooltip
              contentStyle={{
                backgroundColor: "hsl(var(--background))",
                borderColor: "hsl(var(--border))",
                borderRadius: "var(--radius)",
                color: "hsl(var(--foreground))",
              }}
              itemStyle={{
                color: "hsl(var(--foreground))",
              }}
              labelStyle={{
                color: "hsl(var(--foreground))",
              }}
            />
            <Legend />
          </PieChart>
        </ResponsiveContainer>
      </CardContent>
    </Card>
  );
}

export default MuscleGroupsChart;
