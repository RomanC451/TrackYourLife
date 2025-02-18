import { format, isSameDay } from "date-fns";
import {
  CartesianGrid,
  Legend,
  LegendType,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import { CurveType } from "recharts/types/shape/Curve";

import { colors } from "@/constants/tailwindColors";

import { AggregationMode, OverviewType, ViewMode } from "../../data/types";

type NutritionSummaryChartData = {
  name: string;
  startDate: Date;
  endDate: Date;
  calories: number;
  carbs: number;
  proteins: number;
  fats: number;
  caloriesTarget: number;
  carbsTarget: number;
  proteinsTarget: number;
  fatsTarget: number;
}[];

type NutritionSummaryChartProps = {
  chartData: NutritionSummaryChartData;
  overviewType: OverviewType;
  aggregationMode: AggregationMode;
  viewMode: ViewMode;
};

function getLineChartProps(
  name: string,
  color: string,
  type: "value" | "target",
) {
  if (type === "value") {
    return {
      type: "monotone" as CurveType,
      dataKey: name,
      name: name.charAt(0).toUpperCase() + name.slice(1),
      stroke: color,
      dot: { r: 4, fill: color },
      activeDot: {
        r: 8,
        fill: color,
        stroke: "hsl(var(--background))",
      },
    };
  } else {
    return {
      type: "monotone" as CurveType,
      dataKey: name,
      stroke: color,
      strokeDasharray: "5 5",
      dot: { r: 4, fill: color },
      legendType: "none" as LegendType,
    };
  }
}

function NutritionSummaryChart({
  chartData,
  overviewType,
  aggregationMode,
  viewMode,
}: NutritionSummaryChartProps) {
  const formatTooltipDate = (startDate?: Date, endDate?: Date) => {
    if (!startDate || !endDate) return "";

    if (isSameDay(startDate, endDate)) {
      return format(startDate, "MMM dd, yyyy");
    }

    if (overviewType === "weekly") {
      return `${format(startDate, "EEE, MMM dd")} - ${format(endDate, "EEE, MMM dd, yyyy")}`;
    }

    return `${format(startDate, "MMM dd")} - ${format(endDate, "MMM dd, yyyy")}`;
  };

  const formatYAxisTick = (value: number) => {
    if (viewMode === "calories") {
      return overviewType === "daily"
        ? `${value}`
        : `${Math.round(value / 1000)}k`;
    }

    return `${value}g`;
  };

  return (
    <ResponsiveContainer width="100%" height="100%">
      <LineChart
        data={chartData}
        margin={{ top: 30, right: 30, left: 30, bottom: 70 }}
      >
        <CartesianGrid strokeDasharray="3 3" className="stroke-muted" />
        <XAxis
          dataKey="name"
          stroke="currentColor"
          className="text-sm font-medium"
          angle={-45}
          textAnchor="end"
          height={60}
        />
        <YAxis
          stroke="currentColor"
          className="text-sm font-medium"
          tickFormatter={formatYAxisTick}
        />
        <Tooltip
          contentStyle={{
            backgroundColor: "hsl(var(--background))",
            borderColor: "hsl(var(--border))",
            borderRadius: "var(--radius)",
          }}
          labelFormatter={(_, data) => {
            const item = data[0]?.payload;
            return formatTooltipDate(item?.startDate, item?.endDate);
          }}
          formatter={(value: number, name: string) => {
            name = name.replace("Target", " target").trim().toLowerCase();
            const nutrientType = name
              .replace("Target", "")
              .trim()
              .toLowerCase();

            return [
              `${value.toLocaleString()} ${nutrientType === "calories" ? "kcal" : "g"} ${aggregationMode === "average" || overviewType === "daily" ? "(avg/day)" : ""}`,
              name,
            ];
          }}
        />
        ;
        <Legend wrapperStyle={{ paddingTop: "20px" }} />
        {viewMode === "calories" ? (
          <>
            <Line {...getLineChartProps("calories", colors.blue, "value")} />
            <Line
              {...getLineChartProps("caloriesTarget", colors.blue, "target")}
            />
          </>
        ) : (
          <>
            <Line {...getLineChartProps("carbs", colors.green, "value")} />
            <Line
              {...getLineChartProps("carbsTarget", colors.green, "target")}
            />

            <Line {...getLineChartProps("proteins", colors.violet, "value")} />
            <Line
              {...getLineChartProps("proteinsTarget", colors.violet, "target")}
            />

            <Line {...getLineChartProps("fats", colors.yellow, "value")} />
            <Line
              {...getLineChartProps("fatsTarget", colors.yellow, "target")}
            />
          </>
        )}
      </LineChart>
    </ResponsiveContainer>
  );
}

export default NutritionSummaryChart;
