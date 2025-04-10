import {
  differenceInDays,
  differenceInWeeks,
  eachDayOfInterval,
  eachMonthOfInterval,
  eachWeekOfInterval,
  endOfMonth,
  endOfWeek,
  format,
  getYear,
  isSameDay,
  lastDayOfYear,
  startOfYear,
  subDays,
  subWeeks,
} from "date-fns";
import { DateRange } from "react-day-picker";

import { DailyNutritionOverviewDto } from "@/services/openapi";

import {
  addNutritionalContent,
  createEmptyNutritionalContent,
} from "../../common/utils/nutritionalContent";

export function getOverviewForDate(
  data: DailyNutritionOverviewDto[],
  date: Date,
) {
  const overview = data.find((d) => isSameDay(new Date(d.date), date));

  if (!overview)
    throw new Error("Overview not found for the given date " + date);

  return overview;
}

function aggregateOverviews(
  overviews: DailyNutritionOverviewDto[],
  aggregationMode: "average" | "sum",
) {
  const aggregate = overviews.reduce(
    (acc, curr) => ({
      nutritionalContent: addNutritionalContent(
        acc.nutritionalContent,
        curr.nutritionalContent,
      ),
      caloriesGoal: acc.caloriesGoal + curr.caloriesGoal,
      carbohydratesGoal: acc.carbohydratesGoal + curr.carbohydratesGoal,
      proteinGoal: acc.proteinGoal + curr.proteinGoal,
      fatGoal: acc.fatGoal + curr.fatGoal,
    }),
    {
      nutritionalContent: createEmptyNutritionalContent(),
      caloriesGoal: 0,
      carbohydratesGoal: 0,
      proteinGoal: 0,
      fatGoal: 0,
    },
  );

  return aggregationMode === "average"
    ? {
        calories: aggregate.nutritionalContent.energy.value / overviews.length,
        carbs: aggregate.nutritionalContent.carbohydrates / overviews.length,
        proteins: aggregate.nutritionalContent.protein / overviews.length,
        fats: aggregate.nutritionalContent.fat / overviews.length,
        caloriesTarget: aggregate.caloriesGoal / overviews.length,
        carbsTarget: aggregate.carbohydratesGoal / overviews.length,
        proteinsTarget: aggregate.proteinGoal / overviews.length,
        fatsTarget: aggregate.fatGoal / overviews.length,
      }
    : {
        calories: aggregate.nutritionalContent.energy.value,
        carbs: aggregate.nutritionalContent.carbohydrates,
        proteins: aggregate.nutritionalContent.protein,
        fats: aggregate.nutritionalContent.fat,
        caloriesTarget: aggregate.caloriesGoal,
        carbsTarget: aggregate.carbohydratesGoal,
        proteinsTarget: aggregate.proteinGoal,
        fatsTarget: aggregate.fatGoal,
      };
}

function getDailyData(
  startDate: Date,
  endDate: Date,
  data: DailyNutritionOverviewDto[],
) {
  let start = startDate;
  const end = endDate;

  const shouldExpandToWeek = differenceInDays(end, start) < 6;

  if (shouldExpandToWeek) {
    const daysDiff = 6 - differenceInDays(end, start);
    start = subDays(start, daysDiff);
  }

  return eachDayOfInterval({
    start,
    end,
  }).map((date) => {
    const overview = getOverviewForDate(data, date);
    return {
      name: format(date, "EEE, MMM dd"),
      startDate: date,
      endDate: date,
      calories: overview.nutritionalContent.energy.value,
      carbs: overview.nutritionalContent.carbohydrates,
      proteins: overview.nutritionalContent.protein,
      fats: overview.nutritionalContent.fat,
      caloriesTarget: overview.caloriesGoal,
      carbsTarget: overview.carbohydratesGoal,
      proteinsTarget: overview.proteinGoal,
      fatsTarget: overview.fatGoal,
    };
  });
}

export function getWeeklyData(
  startDate: Date,
  endDate: Date,
  data: DailyNutritionOverviewDto[],
  aggregationMode: "average" | "sum",
) {
  const weeksDiff = differenceInWeeks(endDate, startDate);

  const start = weeksDiff < 4 ? subWeeks(endDate, 3) : startDate;
  const end = endDate;

  return eachWeekOfInterval({ start, end }, { weekStartsOn: 1 }).map(
    (weekStart) => {
      const weekEnd = endOfWeek(weekStart, { weekStartsOn: 1 });

      const daysInWeek = eachDayOfInterval({
        start: weekStart,
        end: weekEnd,
      });

      const overviews = daysInWeek.map((date) =>
        getOverviewForDate(data, date),
      );

      const aggregate = aggregateOverviews(overviews, aggregationMode);

      return {
        name: `${format(weekStart, "MMM dd")} - ${format(weekEnd, "MMM dd")}`,
        startDate: weekStart,
        endDate: weekEnd,
        ...aggregate,
      };
    },
  );
}

function getMonthlyData(
  startDate: Date,
  endDate: Date,
  data: DailyNutritionOverviewDto[],
  aggregationMode: "average" | "sum" = "average",
) {
  const start = startOfYear(new Date(getYear(startDate), 0, 1));
  const end = lastDayOfYear(new Date(getYear(endDate), 0, 1));

  return eachMonthOfInterval({
    start,
    end,
  }).map((monthStart) => {
    const monthEnd = endOfMonth(monthStart);

    const daysInMonth = eachDayOfInterval({
      start: monthStart,
      end: monthEnd,
    });

    const overviews = daysInMonth.map((date) => getOverviewForDate(data, date));

    const aggregate = aggregateOverviews(overviews, aggregationMode);

    return {
      name: format(monthStart, "MMMM yyyy"),
      startDate: monthStart,
      endDate: monthEnd,
      ...aggregate,
    };
  });
}

export const aggregateOverviewsByPeriod = (
  dateRange: DateRange,
  overviewType: "daily" | "weekly" | "monthly",
  data: DailyNutritionOverviewDto[],
  aggregationMode: "average" | "sum" = "average",
) => {
  if (!dateRange.from || !dateRange.to) return [];

  switch (overviewType) {
    case "daily": {
      return getDailyData(dateRange.from, dateRange.to, data);
    }

    case "weekly": {
      return getWeeklyData(dateRange.from, dateRange.to, data, aggregationMode);
    }

    case "monthly": {
      return getMonthlyData(
        dateRange.from,
        dateRange.to,
        data,
        aggregationMode,
      );
    }
  }
};
