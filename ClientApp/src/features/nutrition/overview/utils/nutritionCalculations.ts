import {
  eachDayOfInterval,
  endOfMonth,
  endOfWeek,
  isSameDay,
  isSameMonth,
  isSameWeek,
  startOfMonth,
  startOfWeek,
} from "date-fns";

import { DailyNutritionOverviewDto } from "@/services/openapi";

import { OverviewType } from "../components/NutrientsCharts";

interface NutritionSummary {
  calories: {
    value: number;
    target: number;
  };
  carbs: {
    value: number;
    target: number;
  };
  proteins: {
    value: number;
    target: number;
  };
  fats: {
    value: number;
    target: number;
  };
}

export const calculateNutritionSummary = (
  data: DailyNutritionOverviewDto[] | undefined,
  overviewType: OverviewType,
  targetDate: Date, // Add target date parameter
): NutritionSummary => {
  if (!data || data.length === 0) {
    return {
      calories: { value: 0, target: 0 },
      carbs: { value: 0, target: 0 },
      proteins: { value: 0, target: 0 },
      fats: { value: 0, target: 0 },
    };
  }

  console.log(overviewType);

  // For daily view, find the matching date
  if (overviewType === "day") {
    const day = data.find((d) => isSameDay(new Date(d.date), targetDate));

    if (!day) {
      return {
        calories: { value: 0, target: 0 },
        carbs: { value: 0, target: 0 },
        proteins: { value: 0, target: 0 },
        fats: { value: 0, target: 0 },
      };
    }

    return {
      calories: {
        value: day.nutritionalContent.energy.value,
        target: day.caloriesGoal,
      },
      carbs: {
        value: day.nutritionalContent.carbohydrates,
        target: day.carbohydratesGoal,
      },
      proteins: {
        value: day.nutritionalContent.protein,
        target: day.proteinGoal,
      },
      fats: {
        value: day.nutritionalContent.fat,
        target: day.fatGoal,
      },
    };
  }

  // Filter data based on overview type and get expected date range
  const filteredData = data?.filter((day) => {
    const date = new Date(day.date);
    if (overviewType === "week") {
      return isSameWeek(date, targetDate, { weekStartsOn: 1 });
    }
    if (overviewType === "month") {
      return isSameMonth(date, targetDate);
    }
    return isSameDay(date, targetDate);
  });

  if (!filteredData?.length) {
    return {
      calories: { value: 0, target: 0 },
      carbs: { value: 0, target: 0 },
      proteins: { value: 0, target: 0 },
      fats: { value: 0, target: 0 },
    };
  }

  // Get the first and last available day's targets
  const firstDayTargets = {
    calories: filteredData[0].caloriesGoal,
    carbs: filteredData[0].carbohydratesGoal,
    proteins: filteredData[0].proteinGoal,
    fats: filteredData[0].fatGoal,
  };

  const lastDayTargets = {
    calories: filteredData[filteredData.length - 1].caloriesGoal,
    carbs: filteredData[filteredData.length - 1].carbohydratesGoal,
    proteins: filteredData[filteredData.length - 1].proteinGoal,
    fats: filteredData[filteredData.length - 1].fatGoal,
  };

  // Calculate expected date range
  const dateRange = {
    start:
      overviewType === "week"
        ? startOfWeek(targetDate, { weekStartsOn: 1 })
        : startOfMonth(targetDate),
    end:
      overviewType === "week"
        ? endOfWeek(targetDate, { weekStartsOn: 1 })
        : endOfMonth(targetDate),
  };

  // Count missing days at start and end
  const allDatesInRange = eachDayOfInterval(dateRange);
  const missingDaysAtStart =
    filteredData.length > 0
      ? allDatesInRange.filter((date) => date < new Date(filteredData[0].date))
          .length
      : 0;
  const missingDaysAtEnd =
    filteredData.length > 0
      ? allDatesInRange.filter(
          (date) => date > new Date(filteredData[filteredData.length - 1].date),
        ).length
      : 0;

  // Sum up existing days and add targets for missing days
  const summary = filteredData.reduce(
    (acc, day) => ({
      calories: {
        value: acc.calories.value + day.nutritionalContent.energy.value,
        target: acc.calories.target + day.caloriesGoal,
      },
      carbs: {
        value: acc.carbs.value + day.nutritionalContent.carbohydrates,
        target: acc.carbs.target + day.carbohydratesGoal,
      },
      proteins: {
        value: acc.proteins.value + day.nutritionalContent.protein,
        target: acc.proteins.target + day.proteinGoal,
      },
      fats: {
        value: acc.fats.value + day.nutritionalContent.fat,
        target: acc.fats.target + day.fatGoal,
      },
    }),
    {
      calories: {
        value: 0,
        target:
          missingDaysAtStart * firstDayTargets.calories +
          missingDaysAtEnd * lastDayTargets.calories,
      },
      carbs: {
        value: 0,
        target:
          missingDaysAtStart * firstDayTargets.carbs +
          missingDaysAtEnd * lastDayTargets.carbs,
      },
      proteins: {
        value: 0,
        target:
          missingDaysAtStart * firstDayTargets.proteins +
          missingDaysAtEnd * lastDayTargets.proteins,
      },
      fats: {
        value: 0,
        target:
          missingDaysAtStart * firstDayTargets.fats +
          missingDaysAtEnd * lastDayTargets.fats,
      },
    },
  );

  return summary;
};
