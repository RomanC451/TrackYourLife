import { useState } from "react";
import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from "recharts";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { colors } from "@/constants/tailwindColors";
import { MuscleGroupDto } from "@/services/openapi";

import { ChartLoadingOverlay } from "@/components/common/ChartLoadingOverlay";
import { useOverviewDateRange } from "../contexts/OverviewDateRangeContext";
import { muscleGroupDistributionQueryOptions } from "../queries/useMuscleGroupDistributionQuery";
import { muscleGroupsQueryOptions } from "@/features/trainings/exercises/queries/useMuscleGroupsQuery";

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

/** Non-empty value for "All main groups" (Radix Select does not allow empty string). */
const ALL_MAIN_GROUPS_VALUE = "__all__";

function MuscleGroupsChart() {
  const { startDate, endDate } = useOverviewDateRange();
  const [selectedMuscleGroup, setSelectedMuscleGroup] = useState<string>(
    ALL_MAIN_GROUPS_VALUE,
  );

  const { query: muscleGroupsQuery } = useCustomQuery(muscleGroupsQueryOptions.all);
  const mainGroups: MuscleGroupDto[] = muscleGroupsQuery.data ?? [];

  const muscleGroupFilter =
    selectedMuscleGroup === ALL_MAIN_GROUPS_VALUE ? null : selectedMuscleGroup;

  const { query: distributionQuery, isDelayedFetching } = useCustomQuery(
    muscleGroupDistributionQueryOptions.byDateRange(
      startDate,
      endDate,
      muscleGroupFilter,
    ),
  );

  const chartData =
    distributionQuery.data?.map((item) => ({
      name: item.muscleGroup,
      value: item.workoutCount,
      percentage: item.percentage,
    })) ?? [];

  const hasSubgroups = mainGroups.some(
    (g: MuscleGroupDto) => g.children && g.children.length > 0,
  );

  return (
    <Card>
      <CardHeader className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <CardTitle className="text-xl">Muscle Groups Distribution</CardTitle>
        {hasSubgroups && (
          <Select
            value={selectedMuscleGroup}
            onValueChange={setSelectedMuscleGroup}
          >
            <SelectTrigger className="w-full sm:w-[220px]">
              <SelectValue placeholder="View by group" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={ALL_MAIN_GROUPS_VALUE}>
                Main groups
              </SelectItem>
              {mainGroups
                .filter((g: MuscleGroupDto) => g.children && g.children.length > 0)
                .map((g: MuscleGroupDto) => (
                  <SelectItem key={g.id} value={g.name}>
                    {g.name}
                  </SelectItem>
                ))}
            </SelectContent>
          </Select>
        )}
      </CardHeader>
      <CardContent className="relative px-3 py-4">
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
          </PieChart>
        </ResponsiveContainer>
        <ChartLoadingOverlay show={isDelayedFetching} />
      </CardContent>
    </Card>
  );
}

export default MuscleGroupsChart;
