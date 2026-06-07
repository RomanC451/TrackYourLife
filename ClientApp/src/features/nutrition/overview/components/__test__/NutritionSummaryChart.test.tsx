import { render } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import NutritionSummaryChart from "../NutritionSummaryChart/NutritionSummaryChart";

type CapturedChartProps = {
  tickFormatter?: (value: number) => string;
  labelFormatter?: (label: string, payload: { payload?: { startDate?: Date; endDate?: Date } }[]) => string;
  formatter?: (value: number, name: string) => [string, string];
};

const captured: CapturedChartProps = {};

vi.mock("recharts", () => ({
  ResponsiveContainer: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="responsive-container">{children}</div>
  ),
  LineChart: ({
    children,
    data,
  }: {
    children: React.ReactNode;
    data: unknown[];
  }) => (
    <div data-testid="line-chart" data-count={data.length}>
      {children}
    </div>
  ),
  CartesianGrid: () => null,
  XAxis: () => null,
  YAxis: ({
    tickFormatter,
  }: {
    tickFormatter?: (value: number) => string;
  }) => {
    captured.tickFormatter = tickFormatter;
    return null;
  },
  Tooltip: ({
    labelFormatter,
    formatter,
  }: {
    labelFormatter?: CapturedChartProps["labelFormatter"];
    formatter?: CapturedChartProps["formatter"];
  }) => {
    captured.labelFormatter = labelFormatter;
    captured.formatter = formatter;
    return null;
  },
  Legend: () => null,
  Line: () => null,
}));

const chartData = [
  {
    name: "Mon",
    startDate: new Date("2026-06-01"),
    endDate: new Date("2026-06-01"),
    calories: 1800,
    carbs: 200,
    proteins: 120,
    fats: 60,
    caloriesTarget: 2000,
    carbsTarget: 220,
    proteinsTarget: 150,
    fatsTarget: 70,
  },
];

describe("NutritionSummaryChart", () => {
  it("renders the chart with provided data", () => {
    const { getByTestId } = render(
      <NutritionSummaryChart
        chartData={chartData}
        overviewType="Daily"
        aggregationMode="Sum"
        viewMode="calories"
      />,
    );

    expect(getByTestId("line-chart")).toHaveAttribute("data-count", "1");
  });

  it("formats single-day tooltip dates", () => {
    render(
      <NutritionSummaryChart
        chartData={chartData}
        overviewType="Daily"
        aggregationMode="Sum"
        viewMode="calories"
      />,
    );

    expect(
      captured.labelFormatter?.("", [{ payload: chartData[0] }]),
    ).toBe("Jun 01, 2026");
  });

  it("formats weekly tooltip date ranges", () => {
    render(
      <NutritionSummaryChart
        chartData={[
          {
            ...chartData[0],
            startDate: new Date("2026-06-01"),
            endDate: new Date("2026-06-07"),
          },
        ]}
        overviewType="Weekly"
        aggregationMode="Average"
        viewMode="nutrients"
      />,
    );

    expect(
      captured.labelFormatter?.("", [
        {
          payload: {
            startDate: new Date("2026-06-01"),
            endDate: new Date("2026-06-07"),
          },
        },
      ]),
    ).toBe("Mon, Jun 01 - Sun, Jun 07, 2026");
  });

  it("formats y-axis ticks for calories and macros", () => {
    const { rerender } = render(
      <NutritionSummaryChart
        chartData={chartData}
        overviewType="Daily"
        aggregationMode="Sum"
        viewMode="calories"
      />,
    );

    expect(captured.tickFormatter?.(1800)).toBe("1800");

    rerender(
      <NutritionSummaryChart
        chartData={chartData}
        overviewType="Weekly"
        aggregationMode="Average"
        viewMode="calories"
      />,
    );

    expect(captured.tickFormatter?.(1800)).toBe("2k");

    rerender(
      <NutritionSummaryChart
        chartData={chartData}
        overviewType="Daily"
        aggregationMode="Sum"
        viewMode="nutrients"
      />,
    );

    expect(captured.tickFormatter?.(120)).toBe("120g");
  });

  it("formats monthly tooltip date ranges", () => {
    render(
      <NutritionSummaryChart
        chartData={[
          {
            ...chartData[0],
            startDate: new Date("2026-06-01"),
            endDate: new Date("2026-06-30"),
          },
        ]}
        overviewType="Monthly"
        aggregationMode="Sum"
        viewMode="calories"
      />,
    );

    expect(
      captured.labelFormatter?.("", [
        {
          payload: {
            startDate: new Date("2026-06-01"),
            endDate: new Date("2026-06-30"),
          },
        },
      ]),
    ).toBe("Jun 01 - Jun 30, 2026");
  });

  it("formats tooltip values with units and average suffix", () => {
    render(
      <NutritionSummaryChart
        chartData={chartData}
        overviewType="Weekly"
        aggregationMode="Average"
        viewMode="nutrients"
      />,
    );

    expect(captured.formatter?.(150, "Proteins")).toEqual([
      "150 g (avg/day)",
      "proteins",
    ]);
    expect(captured.formatter?.(2000, "Calories")).toEqual([
      "2,000 kcal (avg/day)",
      "calories",
    ]);
  });
});
