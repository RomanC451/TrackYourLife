import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip, Legend } from "recharts";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { colors } from "@/constants/tailwindColors";

import { ChartLoadingOverlay } from "@/components/common/ChartLoadingOverlay";
import { useOverviewDateRange } from "../contexts/OverviewDateRangeContext";
import { difficultyDistributionQueryOptions } from "../queries/useDifficultyDistributionQuery";

const COLORS = {
  Easy: "#82ca9d",
  Medium: "#ffc658",
  Hard: "#ff7300",
};

function DifficultyChart() {
  const { startDate, endDate } = useOverviewDateRange();
  const { query: distributionQuery, isDelayedFetching } = useCustomQuery(
    difficultyDistributionQueryOptions.byDateRange(startDate, endDate),
  );

  const chartData =
    distributionQuery.data?.map((item) => ({
      name: item.difficulty,
      value: item.workoutCount,
      percentage: item.percentage,
    })) ?? [];

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-xl">Difficulty Distribution</CardTitle>
      </CardHeader>
      <CardContent className="relative">
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
        <ChartLoadingOverlay show={isDelayedFetching} />
      </CardContent>
    </Card>
  );
}

export default DifficultyChart;
