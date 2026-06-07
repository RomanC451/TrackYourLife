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
  PieChart: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="pie-chart">{children}</div>
  ),
  Pie: () => <div data-testid="pie" />,
  Cell: () => null,
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

import MuscleGroupsChart from "../MuscleGroupsChart";

describe("MuscleGroupsChart", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
    mockUseCustomQuery.mockImplementation((opts: { queryKey?: unknown[] }) => {
      const key = opts?.queryKey?.[0];
      if (key === "muscleGroups") {
        return { query: { data: [{ id: "1", name: "Chest", subgroups: [] }] } };
      }
      return {
        query: {
          data: [{ muscleGroup: "Chest", workoutCount: 5, percentage: 50 }],
        },
        isDelayedFetching: false,
      };
    });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("renders muscle group distribution chart for pro users", () => {
    render(
      <OverviewDateRangeProvider>
        <MuscleGroupsChart />
      </OverviewDateRangeProvider>,
    );

    expect(screen.getByText("Muscle Groups Distribution")).toBeInTheDocument();
    expect(screen.getByTestId("pie-chart")).toBeInTheDocument();
  });
});
