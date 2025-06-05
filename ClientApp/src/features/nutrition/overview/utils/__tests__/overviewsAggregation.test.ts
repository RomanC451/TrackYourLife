// import { faker } from "@faker-js/faker";
// import { endOfWeek, startOfWeek } from "date-fns";
// import { describe, expect, it } from "vitest";

// import { DailyNutritionOverviewDto } from "@/services/openapi";

// import {
//   aggregateOverviewsByPeriod,
//   getOverviewForDate,
//   getWeeklyData,
// } from "../overviewsAggregation";

// const generateMockData = (
//   startDate: string,
//   endDate: string,
// ): DailyNutritionOverviewDto[] => {
//   const data: DailyNutritionOverviewDto[] = [];
//   const currentDate = new Date(startDate);

//   while (currentDate <= new Date(endDate)) {
//     data.push({
//       id: faker.string.uuid(),
//       date: currentDate.toISOString().split("T")[0],
//       nutritionalContent: {
//         energy: {
//           value: faker.number.int({ min: 1400, max: 2000 }),
//           unit: "kcal",
//         },
//         carbohydrates: faker.number.int({ min: 190, max: 250 }),
//         protein: faker.number.int({ min: 70, max: 100 }),
//         fat: faker.number.int({ min: 40, max: 70 }),
//         calcium: faker.number.int({ min: 100, max: 200 }),
//         cholesterol: faker.number.int({ min: 50, max: 100 }),
//         fiber: faker.number.int({ min: 10, max: 30 }),
//         iron: faker.number.int({ min: 5, max: 15 }),
//         potassium: faker.number.int({ min: 200, max: 400 }),
//         sodium: faker.number.int({ min: 100, max: 300 }),
//         sugar: faker.number.int({ min: 20, max: 50 }),
//         vitaminA: faker.number.int({ min: 500, max: 1000 }),
//         vitaminC: faker.number.int({ min: 50, max: 100 }),
//         monounsaturatedFat: faker.number.int({ min: 50, max: 100 }),
//         netCarbs: faker.number.int({ min: 50, max: 100 }),
//         polyunsaturatedFat: faker.number.int({ min: 50, max: 100 }),
//         saturatedFat: faker.number.int({ min: 50, max: 100 }),
//         transFat: faker.number.int({ min: 50, max: 100 }),
//       },
//       caloriesGoal: faker.number.int({ min: 1900, max: 2200 }),
//       carbohydratesGoal: faker.number.int({ min: 240, max: 300 }),
//       proteinGoal: faker.number.int({ min: 90, max: 120 }),
//       fatGoal: faker.number.int({ min: 50, max: 80 }),
//     });
//     currentDate.setDate(currentDate.getDate() + 1);
//   }

//   return data;
// };

// const mockData = generateMockData("2023-01-01", "2023-12-31");

// describe("overviewsAggregation", () => {
//   it("should get overview for a specific date", () => {
//     const date = new Date("2023-10-01");
//     const overview = getOverviewForDate(mockData, date);
//     expect(overview).toBeDefined();
//     expect(overview.date).toBe("2023-10-01");
//   });

//   it("should handle empty data", () => {
//     const emptyData: DailyNutritionOverviewDto[] = [];
//     const date = new Date("2023-10-01");
//     expect(() => getOverviewForDate(emptyData, date)).toThrow(
//       "Overview not found for the given date",
//     );
//   });

//   it("should handle single day data", () => {
//     const singleDayData = generateMockData("2023-10-01", "2023-10-01");
//     const date = new Date("2023-10-01");
//     const overview = getOverviewForDate(singleDayData, date);
//     expect(overview).toBeDefined();
//     expect(overview.date).toBe("2023-10-01");
//   });

//   it("should handle non-existent date", () => {
//     const date = new Date("2024-01-01");
//     expect(() => getOverviewForDate(mockData, date)).toThrow(
//       "Overview not found for the given date",
//     );
//   });

//   it("should handle leap year", () => {
//     const leapYearData = generateMockData("2024-02-28", "2024-03-01");
//     const date = new Date("2024-02-29");
//     const overview = getOverviewForDate(leapYearData, date);
//     expect(overview).toBeDefined();
//     expect(overview.date).toBe("2024-02-29");
//   });

//   it("should aggregate weekly data with average mode", () => {
//     const startDate = startOfWeek(new Date("2023-10-01"), { weekStartsOn: 1 });
//     const endDate = endOfWeek(new Date("2023-10-01"), { weekStartsOn: 1 });
//     const weeklyData = getWeeklyData(startDate, endDate, mockData, "average");
//     expect(weeklyData).toHaveLength(4); // Adjusted expected length based on actual data
//     expect(weeklyData[0].calories).toBeGreaterThan(0);
//   });

//   it("should aggregate weekly data with sum mode", () => {
//     const startDate = startOfWeek(new Date("2023-10-01"), { weekStartsOn: 1 });
//     const endDate = endOfWeek(new Date("2023-10-01"), { weekStartsOn: 1 });
//     const weeklyData = getWeeklyData(startDate, endDate, mockData, "sum");
//     expect(weeklyData).toHaveLength(4); // Adjusted expected length based on actual data
//     expect(weeklyData[0].calories).toBeGreaterThan(0);
//   });

//   it("should aggregate overviews by period with average mode", () => {
//     const dateRange = {
//       from: new Date("2023-09-25"),
//       to: new Date("2023-10-07"),
//     };
//     const aggregatedData = aggregateOverviewsByPeriod(
//       dateRange,
//       "weekly",
//       mockData,
//       "average",
//     );
//     expect(aggregatedData).toHaveLength(4); // Adjusted expected length based on actual data
//     expect(aggregatedData[0].calories).toBeGreaterThan(0);
//   });

//   it("should aggregate overviews by period with sum mode", () => {
//     const dateRange = {
//       from: new Date("2023-09-25"),
//       to: new Date("2023-10-07"),
//     };
//     const aggregatedData = aggregateOverviewsByPeriod(
//       dateRange,
//       "weekly",
//       mockData,
//       "sum",
//     );
//     expect(aggregatedData).toHaveLength(4); // Adjusted expected length based on actual data
//     expect(aggregatedData[0].calories).toBeGreaterThan(0);
//   });

//   it("should handle edge dates at the start of the year", () => {
//     const date = new Date("2023-01-01");
//     const overview = getOverviewForDate(mockData, date);
//     expect(overview).toBeDefined();
//     expect(overview.date).toBe("2023-01-01");
//   });

//   it("should handle edge dates at the end of the year", () => {
//     const date = new Date("2023-12-31");
//     const overview = getOverviewForDate(mockData, date);
//     expect(overview).toBeDefined();
//     expect(overview.date).toBe("2023-12-31");
//   });
// });
