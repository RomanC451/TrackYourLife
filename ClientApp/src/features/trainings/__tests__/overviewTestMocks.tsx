import { vi } from "vitest";

export const mockUseCustomQuery = vi.fn();

export function setupOverviewTestMocks() {
  vi.mock("@/hooks/useCustomQuery", () => ({
    useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
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
    Legend: () => null,
    BarChart: ({ children }: { children: React.ReactNode }) => (
      <div data-testid="bar-chart">{children}</div>
    ),
    Bar: () => <div data-testid="bar" />,
    XAxis: () => null,
    YAxis: () => null,
    CartesianGrid: () => null,
    LineChart: ({ children }: { children: React.ReactNode }) => (
      <div data-testid="line-chart">{children}</div>
    ),
    Line: () => <div data-testid="line" />,
  }));

  vi.mock("@/components/common/ChartLoadingOverlay", () => ({
    ChartLoadingOverlay: ({ show }: { show: boolean }) =>
      show ? <div data-testid="chart-loading" /> : null,
  }));
}

export function mockOverviewQuery(data: unknown, isDelayedFetching = false) {
  mockUseCustomQuery.mockReturnValue({
    query: { data, isError: false, isPending: false },
    isDelayedFetching,
  });
}
