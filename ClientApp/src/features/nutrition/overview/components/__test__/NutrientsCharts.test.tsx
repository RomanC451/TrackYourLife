import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import NutrientsCharts from "../NutrientsCharts";

const mockNavigate = vi.fn();
const mockUseCustomQuery = vi.fn();

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

vi.mock("../NutrientCard", () => ({
  NutrientCard: ({
    title,
    currentValue,
    targetValue,
    overviewType,
    isLoading,
  }: {
    title: string;
    currentValue: number;
    targetValue: number;
    overviewType: string;
    isLoading: boolean;
  }) => (
    <div data-testid={`nutrient-card-${title.toLowerCase()}`}>
      <span data-testid={`${title.toLowerCase()}-current`}>{currentValue}</span>
      <span data-testid={`${title.toLowerCase()}-target`}>{targetValue}</span>
      <span data-testid={`${title.toLowerCase()}-overview-type`}>
        {overviewType}
      </span>
      <span data-testid={`${title.toLowerCase()}-loading`}>
        {isLoading.toString()}
      </span>
    </div>
  ),
}));

const overviewData = {
  id: "overview-1",
  startDate: "2026-06-01",
  endDate: "2026-06-05",
  nutritionalContent: {
    energy: { value: 1500, unit: "kcal" },
    carbohydrates: 150,
    protein: 75,
    fat: 50,
  },
  caloriesGoal: 2000,
  carbohydratesGoal: 200,
  proteinGoal: 100,
  fatGoal: 70,
  isLoading: false,
  isDeleting: false,
};

describe("NutrientsCharts", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders loading state when data is undefined", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: undefined, isFetching: true },
    });

    render(<NutrientsCharts />);

    expect(screen.getByText("Loading...")).toBeInTheDocument();
  });

  it("renders nutrient cards when data is available", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [overviewData], isFetching: false },
    });

    render(<NutrientsCharts />);

    expect(screen.getByTestId("nutrient-card-calories")).toBeInTheDocument();
    expect(screen.getByTestId("calories-current")).toHaveTextContent("1500");
    expect(screen.getByTestId("calories-target")).toHaveTextContent("2000");
  });

  it("shows empty-state overlay when there is no diary data", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [], isFetching: false },
    });

    render(<NutrientsCharts />);

    expect(
      screen.getByText(/No nutrition diary entries/i),
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Log your food" })).toBeInTheDocument();
  });

  it("changes overview type when period buttons are clicked", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [overviewData], isFetching: false },
    });

    render(<NutrientsCharts />);

    expect(screen.getAllByTestId(/-overview-type$/)[0]).toHaveTextContent(
      "Daily",
    );

    fireEvent.click(screen.getByRole("button", { name: "This week" }));
    expect(screen.getAllByTestId(/-overview-type$/)[0]).toHaveTextContent(
      "Weekly",
    );

    fireEvent.click(screen.getByRole("button", { name: "This month" }));
    expect(screen.getAllByTestId(/-overview-type$/)[0]).toHaveTextContent(
      "Monthly",
    );
  });

  it("navigates to the diary when logging food from the empty state", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [], isFetching: false },
    });

    render(<NutrientsCharts />);

    fireEvent.click(screen.getByRole("button", { name: "Log your food" }));

    expect(mockNavigate).toHaveBeenCalledWith({ to: "/nutrition/diary" });
  });
});
