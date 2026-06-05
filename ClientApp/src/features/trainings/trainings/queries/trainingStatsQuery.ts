import axios from "axios";
import { subDays } from "date-fns";
import { queryOptions } from "@tanstack/react-query";
import { z } from "zod";

import { getDateOnly, type DateOnly } from "@/lib/date";
import { env } from "@/lib/env";
import { queryClient } from "@/queryClient";
import type { Difficulty, WorkoutHistoryDto } from "@/services/openapi";

const trainingStatsRangeSchema = z.enum([
  "FourWeeks",
  "TwelveWeeks",
  "SixMonths",
  "All",
]);

const dateOnlySchema = z
  .string()
  .regex(/^\d{4}-\d{2}-\d{2}$/)
  .optional();

const chartAggregationSchema = z.enum(["Sum", "Average"]);

export const trainingStatsSearchSchema = z.object({
  range: trainingStatsRangeSchema.default("TwelveWeeks"),
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
  chartAggregation: chartAggregationSchema.default("Sum"),
});

export type TrainingStatsSearchSchemaOutput = z.output<
  typeof trainingStatsSearchSchema
>;

export type TrainingStatsRange =
  | "FourWeeks"
  | "TwelveWeeks"
  | "SixMonths"
  | "All";

export type TrainingStatsChartAggregation = "Sum" | "Average";

export type TrainingStatsSearch = {
  range: TrainingStatsRange;
  startDate?: DateOnly;
  endDate?: DateOnly;
  chartAggregation: TrainingStatsChartAggregation;
};

export function defaultTrainingStatsDateWindow(): {
  startDate: DateOnly;
  endDate: DateOnly;
} {
  const end = new Date();
  const start = subDays(end, 83);
  return { startDate: getDateOnly(start), endDate: getDateOnly(end) };
}

export function resolveTrainingStatsSearchFromParsedUrl(
  search: TrainingStatsSearchSchemaOutput,
): TrainingStatsSearch {
  if (search.range === "All" && !search.startDate && !search.endDate) {
    return {
      range: "All",
      chartAggregation: search.chartAggregation,
    };
  }
  const w = defaultTrainingStatsDateWindow();
  return {
    range: search.range,
    chartAggregation: search.chartAggregation,
    startDate: (search.startDate ?? w.startDate) as DateOnly,
    endDate: (search.endDate ?? w.endDate) as DateOnly,
  };
}

export type WorkoutAggregatedValuePoint = {
  date: string;
  value: number;
  startDate?: string | null;
  endDate?: string | null;
};

export type WorkoutFrequencyPoint = {
  date: string;
  workoutCount: number;
  startDate?: string | null;
  endDate?: string | null;
};

export type TrainingStatsDto = {
  trainingId: string;
  trainingName: string;
  difficulty: Difficulty;
  muscleGroups: string[];
  exerciseCount: number;
  estimatedDurationSeconds: number;
  selectedRange: TrainingStatsRange;
  chartAggregationType: TrainingStatsChartAggregation;
  summary: {
    sessionsCompleted: number;
    fullyCompletedCount: number;
    withSkippedCount: number;
    completionRate: number;
    averageDurationSeconds: number;
    totalDurationSeconds: number;
    averageCaloriesBurned?: number | null;
    totalCaloriesBurned?: number | null;
    lastPerformedOnUtc?: string | null;
    windowStartDate: string;
    windowEndDate: string;
  };
  durationTrend: WorkoutAggregatedValuePoint[];
  frequencyTrend: WorkoutFrequencyPoint[];
  caloriesTrend: WorkoutAggregatedValuePoint[];
  recentSessions: WorkoutHistoryDto[];
};

export const trainingStatsQueryKeys = {
  all: ["trainingStats"] as const,
  bySearch: (trainingId: string, search: TrainingStatsSearch) =>
    [
      ...trainingStatsQueryKeys.all,
      trainingId,
      search.range,
      search.startDate ?? null,
      search.endDate ?? null,
      search.chartAggregation,
    ] as const,
};

function trainingStatsPlaceholderSameTraining(trainingId: string) {
  return (
    previousData: TrainingStatsDto | undefined,
    previousQuery: { queryKey: readonly unknown[] } | undefined,
  ): TrainingStatsDto | undefined => {
    if (!previousData || !previousQuery) return undefined;
    const key = previousQuery.queryKey;
    if (key.length < 6) return undefined;
    const id = key[1] as string;
    if (id !== trainingId) return undefined;
    return previousData;
  };
}

export const trainingStatsQueryOptions = {
  bySearch: (trainingId: string, search: TrainingStatsSearch) =>
    queryOptions({
      queryKey: trainingStatsQueryKeys.bySearch(trainingId, search),
      placeholderData: trainingStatsPlaceholderSameTraining(trainingId),
      queryFn: async () => {
        const params: Record<string, string> = {
          range: search.range,
          chartAggregationType: search.chartAggregation,
        };
        if (search.startDate && search.endDate) {
          params.startDate = search.startDate;
          params.endDate = search.endDate;
        }
        const response = await axios.get<TrainingStatsDto>(
          `${env.VITE_API_PATH}/api/trainings/${trainingId}/stats`,
          { params },
        );
        return response.data;
      },
    }),
};

export async function ensureTrainingStatsData(
  trainingId: string,
  rawSearch: unknown,
) {
  const parsed = trainingStatsSearchSchema.parse(rawSearch);
  const trainingSearch = resolveTrainingStatsSearchFromParsedUrl(parsed);
  await queryClient.ensureQueryData(
    trainingStatsQueryOptions.bySearch(trainingId, trainingSearch),
  );
}
