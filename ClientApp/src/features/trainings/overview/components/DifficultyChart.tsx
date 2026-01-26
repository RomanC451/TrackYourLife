import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip, Legend } from "recharts";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { colors } from "@/constants/tailwindColors";

import { trainingsOverviewQueryOptions } from "../queries/useTrainingsOverviewQuery";

const COLORS = {
  Easy: "#82ca9d",
  Medium: "#ffc658",
  Hard: "#ff7300",
};

function DifficultyChart() {
  const { query: overviewQuery } = useCustomQuery(
    trainingsOverviewQueryOptions.byDateRange(null, null),
  );

  const chartData =
    overviewQuery.data?.difficultyDistribution?.map((item) => ({
      name: item.difficulty,
      value: item.workoutCount,
      percentage: item.percentage,
    })) ?? [];

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-xl">Difficulty Distribution</CardTitle>
      </CardHeader>
      <CardContent>
        <ResponsiveContainer width="100%" height={300}>
          <PieChart>
            <Pie
              data={chartData}
              cx="50%"
              cy="50%"
              labelLine={false}
              label={({ percentage }) =>
                `${percentage.toFixed(1)}%`
              }
              outerRadius={80}
              fill="#8884d8"
              dataKey="value"
            >
              {chartData.map((entry) => (
                <Cell
                  key={`cell-${entry.name}`}
                  fill={COLORS[entry.name as keyof typeof COLORS] || colors.violet}
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

export default DifficultyChart;
