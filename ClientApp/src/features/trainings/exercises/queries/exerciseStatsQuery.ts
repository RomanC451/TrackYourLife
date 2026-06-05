import { subDays } from "date-fns";
import { queryOptions } from "@tanstack/react-query";
import { z } from "zod";

import { getDateOnly, type DateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import {
  ExercisesHistoriesApi,
  type ExerciseStatsChartMetric,
  type ExerciseStatsDto,
  type ExerciseStatsRange,
} from "@/services/openapi";

const exercisesHistoriesApi = new ExercisesHistoriesApi();

const chartMetricSchema = z.enum([
  "Volume",
  "TotalWeight",
  "MaxWeight",
  "MinWeight",
  "TotalReps",
  "MaxReps",
  "MinReps",
]);

const dateOnlySchema = z
  .string()
  .regex(/^\d{4}-\d{2}-\d{2}$/)
  .optional();

/** Zod schema for stats URL search (used by route `validateSearch` and loaders). */
export const exerciseStatsSearchSchema = z.object({
  range: z
    .enum(["FourWeeks", "TwelveWeeks", "SixMonths", "All"])
    .default("TwelveWeeks"),
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
  chartMetric: chartMetricSchema.default("Volume"),
});

export type ExerciseStatsSearchSchemaOutput = z.output<
  typeof exerciseStatsSearchSchema
>;

export type ExerciseStatsSearch = {
  range: ExerciseStatsRange;
  startDate?: DateOnly;
  endDate?: DateOnly;
  chartMetric: ExerciseStatsChartMetric;
};

/** Matches backend TwelveWeeks default (today minus 83 days through today). */
export function defaultExerciseStatsDateWindow(): {
  startDate: DateOnly;
  endDate: DateOnly;
} {
  const end = new Date();
  const start = subDays(end, 83);
  return { startDate: getDateOnly(start), endDate: getDateOnly(end) };
}

/** Same resolution as the stats page: effective API search from validated URL search. */
export function resolveExerciseStatsSearchFromParsedUrl(
  search: ExerciseStatsSearchSchemaOutput,
): ExerciseStatsSearch {
  if (search.range === "All" && !search.startDate && !search.endDate) {
    return {
      range: "All",
      chartMetric: search.chartMetric,
    };
  }
  const w = defaultExerciseStatsDateWindow();
  return {
    range: search.range,
    chartMetric: search.chartMetric,
    startDate: (search.startDate ?? w.startDate) as DateOnly,
    endDate: (search.endDate ?? w.endDate) as DateOnly,
  };
}

export const exerciseStatsQueryKeys = {
  all: ["exerciseStats"] as const,
  bySearch: (exerciseId: string, search: ExerciseStatsSearch) =>
    [
      ...exerciseStatsQueryKeys.all,
      exerciseId,
      search.range,
      search.startDate ?? null,
      search.endDate ?? null,
      search.chartMetric,
    ] as const,
};

/** Keep prior stats while refetching whenever the exercise is unchanged (metric or date window updates). */
export function exerciseStatsPlaceholderSameExercise(exerciseId: string) {
  return (
    previousData: ExerciseStatsDto | undefined,
    previousQuery: { queryKey: readonly unknown[] } | undefined,
  ): ExerciseStatsDto | undefined => {
    if (!previousData || !previousQuery) return undefined;
    const key = previousQuery.queryKey;
    if (key.length < 6) return undefined;
    const id = key[1] as string;
    if (id !== exerciseId) return undefined;
    return previousData;
  };
}

export const exerciseStatsQueryOptions = {
  bySearch: (exerciseId: string, search: ExerciseStatsSearch) =>
    queryOptions({
      queryKey: exerciseStatsQueryKeys.bySearch(exerciseId, search),
      placeholderData: exerciseStatsPlaceholderSameExercise(exerciseId),
      queryFn: async () => {
        const response = await exercisesHistoriesApi.getExerciseStats(
          exerciseId,
          search.range,
          search.chartMetric,
          search.startDate ?? null,
          search.endDate ?? null,
        );
        return response.data;
      },

    }),
};

/** For route loaders / preload: parse URL search and fill the stats query cache. */
export async function ensureExerciseStatsData(
  exerciseId: string,
  rawSearch: unknown,
) {
  const parsed = exerciseStatsSearchSchema.parse(rawSearch);
  const exerciseSearch = resolveExerciseStatsSearchFromParsedUrl(parsed);
  await queryClient.ensureQueryData(
    exerciseStatsQueryOptions.bySearch(exerciseId, exerciseSearch),
  );
}
