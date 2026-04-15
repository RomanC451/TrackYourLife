import { useEffect, useMemo, useState } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip as RechartsTooltip,
  XAxis,
  YAxis,
} from "recharts";

import { ChartProFeatureOverlay } from "@/components/common/ChartProFeatureOverlay";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { colors } from "@/constants/tailwindColors";

import {
  exercisePerformanceQueryOptions,
  PerformanceCalculationMethod,
} from "../queries/useExercisePerformanceQuery";
import { ChartLoadingOverlay } from "@/components/common/ChartLoadingOverlay";
import { PaginationButtons } from "@/components/common/PaginationButtons";
import { CalculationMethodSelector } from "./CalculationMethodSelector";
import ExerciseHistoriesDialog from "./ExerciseHistoriesDialog";
import { useOverviewDateRange } from "../contexts/OverviewDateRangeContext";

const ITEMS_PER_PAGE = 10;

/** Demo bars for Free users (performance query is disabled). */
const PLACEHOLDER_EXERCISE_PERFORMANCE_CHART: {
  name: string;
  averageImprovement: number;
  exerciseId: string;
}[] = [
  { name: "Bench Press", averageImprovement: 12.5, exerciseId: "" },
  { name: "Squat", averageImprovement: 9.2, exerciseId: "" },
  { name: "Deadlift", averageImprovement: 7.8, exerciseId: "" },
  { name: "Overhead Press", averageImprovement: 6.1, exerciseId: "" },
  { name: "Barbell Row", averageImprovement: 5.4, exerciseId: "" },
  { name: "Pull-up", averageImprovement: 4.9, exerciseId: "" },
];

function ExercisePerformanceChart() {
  const { isPro } = useAuthenticationContext();
  const { startDate, endDate } = useOverviewDateRange();
  const [calculationMethod, setCalculationMethod] =
    useState<PerformanceCalculationMethod>("Sequential");
  const [currentPage, setCurrentPage] = useState(1);
  const [selectedExerciseId, setSelectedExerciseId] = useState<string | null>(
    null,
  );
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const { query: performanceQuery, isDelayedFetching } = useCustomQuery({
    ...exercisePerformanceQueryOptions.byFilters(
      startDate,
      endDate,
      null,
      calculationMethod,
      currentPage,
      ITEMS_PER_PAGE,
    ),
    enabled: isPro,
  });

  const chartData = useMemo(() => {
    if (!isPro) {
      return PLACEHOLDER_EXERCISE_PERFORMANCE_CHART;
    }
    if (!performanceQuery.data?.items || performanceQuery.data.items.length === 0) {
      return [];
    }

    return performanceQuery.data.items.map((item) => ({
      name: item.exerciseName,
      averageImprovement: item.improvementPercentage ?? 0,
      exerciseId: item.exerciseId,
    }));
  }, [isPro, performanceQuery.data]);

  const pagedData = performanceQuery.data;

  const handlePreviousPage = () => {
    if (pagedData?.hasPreviousPage) {
      setCurrentPage((prev) => prev - 1);
    }
  };

  const handleNextPage = () => {
    if (pagedData?.hasNextPage) {
      setCurrentPage((prev) => prev + 1);
    }
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  useEffect(() => {
    setCurrentPage(1);
  }, [startDate, endDate, calculationMethod]);


  const handleDialogOpenChange = (open: boolean) => {
    setIsDialogOpen(open);
    if (!open) {
      setSelectedExerciseId(null);
    }
  };

  return (
    <Card>
      <CardHeader>
        <div className="flex flex-col items-center justify-center gap-2 sm:flex-row sm:justify-between sm:items-start">
          <CardTitle className="text-xl">Exercises Performance</CardTitle>
          <div className="flex flex-col items-end gap-2 ">
            <CalculationMethodSelector
              value={calculationMethod}
              onValueChange={setCalculationMethod}
              disabled={!isPro}
              loading={isPro && performanceQuery.isFetching}
            />
          </div>
        </div>
      </CardHeader>
      <CardContent className="space-y-2">
        <div className="relative overflow-hidden">
          <ResponsiveContainer width="100%" height={400} >
            <BarChart
              data={chartData}
              layout="vertical"
              margin={{ top: 0, right: 0, left: -30, bottom: 0 }}
              onClick={(data) => {
                if (!isPro) return;
                if (data?.activePayload && data.activePayload.length > 0) {
                  const payload = data.activePayload[0].payload as { exerciseId?: string };
                  if (payload?.exerciseId) {
                    setSelectedExerciseId(payload.exerciseId);
                    setIsDialogOpen(true);
                  }
                }
              }}
            >
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis type="number" />
              <YAxis
                dataKey="name"
                type="category"
                width={150}
                tick={{ fontSize: 12 }}
              />
              <RechartsTooltip
                contentStyle={{
                  backgroundColor: "hsl(var(--background))",
                  borderColor: "hsl(var(--border))",
                  borderRadius: "var(--radius)",
                  color: "hsl(var(--foreground))",
                }}
                formatter={(value: number) => `${value.toFixed(1)}%`}
              />
              <Bar
                dataKey="averageImprovement"
                fill={colors.violet}
                name="Avg Improvement %"
                style={{ cursor: isPro ? "pointer" : "default" }}
              />
            </BarChart>
          </ResponsiveContainer>
          <ChartLoadingOverlay show={isPro && isDelayedFetching} />
          <ChartProFeatureOverlay
            show={!isPro}
            title="Exercise performance is a Pro feature"
            description="Track improvement trends per exercise over your selected date range. Upgrade to see real performance analytics."
          />
        </div>
        {isPro ? (
          <PaginationButtons
            pagedData={pagedData}
            onPreviousPage={handlePreviousPage}
            onNextPage={handleNextPage}
            onPageChange={handlePageChange}
            showCondition={(data) => data.maxPage > 1 || data.page > 1}
            maintainLayout
            loading={performanceQuery.isFetching}
          />
        ) : null}
      </CardContent>
      {selectedExerciseId && (
        <ExerciseHistoriesDialog
          exerciseId={selectedExerciseId}
          open={isDialogOpen}
          onOpenChange={handleDialogOpenChange}
        />
      )}
    </Card>
  );
}

export default ExercisePerformanceChart;
