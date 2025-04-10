import { faker } from "@faker-js/faker";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import * as getDateOnlyModule from "@/lib/date";
import { DailyNutritionOverviewDto } from "@/services/openapi";

import NutrientsCharts from "../../components/NutrientsCharts";
import * as useDailyNutritionOverviewsQueryModule from "../../queries/useDailyNutritionOverviewsQuery";
import * as nutritionCalculationsModule from "../../utils/nutritionCalculations";

// Mock dependencies
vi.mock("@/lib/date", () => ({
  getDateOnly: vi.fn(),
}));

vi.mock("../../queries/useDailyNutritionOverviewsQuery", () => ({
  __esModule: true,
  default: vi.fn(),
}));

vi.mock("../../utils/nutritionCalculations", () => ({
  calculateNutritionSummary: vi.fn(),
}));

vi.mock("../../components/NutrientCard", () => ({
  NutrientCard: ({
    title,
    current,
    target,
    unit,
    overviewType,
    isLoading,
  }: {
    title: string;
    current: number;
    target: number;
    unit: string;
    overviewType: string;
    isLoading: boolean;
  }) => (
    <div data-testid={`nutrient-card-${title.toLowerCase()}`}>
      <div data-testid={`${title.toLowerCase()}-current`}>{current}</div>
      <div data-testid={`${title.toLowerCase()}-target`}>{target}</div>
      <div data-testid={`${title.toLowerCase()}-unit`}>{unit}</div>
      <div data-testid={`${title.toLowerCase()}-overview-type`}>
        {overviewType}
      </div>
      <div data-testid={`${title.toLowerCase()}-loading`}>
        {isLoading.toString()}
      </div>
    </div>
  ),
}));

const generateMockData = (
  startDate: string,
  endDate: string,
): DailyNutritionOverviewDto[] => {
  const data: DailyNutritionOverviewDto[] = [];
  const currentDate = new Date(startDate);

  while (currentDate <= new Date(endDate)) {
    data.push({
      id: faker.string.uuid(),
      date: currentDate.toISOString().split("T")[0],
      nutritionalContent: {
        energy: {
          value: faker.number.int({ min: 1400, max: 2000 }),
          unit: "kcal",
        },
        carbohydrates: faker.number.int({ min: 190, max: 250 }),
        protein: faker.number.int({ min: 70, max: 100 }),
        fat: faker.number.int({ min: 40, max: 70 }),
        calcium: faker.number.int({ min: 100, max: 200 }),
        cholesterol: faker.number.int({ min: 50, max: 100 }),
        fiber: faker.number.int({ min: 10, max: 30 }),
        iron: faker.number.int({ min: 5, max: 15 }),
        potassium: faker.number.int({ min: 200, max: 400 }),
        sodium: faker.number.int({ min: 100, max: 300 }),
        sugar: faker.number.int({ min: 20, max: 50 }),
        vitaminA: faker.number.int({ min: 500, max: 1000 }),
        vitaminC: faker.number.int({ min: 50, max: 100 }),
        monounsaturatedFat: faker.number.int({ min: 50, max: 100 }),
        netCarbs: faker.number.int({ min: 50, max: 100 }),
        polyunsaturatedFat: faker.number.int({ min: 50, max: 100 }),
        saturatedFat: faker.number.int({ min: 50, max: 100 }),
        transFat: faker.number.int({ min: 50, max: 100 }),
      },
      caloriesGoal: faker.number.int({ min: 1900, max: 2200 }),
      carbohydratesGoal: faker.number.int({ min: 240, max: 300 }),
      proteinGoal: faker.number.int({ min: 90, max: 120 }),
      fatGoal: faker.number.int({ min: 50, max: 80 }),
    });
    currentDate.setDate(currentDate.getDate() + 1);
  }

  return data;
};

describe("NutrientsCharts", () => {
  // Define proper types for the mock data and return values

  const mockQueryImplementation = (
    data: DailyNutritionOverviewDto[] = [],
    isLoading = false,
  ) => {
    const mockQuery = vi.fn().mockReturnValue({
      dailyNutritionOverviewsQuery: {
        data,
      },
      isPending: {
        isLoading,
      },
    });

    (
      useDailyNutritionOverviewsQueryModule.default as ReturnType<typeof vi.fn>
    ).mockImplementation(mockQuery);

    return mockQuery;
  };

  const mockCalculateNutritionSummary = (
    returnValue = {
      calories: { value: 0, target: 0 },
      carbs: { value: 0, target: 0 },
      proteins: { value: 0, target: 0 },
      fats: { value: 0, target: 0 },
    },
  ) => {
    const mockCalculation = vi.fn().mockReturnValue(returnValue);
    (
      nutritionCalculationsModule.calculateNutritionSummary as ReturnType<
        typeof vi.fn
      >
    ).mockImplementation(mockCalculation);

    return mockCalculation;
  };

  const mockGetDateOnly = (dates: getDateOnlyModule.DateOnly[]) => {
    const mockGetDate = vi.fn();

    for (const date of dates) {
      mockGetDate.mockReturnValueOnce(date);
    }

    (
      getDateOnlyModule.getDateOnly as ReturnType<typeof vi.fn>
    ).mockImplementation(mockGetDate);

    return mockGetDate;
  };

  beforeEach(() => {
    vi.clearAllMocks();

    mockQueryImplementation();
    mockCalculateNutritionSummary();
  });

  it("renders Empty component when no data is available", () => {
    // Setup empty data
    mockQueryImplementation([]);

    render(<NutrientsCharts />);

    expect(screen.getByText("No data")).toBeInTheDocument();
    expect(screen.queryByText("Today")).not.toBeInTheDocument();
  });

  it("renders nutrient cards when data is available", () => {
    // Setup data and mock calculation return
    mockQueryImplementation(generateMockData("2023-01-01", "2023-01-01"));
    mockCalculateNutritionSummary({
      calories: { value: 1500, target: 2000 },
      carbs: { value: 150, target: 200 },
      proteins: { value: 75, target: 100 },
      fats: { value: 50, target: 70 },
    });

    render(<NutrientsCharts />);

    // Check all nutrient cards are rendered
    expect(screen.getByTestId("nutrient-card-calories")).toBeInTheDocument();
    expect(screen.getByTestId("nutrient-card-carbs")).toBeInTheDocument();
    expect(screen.getByTestId("nutrient-card-proteins")).toBeInTheDocument();
    expect(screen.getByTestId("nutrient-card-fats")).toBeInTheDocument();

    // Verify values are passed correctly to each card
    expect(screen.getByTestId("calories-current")).toHaveTextContent("1500");
    expect(screen.getByTestId("calories-target")).toHaveTextContent("2000");
    expect(screen.getByTestId("carbs-current")).toHaveTextContent("150");
    expect(screen.getByTestId("carbs-target")).toHaveTextContent("200");
    expect(screen.getByTestId("proteins-current")).toHaveTextContent("75");
    expect(screen.getByTestId("proteins-target")).toHaveTextContent("100");
    expect(screen.getByTestId("fats-current")).toHaveTextContent("50");
    expect(screen.getByTestId("fats-target")).toHaveTextContent("70");
  });

  it("passes the correct loading state to nutrient cards", () => {
    // Setup loading state
    mockQueryImplementation(generateMockData("2023-01-01", "2023-01-01"), true);

    render(<NutrientsCharts />);

    // Verify loading state is passed to all cards
    expect(screen.getByTestId("calories-loading")).toHaveTextContent("true");
    expect(screen.getByTestId("carbs-loading")).toHaveTextContent("true");
    expect(screen.getByTestId("proteins-loading")).toHaveTextContent("true");
    expect(screen.getByTestId("fats-loading")).toHaveTextContent("true");
  });

  it("passes the correct units to nutrient cards", () => {
    mockQueryImplementation(generateMockData("2023-01-01", "2023-01-01"));

    render(<NutrientsCharts />);

    // Verify units are correctly passed
    expect(screen.getByTestId("calories-unit")).toHaveTextContent("kcal");
    expect(screen.getByTestId("carbs-unit")).toHaveTextContent("g");
    expect(screen.getByTestId("proteins-unit")).toHaveTextContent("g");
    expect(screen.getByTestId("fats-unit")).toHaveTextContent("g");
  });

  it("changes overview type when buttons are clicked", () => {
    // Setup query and calculation mocks
    mockQueryImplementation(generateMockData("2023-01-01", "2023-01-01"));
    const mockCalculation = mockCalculateNutritionSummary();

    render(<NutrientsCharts />);

    // Check default overview type
    expect(mockCalculation).toHaveBeenCalledWith(
      expect.anything(),
      "day",
      expect.anything(),
    );
    expect(screen.getAllByTestId(/-overview-type$/)).toEqual(
      expect.arrayContaining([expect.objectContaining({ textContent: "day" })]),
    );

    // Click week button and verify changes
    fireEvent.click(screen.getByText("This week"));
    expect(mockCalculation).toHaveBeenCalledWith(
      expect.anything(),
      "week",
      expect.anything(),
    );
    expect(screen.getAllByTestId(/-overview-type$/)).toEqual(
      expect.arrayContaining([
        expect.objectContaining({ textContent: "week" }),
      ]),
    );

    // Click month button and verify changes
    fireEvent.click(screen.getByText("This month"));
    expect(mockCalculation).toHaveBeenCalledWith(
      expect.anything(),
      "month",
      expect.anything(),
    );
    expect(screen.getAllByTestId(/-overview-type$/)).toEqual(
      expect.arrayContaining([
        expect.objectContaining({ textContent: "month" }),
      ]),
    );

    // Click day button again
    fireEvent.click(screen.getByText("Today"));
    expect(mockCalculation).toHaveBeenCalledWith(
      expect.anything(),
      "day",
      expect.anything(),
    );
  });

  it("calls query hook with correct date parameters", () => {
    const mockQuery = mockQueryImplementation();

    // Setup date-fns mock return values for testing dates
    const mockStartDate = "2023-01-01";
    const mockEndDate = "2023-12-31";

    const getDateOnlyMock = mockGetDateOnly([mockStartDate, mockEndDate]);

    render(<NutrientsCharts />);

    // Verify the query was called with correct date range
    expect(mockQuery).toHaveBeenCalledWith(mockStartDate, mockEndDate);
    expect(getDateOnlyMock).toHaveBeenCalledWith(expect.any(Date)); // startOfYear
    expect(getDateOnlyMock).toHaveBeenCalledWith(expect.any(Date)); // endOfYear
  });

  it("recalculates overview data when data or overview type changes", async () => {
    const queryData = generateMockData("2023-01-01", "2023-01-01");

    const mockQuery = mockQueryImplementation(queryData);
    const mockCalculation = mockCalculateNutritionSummary();

    render(<NutrientsCharts />);

    // Initial calculation
    expect(mockCalculation).toHaveBeenCalledTimes(1);

    // Change overview type
    fireEvent.click(screen.getByText("This week"));
    expect(mockCalculation).toHaveBeenCalledTimes(2);
    expect(mockCalculation).toHaveBeenLastCalledWith(
      queryData,
      "week",
      expect.any(Date),
    );

    // Change data
    const newData = generateMockData("2023-01-01", "2023-01-01");
    mockQuery.mockReturnValue({
      dailyNutritionOverviewsQuery: {
        data: newData,
      },
      isPending: {
        isLoading: false,
      },
    });

    // Re-render with data change trigger
    render(<NutrientsCharts />);

    // Wait for the effect to run
    await waitFor(() => {
      expect(mockCalculation).toHaveBeenCalledTimes(3);
      expect(mockCalculation).toHaveBeenLastCalledWith(
        newData,
        "day",
        expect.any(Date),
      );
    });
  });

  it("handles edge case with zero targets gracefully", () => {
    mockQueryImplementation(generateMockData("2023-01-01", "2023-01-01"));
    mockCalculateNutritionSummary({
      calories: { value: 500, target: 0 },
      carbs: { value: 50, target: 0 },
      proteins: { value: 30, target: 0 },
      fats: { value: 20, target: 0 },
    });

    render(<NutrientsCharts />);

    // Verify values are passed correctly to each card even with zero targets
    expect(screen.getByTestId("calories-current")).toHaveTextContent("500");
    expect(screen.getByTestId("calories-target")).toHaveTextContent("0");
    expect(screen.getByTestId("carbs-current")).toHaveTextContent("50");
    expect(screen.getByTestId("carbs-target")).toHaveTextContent("0");
  });

  it("handles edge case with exceeded targets gracefully", () => {
    mockQueryImplementation(generateMockData("2023-01-01", "2023-01-01"));
    mockCalculateNutritionSummary({
      calories: { value: 2500, target: 2000 },
      carbs: { value: 250, target: 200 },
      proteins: { value: 130, target: 100 },
      fats: { value: 80, target: 70 },
    });

    render(<NutrientsCharts />);

    // Verify values are passed correctly to each card even with exceeded targets
    expect(screen.getByTestId("calories-current")).toHaveTextContent("2500");
    expect(screen.getByTestId("calories-target")).toHaveTextContent("2000");
    expect(screen.getByTestId("carbs-current")).toHaveTextContent("250");
    expect(screen.getByTestId("carbs-target")).toHaveTextContent("200");
  });

  it("handles edge case with undefined data gracefully", () => {
    // Setup undefined data
    mockQueryImplementation(undefined);
    mockCalculateNutritionSummary({
      calories: { value: 0, target: 0 },
      carbs: { value: 0, target: 0 },
      proteins: { value: 0, target: 0 },
      fats: { value: 0, target: 0 },
    });

    render(<NutrientsCharts />);

    // Component should render Empty state for undefined data
    expect(screen.getByText("No data")).toBeInTheDocument();
  });

  it("handles loading state correctly", () => {
    // Setup loading state with pending query
    mockQueryImplementation(generateMockData("2023-01-01", "2023-01-01"), true);

    render(<NutrientsCharts />);

    // All cards should show loading state
    expect(screen.getAllByTestId(/-loading$/)).toEqual(
      expect.arrayContaining([
        expect.objectContaining({ textContent: "true" }),
        expect.objectContaining({ textContent: "true" }),
        expect.objectContaining({ textContent: "true" }),
        expect.objectContaining({ textContent: "true" }),
      ]),
    );
  });

  it("preserves state when re-rendering with same data", () => {
    mockQueryImplementation(generateMockData("2023-01-01", "2023-01-01"));
    mockCalculateNutritionSummary({
      calories: { value: 1500, target: 2000 },
      carbs: { value: 150, target: 200 },
      proteins: { value: 75, target: 100 },
      fats: { value: 50, target: 70 },
    });

    const { rerender } = render(<NutrientsCharts />);

    // Change to week view
    fireEvent.click(screen.getByText("This week"));

    // Re-render with same data
    rerender(<NutrientsCharts />);

    // State should be preserved
    expect(screen.getAllByTestId(/-overview-type$/)).toEqual(
      expect.arrayContaining([
        expect.objectContaining({ textContent: "week" }),
      ]),
    );
  });

  it("handles date boundaries correctly", () => {
    // Reset getDateOnly mock to verify date boundary calculations
    (getDateOnlyModule.getDateOnly as ReturnType<typeof vi.fn>).mockClear();
    mockQueryImplementation(generateMockData("2023-01-01", "2023-01-01"));

    render(<NutrientsCharts />);

    // Verify startOfYear and endOfYear were used to set date range
    expect(getDateOnlyModule.getDateOnly).toHaveBeenCalledTimes(4);

    const firstCallDate = (
      getDateOnlyModule.getDateOnly as ReturnType<typeof vi.fn>
    ).mock.calls[0][0];
    const secondCallDate = (
      getDateOnlyModule.getDateOnly as ReturnType<typeof vi.fn>
    ).mock.calls[1][0];

    // Verify the dates are created using startOfYear and endOfYear
    expect(firstCallDate.getMonth()).toBe(0); // January (0-indexed)
    expect(firstCallDate.getDate()).toBe(1); // 1st day

    expect(secondCallDate.getMonth()).toBe(11); // December (0-indexed)
    expect(secondCallDate.getDate()).toBe(31); // 31st day
  });
});
