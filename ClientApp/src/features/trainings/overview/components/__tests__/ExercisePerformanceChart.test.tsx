import { render, screen } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { OverviewDateRangeProvider } from "../../contexts/OverviewDateRangeContext";

const { mockUseCustomQuery } = vi.hoisted(() => ({
  mockUseCustomQuery: vi.fn(),
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

vi.mock("@/contexts/AuthenticationContextProvider", () => ({
  useAuthenticationContext: () => ({ isPro: true }),
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

vi.mock("@/components/common/ChartProFeatureOverlay", () => ({
  ChartProFeatureOverlay: ({ show }: { show: boolean }) =>
    show ? <div data-testid="pro-overlay" /> : null,
}));

vi.mock("@/components/common/PaginationButtons", () => ({
  PaginationButtons: () => null,
}));

vi.mock("../CalculationMethodSelector", () => ({
  CalculationMethodSelector: () => <div data-testid="calc-method" />,
}));

vi.mock("../ExerciseHistoriesDialog", () => ({
  default: () => null,
}));

import ExercisePerformanceChart from "../ExercisePerformanceChart";

describe("ExercisePerformanceChart", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: {
          items: [
            {
              exerciseName: "Squat",
              averageImprovement: 8.5,
              exerciseId: "ex-1",
            },
          ],
          hasNextPage: false,
          hasPreviousPage: false,
          pageNumber: 1,
          totalPages: 1,
        },
        isFetching: false,
      },
      isDelayedFetching: false,
    });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("renders exercise performance chart for pro users", () => {
    render(
      <OverviewDateRangeProvider>
        <ExercisePerformanceChart />
      </OverviewDateRangeProvider>,
    );

    expect(screen.getByText("Exercises Performance")).toBeInTheDocument();
    expect(screen.getByTestId("bar-chart")).toBeInTheDocument();
    expect(screen.getByTestId("calc-method")).toBeInTheDocument();
  });
});
