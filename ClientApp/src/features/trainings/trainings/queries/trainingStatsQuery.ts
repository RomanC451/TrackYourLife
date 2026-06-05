import { subDays } from "date-fns";
import { queryOptions } from "@tanstack/react-query";
import { z } from "zod";

import { getDateOnly, type DateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import {
  TrainingsApi,
  type AggregationType,
  type ExerciseStatsRange,
  type TrainingStatsDto,
} from "@/services/openapi";

const trainingsApi = new TrainingsApi();

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

export type TrainingStatsSearch = {
  range: ExerciseStatsRange;
  startDate?: DateOnly;
  endDate?: DateOnly;
  chartAggregation: AggregationType;
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

export function trainingStatsPlaceholderSameTraining(trainingId: string) {
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
        const response = await trainingsApi.getTrainingStats(
          trainingId,
          search.range,
          search.chartAggregation,
          search.startDate ?? null,
          search.endDate ?? null,
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
