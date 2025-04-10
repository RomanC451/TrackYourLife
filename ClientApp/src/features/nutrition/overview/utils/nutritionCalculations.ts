import {
  endOfMonth,
  endOfWeek,
  isSameDay,
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

  if (overviewType === "week") {
    const weekStart = startOfWeek(targetDate, { weekStartsOn: 1 });
    const weekEnd = endOfWeek(targetDate, { weekStartsOn: 1 });

    const weekData = data.filter((day) => {
      const date = new Date(day.date);
      return date >= weekStart && date <= weekEnd;
    });

    if (!weekData.length) {
      return {
        calories: { value: 0, target: 0 },
        carbs: { value: 0, target: 0 },
        proteins: { value: 0, target: 0 },
        fats: { value: 0, target: 0 },
      };
    }

    return weekData.reduce(
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
        calories: { value: 0, target: 0 },
        carbs: { value: 0, target: 0 },
        proteins: { value: 0, target: 0 },
        fats: { value: 0, target: 0 },
      },
    );
  } else {
    const monthStart = startOfMonth(targetDate);
    const monthEnd = endOfMonth(targetDate);

    const monthData = data.filter((day) => {
      const date = new Date(day.date);
      return date >= monthStart && date <= monthEnd;
    });

    if (!monthData.length) {
      return {
        calories: { value: 0, target: 0 },
        carbs: { value: 0, target: 0 },
        proteins: { value: 0, target: 0 },
        fats: { value: 0, target: 0 },
      };
    }

    return monthData.reduce(
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
        calories: { value: 0, target: 0 },
        carbs: { value: 0, target: 0 },
        proteins: { value: 0, target: 0 },
        fats: { value: 0, target: 0 },
      },
    );
  }
};
