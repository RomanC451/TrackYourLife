import { useMemo, useState } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { colors } from "@/constants/tailwindColors";

import { ChartLoadingOverlay } from "@/components/common/ChartLoadingOverlay";
import { PaginationButtons } from "@/components/common/PaginationButtons";
import { topExercisesQueryOptions } from "../queries/useTopExercisesQuery";
import { useOverviewDateRange } from "../contexts/OverviewDateRangeContext";

const ITEMS_PER_PAGE = 10;

function TopExercisesChart() {
  const [currentPage, setCurrentPage] = useState(1);
  const { startDate, endDate } = useOverviewDateRange();

  const { query: topExercisesQuery, isDelayedFetching } = useCustomQuery(
    topExercisesQueryOptions.byPage(
      currentPage,
      ITEMS_PER_PAGE,
      startDate,
      endDate,
    ),
  );

  // Fetch page 1 to get the maximum values (top exercises will have highest counts)
  const { query: firstPageQuery } = useCustomQuery(
    topExercisesQueryOptions.byPage(1, ITEMS_PER_PAGE, startDate, endDate),
  );

  const chartData =
    topExercisesQuery.data?.items.map((exercise) => ({
      name: exercise.exerciseName,
      completed: exercise.completionCount,
      skipped: exercise.skipCount,
      improvement: exercise.improvementPercentage,
    })) ?? [];

  const pagedData = topExercisesQuery.data;

  // Calculate the maximum value from page 1 (which contains the top exercises with highest counts)
  const maxXAxisValue = useMemo(() => {
    if (!firstPageQuery.data?.items || firstPageQuery.data.items.length === 0) {
      return undefined;
    }

    const maxValue = firstPageQuery.data.items.reduce((max, exercise) => {
      // Consider both completed and skipped counts, and their sum
      const totalCount = exercise.completionCount + exercise.skipCount;
      return Math.max(max, exercise.completionCount, exercise.skipCount, totalCount);
    }, 0);

    // Add some padding (10% or at least 1) to make the chart look better
    return Math.ceil(maxValue * 1.1) || 1;
  }, [firstPageQuery.data]);

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

  return (
    <Card>
      <CardHeader>
        <div className="flex flex-col items-center justify-center gap-2 sm:flex-row sm:justify-between">
          <CardTitle className="text-xl">Top Exercises</CardTitle>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="relative">
          <ResponsiveContainer width="100%" height={400}>
            <BarChart data={chartData} layout="vertical"
              margin={{ top: 0, right: 0, left: -30, bottom: 0 }}
            >
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis
                type="number"
                domain={maxXAxisValue !== undefined ? [0, maxXAxisValue] : [0, "auto"]}
              />
              <YAxis
                dataKey="name"
                type="category"
                width={150}
                tick={{ fontSize: 12 }}
              />
              <Tooltip
                contentStyle={{
                  backgroundColor: "hsl(var(--background))",
                  borderColor: "hsl(var(--border))",
                  borderRadius: "var(--radius)",
                  color: "hsl(var(--foreground))",
                }}
              />
              <Legend wrapperStyle={{ marginLeft: "30px" }} />
              <Bar dataKey="completed" fill={colors.green} name="Completed" />
              <Bar dataKey="skipped" fill={colors.red} name="Skipped" />

            </BarChart>
          </ResponsiveContainer>
          <ChartLoadingOverlay show={isDelayedFetching} />
        </div>
        <PaginationButtons
          pagedData={pagedData}
          onPreviousPage={handlePreviousPage}
          onNextPage={handleNextPage}
          onPageChange={handlePageChange}
          showCondition={(data) => data.maxPage > 1}
          loading={topExercisesQuery.isFetching}
        />
      </CardContent>
    </Card>
  );
}

export default TopExercisesChart;
