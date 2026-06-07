import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";

import { NutritionSummary } from "../NutritionSummaryChart/NutritionSummary";

const mockUseCustomQuery = vi.fn();

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

vi.mock("@/components/common/DateRangeSelector", () => ({
  DateRangeSelector: ({
    handleRangeSelect,
  }: {
    handleRangeSelect: (range: { from: Date; to: Date }) => void;
  }) => (
    <button
      type="button"
      onClick={() =>
        handleRangeSelect({
          from: new Date("2026-06-01"),
          to: new Date("2026-06-07"),
        })
      }
    >
      Change range
    </button>
  ),
}));

vi.mock("../NutritionSummaryChart/NutritionSummaryChart", () => ({
  default: ({ chartData }: { chartData: unknown[] }) => (
    <div data-testid="nutrition-summary-chart" data-count={chartData.length} />
  ),
}));

const overviewData = {
  id: "overview-1",
  startDate: "2026-06-05",
  endDate: "2026-06-05",
  nutritionalContent: (() => {
    const nutrition = createEmptyNutritionalContent();
    nutrition.energy = { unit: "calories", value: 1800 };
    nutrition.carbohydrates = 200;
    nutrition.protein = 120;
    nutrition.fat = 60;
    return nutrition;
  })(),
  caloriesGoal: 2000,
  carbohydratesGoal: 220,
  proteinGoal: 150,
  fatGoal: 70,
  isLoading: false,
  isDeleting: false,
};

describe("NutritionSummary", () => {
  it("renders chart data from the overview query", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [overviewData], isFetching: false },
    });

    render(<NutritionSummary />);

    expect(screen.getByText("Nutrition Summary")).toBeInTheDocument();
    expect(screen.getByTestId("nutrition-summary-chart")).toHaveAttribute(
      "data-count",
      "1",
    );
  });

  it("shows an empty-state overlay when there is no diary data", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [], isFetching: false },
    });

    render(<NutritionSummary />);

    expect(
      screen.getByText("No diary entries for selected period"),
    ).toBeInTheDocument();
    expect(screen.getByTestId("nutrition-summary-chart")).toHaveAttribute(
      "data-count",
      "7",
    );
  });

  it("shows a loading overlay while fetching", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [overviewData], isFetching: true },
    });

    const { container } = render(<NutritionSummary />);

    expect(container.querySelector(".fill-primary")).toBeTruthy();
  });

  it("requests daily overview data on initial render", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [overviewData], isFetching: false },
    });

    render(<NutritionSummary />);

    const initialCall = mockUseCustomQuery.mock.calls[0]?.[0];
    expect(initialCall.queryKey).toContain("Daily");
    expect(initialCall.queryKey).toContain("Average");
  });
});
