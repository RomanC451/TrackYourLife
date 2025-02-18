import { useMemo, useState } from "react";
import { endOfYear, startOfYear } from "date-fns";
import { DateRange } from "react-day-picker";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { getDateOnly } from "@/lib/date";

import { AggregationMode, OverviewType, ViewMode } from "../../data/types";
import useDailyNutritionOverviewsQuery from "../../queries/useDailyNutritionOverviewsQuery";
import { aggregateOverviewsByPeriod } from "../../utils/overviewsAggregation";
import AggregationModeDropDownMenu from "./AggregationModeDropDownMenu";
import { DateRangeSelector } from "./DateRangeSelector";
import NutritionSummaryChart from "./NutritionSummaryChart";
import OverviewTypeDropDownMenu from "./OverviewTypeDropDownMenu";
import ViewModeDropDownMenu from "./ViewModeDropDownMenu";

export function NutritionSummary() {
  const [selectedRange, setSelectedRange] = useState<DateRange | undefined>({
    from: new Date(),
    to: new Date(),
  });

  const dailyNutritionOverviewsQuery = useDailyNutritionOverviewsQuery(
    getDateOnly(startOfYear(selectedRange?.from ?? new Date())),
    getDateOnly(endOfYear(selectedRange?.to ?? new Date())),
  );

  const [viewMode, setViewMode] = useState<ViewMode>("calories");
  const [overviewType, setOverviewType] = useState<OverviewType>("daily");
  const [aggregationMode, setAggregationMode] =
    useState<AggregationMode>("average");

  const chartData = useMemo(() => {
    if (
      !selectedRange?.from ||
      !selectedRange?.to ||
      !dailyNutritionOverviewsQuery.data
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
        <div className="aspect-square max-h-[600px] min-h-[400px] w-full">
          <NutritionSummaryChart
            chartData={chartData}
            overviewType={overviewType}
            aggregationMode={aggregationMode}
            viewMode={viewMode}
          />
        </div>
      </CardContent>
    </Card>
  );
}
