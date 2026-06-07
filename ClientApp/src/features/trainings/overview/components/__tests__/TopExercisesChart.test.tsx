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
  BarChart: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="bar-chart">{children}</div>
  ),
  Bar: () => <div data-testid="bar" />,
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


import TopExercisesChart from "../TopExercisesChart";

describe("TopExercisesChart", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
    mockUseCustomQuery.mockImplementation((opts: { queryKey?: unknown[] }) => {
      const page = Array.isArray(opts?.queryKey) ? opts.queryKey[4] : 1;
      return {
        query: {
          data: {
            items: [
              {
                exerciseName: "Bench press",
                completionCount: 10,
                skipCount: 1,
                improvementPercentage: 5,
              },
            ],
            hasNextPage: page === 1,
            hasPreviousPage: page !== 1,
            page: page as number,
            maxPage: 2,
          },
          isFetching: false,
        },
        isDelayedFetching: false,
      };
    });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("renders top exercises chart with pagination", () => {
    render(
      <OverviewDateRangeProvider>
        <TopExercisesChart />
      </OverviewDateRangeProvider>,
    );

    expect(screen.getByText("Top Exercises")).toBeInTheDocument();
    expect(screen.getByTestId("bar-chart")).toBeInTheDocument();
    expect(screen.getByText("1")).toBeInTheDocument();
    expect(screen.getByText("2")).toBeInTheDocument();
  });
});
