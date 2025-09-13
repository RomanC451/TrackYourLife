import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { endOfYear, startOfYear } from "date-fns";
import { DateRange } from "react-day-picker";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { getDateOnly } from "@/lib/date";

import { AggregationMode, OverviewType, ViewMode } from "../../data/types";
import { dailyNutritionOverviewsQueryOptions } from "../../queries/useDailyNutritionOverviewsQuery";
import { aggregateOverviewsByPeriod } from "../../utils/overviewsAggregation";
import AggregationModeDropDownMenu from "./AggregationModeDropDownMenu";
import { DateRangeSelector } from "./DateRangeSelector";
import NutritionSummaryChart from "./NutritionSummaryChart";
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
  const [selectedRange, setSelectedRange] = useState<DateRange | undefined>({
    from: new Date(),
    to: new Date(),
  });

  const dailyNutritionOverviewsQuery = useQuery(
    dailyNutritionOverviewsQueryOptions.byDateRange(
      getDateOnly(startOfYear(selectedRange?.from ?? new Date())),
      getDateOnly(endOfYear(selectedRange?.to ?? new Date())),
    ),
  );

  const [viewMode, setViewMode] = useState<ViewMode>("calories");
  const [overviewType, setOverviewType] = useState<OverviewType>("daily");
  const [aggregationMode, setAggregationMode] =
    useState<AggregationMode>("average");

  const chartData = useMemo(() => {
    if (
      !selectedRange?.from ||
      !selectedRange?.to ||
      !dailyNutritionOverviewsQuery.data ||
      dailyNutritionOverviewsQuery.data.length === 0
    )
      return [];

    return aggregateOverviewsByPeriod(
      selectedRange,
      overviewType,
      dailyNutritionOverviewsQuery.data,
      aggregationMode,
    );
  }, [
    selectedRange,
    overviewType,
    dailyNutritionOverviewsQuery.data,
    aggregationMode,
  ]);

  const isEmptyData = chartData.length === 0;

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
              hidden={overviewType === "daily"}
            />
          </div>
        </div>
      </CardHeader>
      <CardContent className="-m-6 mt-0">
        <div className="relative aspect-square max-h-[600px] min-h-[400px] w-full">
          <NutritionSummaryChart
            chartData={isEmptyData ? placeholderData : chartData}
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
        </div>
      </CardContent>
    </Card>
  );
}
