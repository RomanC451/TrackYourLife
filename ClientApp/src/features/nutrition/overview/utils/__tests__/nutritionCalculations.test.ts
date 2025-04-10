import { describe, expect, it } from "vitest";

import { DailyNutritionOverviewDto } from "@/services/openapi";

import { calculateNutritionSummary } from "../nutritionCalculations";

const generateMockData = (
  startDate: string,
  endDate: string,
): DailyNutritionOverviewDto[] => {
  const data: DailyNutritionOverviewDto[] = [];
  const currentDate = new Date(startDate);

  while (currentDate <= new Date(endDate)) {
    data.push({
      id: "1",
      date: currentDate.toISOString().split("T")[0],
      nutritionalContent: {
        energy: { value: 2000, unit: "kcal" },
        carbohydrates: 250,
        protein: 100,
        fat: 70,
        calcium: 200,
        cholesterol: 100,
        fiber: 30,
        iron: 15,
        potassium: 400,
        sodium: 300,
        sugar: 50,
        vitaminA: 1000,
        vitaminC: 100,
        monounsaturatedFat: 100,
        netCarbs: 100,
        polyunsaturatedFat: 100,
        saturatedFat: 100,
        transFat: 100,
      },
      caloriesGoal: 2200,
      carbohydratesGoal: 300,
      proteinGoal: 120,
      fatGoal: 80,
    });
    currentDate.setDate(currentDate.getDate() + 1);
  }

  return data;
};

const mockData = generateMockData("2023-01-01", "2023-12-31");

const emptyData: DailyNutritionOverviewDto[] = [];

const undefinedData: DailyNutritionOverviewDto[] | undefined = undefined;

const targetDate = new Date("2023-10-01");

const nonExistentDate = new Date("2024-01-01");

const leapYearDate = new Date("2024-02-29");

describe("calculateNutritionSummary", () => {
  it("should return zero values for empty data", () => {
    const summary = calculateNutritionSummary(emptyData, "day", targetDate);
    expect(summary).toEqual({
      calories: { value: 0, target: 0 },
      carbs: { value: 0, target: 0 },
      proteins: { value: 0, target: 0 },
      fats: { value: 0, target: 0 },
    });
  });

  it("should return zero values for undefined data", () => {
    const summary = calculateNutritionSummary(undefinedData, "day", targetDate);
    expect(summary).toEqual({
      calories: { value: 0, target: 0 },
      carbs: { value: 0, target: 0 },
      proteins: { value: 0, target: 0 },
      fats: { value: 0, target: 0 },
    });
  });

  it("should calculate daily nutrition summary", () => {
    const summary = calculateNutritionSummary(mockData, "day", targetDate);
    expect(summary).toEqual({
      calories: { value: 2000, target: 2200 },
      carbs: { value: 250, target: 300 },
      proteins: { value: 100, target: 120 },
      fats: { value: 70, target: 80 },
    });
  });

  it("should return zero values for non-existent day", () => {
    const summary = calculateNutritionSummary(mockData, "day", nonExistentDate);
    expect(summary).toEqual({
      calories: { value: 0, target: 0 },
      carbs: { value: 0, target: 0 },
      proteins: { value: 0, target: 0 },
      fats: { value: 0, target: 0 },
    });
  });

  it("should calculate weekly nutrition summary", () => {
    const summary = calculateNutritionSummary(mockData, "week", targetDate);
    expect(summary).toEqual({
      calories: { value: 14000, target: 15400 },
      carbs: { value: 1750, target: 2100 },
      proteins: { value: 700, target: 840 },
      fats: { value: 490, target: 560 },
    });
  });

  it("should return zero values for non-existent week", () => {
    const summary = calculateNutritionSummary(
      mockData,
      "week",
      nonExistentDate,
    );
    expect(summary).toEqual({
      calories: { value: 0, target: 0 },
      carbs: { value: 0, target: 0 },
      proteins: { value: 0, target: 0 },
      fats: { value: 0, target: 0 },
    });
  });

  it("should calculate monthly nutrition summary", () => {
    const summary = calculateNutritionSummary(mockData, "month", targetDate);
    expect(summary).toEqual({
      calories: { value: 60000, target: 66000 },
      carbs: { value: 7500, target: 9000 },
      proteins: { value: 3000, target: 3600 },
      fats: { value: 2100, target: 2400 },
    });
  });

  it("should return zero values for non-existent month", () => {
    const summary = calculateNutritionSummary(
      mockData,
      "month",
      nonExistentDate,
    );
    expect(summary).toEqual({
      calories: { value: 0, target: 0 },
      carbs: { value: 0, target: 0 },
      proteins: { value: 0, target: 0 },
      fats: { value: 0, target: 0 },
    });
  });

  it("should handle leap year date", () => {
    const leapYearData = generateMockData("2024-02-28", "2024-03-01");
    const summary = calculateNutritionSummary(
      leapYearData,
      "day",
      leapYearDate,
    );
    expect(summary).toEqual({
      calories: { value: 2000, target: 2200 },
      carbs: { value: 250, target: 300 },
      proteins: { value: 100, target: 120 },
      fats: { value: 70, target: 80 },
    });
  });
});
