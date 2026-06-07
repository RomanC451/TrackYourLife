import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { OverviewDateRangeProvider } from "../../contexts/OverviewDateRangeContext";

const { mockUseCustomQuery } = vi.hoisted(() => ({
  mockUseCustomQuery: vi.fn(),
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

import WorkoutSummaryCards from "../WorkoutSummaryCards";

describe("WorkoutSummaryCards", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("renders overview metrics when data is available", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: {
          totalWorkoutsCompleted: 12,
          totalTrainingTimeSeconds: 3660,
          totalCaloriesBurned: 1800,
          hasActiveTraining: true,
        },
        isPending: false,
        isError: false,
      },
    });

    render(
      <OverviewDateRangeProvider>
        <WorkoutSummaryCards />
      </OverviewDateRangeProvider>,
    );

    expect(screen.getByText("Total Workouts")).toBeInTheDocument();
    expect(screen.getByText("12")).toBeInTheDocument();
    expect(screen.getByText("Calories Burned")).toBeInTheDocument();
    expect(screen.getByText("1,800")).toBeInTheDocument();
    expect(screen.getByText("Yes")).toBeInTheDocument();
  });

  it("renders skeleton cards while pending", () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: undefined,
        isPending: true,
        isError: false,
      },
    });

    const { container } = render(
      <OverviewDateRangeProvider>
        <WorkoutSummaryCards />
      </OverviewDateRangeProvider>,
    );

    expect(container.querySelectorAll(".animate-pulse").length).toBeGreaterThan(0);
  });
});
