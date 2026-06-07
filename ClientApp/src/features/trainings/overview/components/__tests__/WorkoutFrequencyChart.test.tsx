import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { OverviewDateRangeProvider } from "../../contexts/OverviewDateRangeContext";

const { mockUseCustomQuery } = vi.hoisted(() => ({
  mockUseCustomQuery: vi.fn(),
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

vi.mock("recharts", () => ({
  ResponsiveContainer: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="responsive-container">{children}</div>
  ),
  BarChart: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="bar-chart">{children}</div>
  ),
  Bar: () => <div data-testid="bar" />,
  CartesianGrid: () => null,
  XAxis: () => null,
  YAxis: () => null,
  Tooltip: () => null,
}));

vi.mock("@/components/common/ChartLoadingOverlay", () => ({
  ChartLoadingOverlay: ({ show }: { show: boolean }) =>
    show ? <div data-testid="chart-loading" /> : null,
}));

import WorkoutFrequencyChart from "../WorkoutFrequencyChart";

describe("WorkoutFrequencyChart", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: [
          {
            date: "2026-06-01",
            workoutCount: 2,
            startDate: "2026-06-01",
            endDate: "2026-06-07",
          },
        ],
        isFetching: false,
      },
      isDelayedFetching: false,
    });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("renders the workout frequency chart", () => {
    render(
      <OverviewDateRangeProvider>
        <WorkoutFrequencyChart />
      </OverviewDateRangeProvider>,
    );

    expect(screen.getByText("Workout Frequency")).toBeInTheDocument();
    expect(screen.getByTestId("bar-chart")).toBeInTheDocument();
  });
});
