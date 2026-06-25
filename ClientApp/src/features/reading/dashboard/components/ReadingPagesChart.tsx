"use no memo";

import { useState } from "react";
import { format, isSameDay } from "date-fns";
import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { ChartLoadingOverlay } from "@/components/common/ChartLoadingOverlay";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { colors } from "@/constants/tailwindColors";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import type { ReadingOverviewType } from "@/services/openapi";

import { readingPagesHistoryQueryOptions } from "../../queries/readingQueries";
import ReadingOverviewTypeDropDownMenu from "./ReadingOverviewTypeDropDownMenu";

function ReadingPagesChart() {
  const [overviewType, setOverviewType] =
    useState<ReadingOverviewType>("Weekly");

  const { query: pagesHistoryQuery, isDelayedFetching } = useCustomQuery(
    readingPagesHistoryQueryOptions.byOverviewType(overviewType),
  );

  const chartData =
    pagesHistoryQuery.data?.map((item) => ({
      date: item.date,
      pages: item.pages,
      startDate: item.startDate
        ? new Date(item.startDate)
        : new Date(item.date),
      endDate: item.endDate ? new Date(item.endDate) : new Date(item.date),
    })) ?? [];

  const hasReadingData = chartData.some((item) => item.pages > 0);

  const formatTooltipDate = (startDate?: Date, endDate?: Date) => {
    if (!startDate || !endDate) return "";

    if (isSameDay(startDate, endDate)) {
      return format(startDate, "MMM dd, yyyy");
    }

    if (overviewType === "Weekly") {
      return `${format(startDate, "EEE, MMM dd")} - ${format(endDate, "EEE, MMM dd, yyyy")}`;
    }

    return `${format(startDate, "MMM dd")} - ${format(endDate, "MMM dd, yyyy")}`;
  };

  const formatXAxisTick = (value: string) => {
    const date = new Date(value);

    if (overviewType === "Daily") {
      return format(date, "MMM dd");
    }

    if (overviewType === "Weekly") {
      return format(date, "MMM dd");
    }

    return format(date, "MMM yyyy");
  };

  return (
    <Card>
      <CardHeader>
        <div className="flex flex-col items-center justify-center gap-2 sm:flex-row sm:items-start sm:justify-between">
          <CardTitle>Pages read</CardTitle>
          <ReadingOverviewTypeDropDownMenu
            overviewType={overviewType}
            setOverviewType={setOverviewType}
            loading={pagesHistoryQuery.isFetching}
          />
        </div>
      </CardHeader>
      <CardContent className="relative px-3 py-4">
        {!hasReadingData && !pagesHistoryQuery.isFetching ? (
          <p className="text-muted-foreground text-sm">
            Finish a reading session to see your pages chart.
          </p>
        ) : (
          <ResponsiveContainer width="100%" height={300}>
            <BarChart
              data={chartData}
              margin={{ top: 0, right: 0, left: -30, bottom: 0 }}
            >
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="date" tickFormatter={formatXAxisTick} />
              <YAxis
                domain={[0, (dataMax: number) => Math.max(3, Math.ceil(dataMax))]}
                allowDecimals={false}
              />
              <Tooltip
                contentStyle={{
                  backgroundColor: "hsl(var(--background))",
                  borderColor: "hsl(var(--border))",
                  borderRadius: "var(--radius)",
                  color: "hsl(var(--foreground))",
                }}
                labelFormatter={(_, data) =>
                  formatTooltipDate(
                    data[0]?.payload?.startDate,
                    data[0]?.payload?.endDate,
                  )
                }
              />
              <Bar dataKey="pages" fill={colors.blue} name="Pages" />
            </BarChart>
          </ResponsiveContainer>
        )}
        <ChartLoadingOverlay show={isDelayedFetching} />
      </CardContent>
    </Card>
  );
}

export default ReadingPagesChart;
