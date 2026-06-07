import { render, screen } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

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
  LineChart: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="line-chart">{children}</div>
  ),
  Line: () => <div data-testid="line" />,
  CartesianGrid: () => null,
  XAxis: () => null,
  YAxis: () => null,
  Tooltip: () => null,
  Legend: () => null,
}));

vi.mock("@/components/common/ChartLoadingOverlay", () => ({
  ChartLoadingOverlay: ({ show }: { show: boolean }) =>
    show ? <div data-testid="chart-loading" /> : null,
}));

import DurationChart from "../DurationChart";

describe("DurationChart", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: [{ date: "2026-06-01", value: 45, startDate: "2026-06-01", endDate: "2026-06-07" }],
        isFetching: false,
      },
      isDelayedFetching: false,
    });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("renders workout duration chart", () => {
    render(
      <OverviewDateRangeProvider>
        <DurationChart />
      </OverviewDateRangeProvider>,
    );

    expect(screen.getByText("Workout Duration")).toBeInTheDocument();
    expect(screen.getByTestId("line-chart")).toBeInTheDocument();
  });
});
