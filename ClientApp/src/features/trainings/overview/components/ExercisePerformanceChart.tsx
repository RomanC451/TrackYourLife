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

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { colors } from "@/constants/tailwindColors";

import {
  exercisePerformanceQueryOptions,
  PerformanceCalculationMethod,
} from "../queries/useExercisePerformanceQuery";
import { DateRangeSelector } from "@/components/common/DateRangeSelector";
import { PaginationButtons } from "@/components/common/PaginationButtons";
import { CalculationMethodSelector } from "./CalculationMethodSelector";
import ExerciseHistoriesDialog from "./ExerciseHistoriesDialog";
import { useDateRange } from "../hooks/useDateRange";

const ITEMS_PER_PAGE = 10;

function ExercisePerformanceChart() {
  const { selectedRange, setSelectedRange, startDate, endDate } = useDateRange();
  const [calculationMethod, setCalculationMethod] =
    useState<PerformanceCalculationMethod>("Sequential");
  const [currentPage, setCurrentPage] = useState(1);
  const [selectedExerciseId, setSelectedExerciseId] = useState<string | null>(
    null,
  );
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const { query: performanceQuery } = useCustomQuery(
    exercisePerformanceQueryOptions.byFilters(
      startDate,
      endDate,
      null,
      calculationMethod,
      currentPage,
      ITEMS_PER_PAGE,
    ),
  );

  const chartData = useMemo(() => {
    if (!performanceQuery.data?.items || performanceQuery.data.items.length === 0) {
      return [];
    }

    return performanceQuery.data.items.map((item) => ({
      name: item.exerciseName,
      averageImprovement: item.improvementPercentage ?? 0,
      exerciseId: item.exerciseId,
    }));
  }, [performanceQuery.data]);

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
        <div className="flex flex-col gap-4">
          <div className="flex flex-col  gap-2 sm:flex-row sm:justify-between">
            <CardTitle className="text-xl">Exercise Performance</CardTitle>
            <div className="flex flex-col items-end gap-2 ">

              <DateRangeSelector
                handleRangeSelect={setSelectedRange}
                selectedRange={selectedRange}
              />
              <CalculationMethodSelector
                value={calculationMethod}
                onValueChange={setCalculationMethod}
              />
            </div>
          </div>
        </div>
      </CardHeader>
      <CardContent className="space-y-2">
        <ResponsiveContainer width="100%" height={400} >
          <BarChart
            data={chartData}
            layout="vertical"
            margin={{ top: 0, right: 0, left: -30, bottom: 0 }}
            onClick={(data) => {
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
              style={{ cursor: "pointer" }}
            />
          </BarChart>
        </ResponsiveContainer>
        <PaginationButtons
          pagedData={pagedData}
          onPreviousPage={handlePreviousPage}
          onNextPage={handleNextPage}
          onPageChange={handlePageChange}
          showCondition={(data) => data.maxPage > 1 || data.page > 1}
          maintainLayout
        />
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
