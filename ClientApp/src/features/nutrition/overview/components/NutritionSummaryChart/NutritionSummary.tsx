import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import {
  addDays,
  differenceInDays,
  endOfMonth,
  endOfWeek,
  startOfMonth,
  startOfWeek,
} from "date-fns";
import { DateRange } from "react-day-picker";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import Spinner from "@/components/ui/spinner";
import { getDateOnly } from "@/lib/date";
import { AggregationMode, OverviewType } from "@/services/openapi";

import { ViewMode } from "../../data/types";
import { dailyNutritionOverviewsQueryOptions } from "../../queries/useDailyNutritionOverviewsQuery";
import AggregationModeDropDownMenu from "./AggregationModeDropDownMenu";
import { DateRangeSelector } from "./DateRangeSelector";
import NutritionSummaryChart, {
  NutritionSummaryChartData,
} from "./NutritionSummaryChart";
import OverviewTypeDropDownMenu from "./OverviewTypeDropDownMenu";
import ViewModeDropDownMenu from "./ViewModeDropDownMenu";

// Placeholder data for empty state
const placeholderData = Array.from({ length: 7 }, (_, i) => ({
  name: `Day ${i + 1}`,
  startDate: new Date(),
  endDate: new Date(),
  calories: Math.random() * 2000 + 1000,
  carbs: Math.random() * 200 + 100,
  proteins: Math.random() * 100 + 50,
  fats: Math.random() * 80 + 40,
  caloriesTarget: 2000,
  carbsTarget: 250,
  proteinsTarget: 150,
  fatsTarget: 70,
}));

export function NutritionSummary() {
  const [overviewType, setOverviewType] = useState<OverviewType>("Daily");
  const [aggregationMode, setAggregationMode] =
    useState<AggregationMode>("Average");

  const [selectedRange, setSelectedRange] = useState<DateRange | undefined>({
    from: new Date(),
    to: new Date(),
  });

  const startDate = useMemo(() => {
    if (overviewType === "Daily")
      return getDateOnly(
        startOfWeek(selectedRange?.from ?? new Date(), { weekStartsOn: 1 }),
      );
    else if (overviewType === "Weekly")
      return getDateOnly(startOfMonth(selectedRange?.from ?? new Date()));
    else return getDateOnly(startOfMonth(selectedRange?.from ?? new Date()));
  }, [overviewType, selectedRange]);
  const endDate = useMemo(
    () => getEndDate(overviewType, selectedRange),
    [overviewType, selectedRange],
  );

  const dailyNutritionOverviewsQuery = useQuery(
    dailyNutritionOverviewsQueryOptions.byDateRange(
      startDate,
      endDate,
      overviewType,
      aggregationMode,
    ),
  );

  const [viewMode, setViewMode] = useState<ViewMode>("calories");

  const chartData: NutritionSummaryChartData =
    dailyNutritionOverviewsQuery.data?.map((overview) => ({
      name: overview.startDate,
      startDate: new Date(overview.startDate),
      endDate: new Date(overview.endDate),
      calories: overview.nutritionalContent.energy.value,
      carbs: overview.nutritionalContent.carbohydrates,
      proteins: overview.nutritionalContent.protein,
      fats: overview.nutritionalContent.fat,
      caloriesTarget: overview.caloriesGoal,
      carbsTarget: overview.carbohydratesGoal,
      proteinsTarget: overview.proteinGoal,
      fatsTarget: overview.fatGoal,
    })) ?? [];

  const isEmptyData = chartData?.length === 0;

  return (
    <Card>
      <CardHeader>
        <div className="flex flex-col items-center justify-center gap-2 sm:flex-row sm:justify-between">
          <CardTitle className="text-xl">Nutrition Summary</CardTitle>
          <DateRangeSelector
            handleRangeSelect={setSelectedRange}
            selectedRange={selectedRange}
          />
        </div>
        <div className="flex flex-col items-center gap-4 pt-2 sm:flex-row sm:flex-wrap sm:items-center sm:justify-end">
          <ViewModeDropDownMenu viewMode={viewMode} setViewMode={setViewMode} />
          <div className="max-w-[280px] space-x-4">
            <OverviewTypeDropDownMenu
              overviewType={overviewType}
              setOverviewType={setOverviewType}
            />
            <AggregationModeDropDownMenu
              aggregationMode={aggregationMode}
              setAggregationMode={setAggregationMode}
              hidden={overviewType === "Daily"}
            />
          </div>
        </div>
      </CardHeader>
      <CardContent className="-m-6 mt-0">
        <div className="relative aspect-square max-h-[600px] min-h-[400px] w-full">
          <NutritionSummaryChart
            chartData={isEmptyData ? placeholderData : (chartData ?? [])}
            overviewType={overviewType}
            aggregationMode={aggregationMode}
            viewMode={viewMode}
          />
          {isEmptyData && (
            <div className="absolute inset-0 flex items-center justify-center bg-background/80 backdrop-blur-sm">
              <p className="text-xl font-semibold text-muted-foreground">
                No diary entries for selected period
              </p>
            </div>
          )}
          {dailyNutritionOverviewsQuery.isFetching && (
            <div className="absolute inset-0 flex items-center justify-center bg-background/80 backdrop-blur-sm">
              <Spinner className="h-10 w-10 fill-primary" />
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}

function getEndDate(
  overviewType: OverviewType,
  selectedRange: DateRange | undefined,
) {
  if (overviewType === "Daily")
    return getDateOnly(
      endOfWeek(selectedRange?.to ?? new Date(), { weekStartsOn: 1 }),
    );
  else if (overviewType === "Weekly")
    return getDateOnly(endOfMonth(selectedRange?.to ?? new Date()));
  else {
    const startDate = startOfMonth(selectedRange?.from ?? new Date());

    let endDate = endOfMonth(selectedRange?.to ?? new Date());

    if (differenceInDays(endDate, startDate) < 364) {
      endDate = addDays(startDate, 364);
    }

    return getDateOnly(endDate);
  }
}
