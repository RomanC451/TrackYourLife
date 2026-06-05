import type { QueryClient } from "@tanstack/react-query";
import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import {
  AggregationType,
  ExercisesApi,
  ExercisesHistoriesApi,
  OverviewType2,
  type PerformanceCalculationMethod,
  TrainingsApi,
} from "@/services/openapi";

const trainingsApi = new TrainingsApi();
const exercisesApi = new ExercisesApi();
const exercisesHistoriesApi = new ExercisesHistoriesApi();

const keepPrevious = { placeholderData: keepPreviousData } as const;

// --- Summary overview ---

export const trainingsOverviewQueryKeys = {
  all: ["trainingsOverview"] as const,
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    [...trainingsOverviewQueryKeys.all, startDate, endDate] as const,
};

export const trainingsOverviewQueryOptions = {
  all: () =>
    queryOptions({
      queryKey: trainingsOverviewQueryKeys.byDateRange(null, null),
      queryFn: () =>
        trainingsApi.getTrainingsOverview().then((res) => res.data),
      ...keepPrevious,
    }),
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    queryOptions({
      queryKey: trainingsOverviewQueryKeys.byDateRange(startDate, endDate),
      queryFn: () =>
        trainingsApi
          .getTrainingsOverview(startDate, endDate)
          .then((res) => res.data),
      ...keepPrevious,
    }),
};

// --- Workout frequency ---

export const workoutFrequencyQueryKeys = {
  all: ["workoutFrequency"] as const,
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2,
  ) =>
    [
      ...workoutFrequencyQueryKeys.all,
      startDate,
      endDate,
      overviewType,
    ] as const,
};

export const workoutFrequencyQueryOptions = {
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2 = "Daily",
  ) =>
    queryOptions({
      queryKey: workoutFrequencyQueryKeys.byFilters(
        startDate,
        endDate,
        overviewType,
      ),
      queryFn: () =>
        trainingsApi
          .getWorkoutFrequency(overviewType, startDate, endDate)
          .then((res) => res.data),
      ...keepPrevious,
    }),
};

// --- Calories burned history ---

export const caloriesBurnedHistoryQueryKeys = {
  all: ["caloriesBurnedHistory"] as const,
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2,
    aggregationType: AggregationType,
  ) =>
    [
      ...caloriesBurnedHistoryQueryKeys.all,
      startDate,
      endDate,
      overviewType,
      aggregationType,
    ] as const,
};

export const caloriesBurnedHistoryQueryOptions = {
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2 = "Weekly",
    aggregationType: AggregationType = "Sum",
  ) =>
    queryOptions({
      queryKey: caloriesBurnedHistoryQueryKeys.byFilters(
        startDate,
        endDate,
        overviewType,
        aggregationType,
      ),
      queryFn: () =>
        trainingsApi
          .getCaloriesBurnedHistory(
            overviewType,
            aggregationType,
            startDate,
            endDate,
          )
          .then((res) => res.data),
      ...keepPrevious,
    }),
};

// --- Difficulty distribution ---

export const difficultyDistributionQueryKeys = {
  all: ["difficultyDistribution"] as const,
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    [...difficultyDistributionQueryKeys.all, startDate, endDate] as const,
};

export const difficultyDistributionQueryOptions = {
  all: () =>
    queryOptions({
      queryKey: difficultyDistributionQueryKeys.byDateRange(null, null),
      queryFn: () =>
        trainingsApi.getDifficultyDistribution().then((res) => res.data),
      ...keepPrevious,
    }),
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    queryOptions({
      queryKey: difficultyDistributionQueryKeys.byDateRange(startDate, endDate),
      queryFn: () =>
        trainingsApi
          .getDifficultyDistribution(startDate, endDate)
          .then((res) => res.data),
      ...keepPrevious,
    }),
};

// --- Muscle group distribution ---

/** null = main groups only; string = subgroups of that muscle group. */
export type MuscleGroupFilter = string | null;

export const muscleGroupDistributionQueryKeys = {
  all: ["muscleGroupDistribution"] as const,
  byDateRange: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    muscleGroup: MuscleGroupFilter = null,
  ) =>
    [
      ...muscleGroupDistributionQueryKeys.all,
      startDate,
      endDate,
      muscleGroup,
    ] as const,
};

export const muscleGroupDistributionQueryOptions = {
  all: () =>
    queryOptions({
      queryKey: muscleGroupDistributionQueryKeys.byDateRange(null, null),
      queryFn: () =>
        trainingsApi.getMuscleGroupDistribution().then((res) => res.data),
      ...keepPrevious,
    }),
  byDateRange: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    muscleGroup: MuscleGroupFilter = null,
  ) =>
    queryOptions({
      queryKey: muscleGroupDistributionQueryKeys.byDateRange(
        startDate,
        endDate,
        muscleGroup,
      ),
      queryFn: () =>
        trainingsApi
          .getMuscleGroupDistribution(startDate, endDate, muscleGroup)
          .then((res) => res.data),
      ...keepPrevious,
    }),
};

// --- Top exercises ---

export const topExercisesQueryKeys = {
  all: ["topExercises"] as const,
  byPage: (page: number, pageSize: number) =>
    [...topExercisesQueryKeys.all, page, pageSize] as const,
  byFilters: (
    page: number,
    pageSize: number,
    startDate: DateOnly | null,
    endDate: DateOnly | null,
  ) =>
    [...topExercisesQueryKeys.all, page, pageSize, startDate, endDate] as const,
};

export const topExercisesQueryOptions = {
  byPage: (
    page: number,
    pageSize: number,
    startDate: DateOnly | null = null,
    endDate: DateOnly | null = null,
  ) =>
    queryOptions({
      queryKey: topExercisesQueryKeys.byFilters(
        page,
        pageSize,
        startDate,
        endDate,
      ),
      queryFn: () =>
        exercisesApi
          .getTopExercises(page, pageSize, startDate, endDate)
          .then((res) => res.data),
      ...keepPrevious,
    }),
};

// --- Exercise performance ---

export const exercisePerformanceQueryKeys = {
  all: ["exercisePerformance"] as const,
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    exerciseId: string | null,
    calculationMethod: PerformanceCalculationMethod,
    page: number,
    pageSize: number,
  ) =>
    [
      ...exercisePerformanceQueryKeys.all,
      startDate,
      endDate,
      exerciseId,
      calculationMethod,
      page,
      pageSize,
    ] as const,
};

export const exercisePerformanceQueryOptions = {
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    exerciseId: string | null,
    calculationMethod: PerformanceCalculationMethod = "Sequential",
    page: number = 1,
    pageSize: number = 10,
  ) =>
    queryOptions({
      queryKey: exercisePerformanceQueryKeys.byFilters(
        startDate,
        endDate,
        exerciseId,
        calculationMethod,
        page,
        pageSize,
      ),
      queryFn: () =>
        exercisesHistoriesApi
          .getExercisePerformance(
            page,
            pageSize,
            startDate,
            endDate,
            exerciseId,
            calculationMethod,
          )
          .then((res) => res.data),
      ...keepPrevious,
    }),
};

// --- Training templates usage ---

export const trainingTemplatesUsageQueryKeys = {
  all: ["trainingTemplatesUsage"] as const,
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    [...trainingTemplatesUsageQueryKeys.all, startDate, endDate] as const,
};

export const trainingTemplatesUsageQueryOptions = {
  all: queryOptions({
    queryKey: trainingTemplatesUsageQueryKeys.all,
    queryFn: () =>
      trainingsApi.getTrainingTemplatesUsage().then((res) => res.data),
    ...keepPrevious,
  }),
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    queryOptions({
      queryKey: trainingTemplatesUsageQueryKeys.byDateRange(startDate, endDate),
      queryFn: () =>
        trainingsApi
          .getTrainingTemplatesUsage(startDate, endDate)
          .then((res) => res.data),
      ...keepPrevious,
    }),
};

// --- Workout duration history ---

export const workoutDurationHistoryQueryKeys = {
  all: ["workoutDurationHistory"] as const,
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2,
    aggregationType: AggregationType,
  ) =>
    [
      ...workoutDurationHistoryQueryKeys.all,
      startDate,
      endDate,
      overviewType,
      aggregationType,
    ] as const,
};

export const workoutDurationHistoryQueryOptions = {
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2 = "Weekly",
    aggregationType: AggregationType = "Sum",
  ) =>
    queryOptions({
      queryKey: workoutDurationHistoryQueryKeys.byFilters(
        startDate,
        endDate,
        overviewType,
        aggregationType,
      ),
      queryFn: () =>
        trainingsApi
          .getWorkoutDurationHistory(
            overviewType,
            aggregationType,
            startDate,
            endDate,
          )
          .then((res) => res.data),
      ...keepPrevious,
    }),
};

// --- Bulk invalidation ---

export const allTrainingsOverviewKeys = [
  caloriesBurnedHistoryQueryKeys.all,
  difficultyDistributionQueryKeys.all,
  exercisePerformanceQueryKeys.all,
  muscleGroupDistributionQueryKeys.all,
  topExercisesQueryKeys.all,
  trainingsOverviewQueryKeys.all,
  trainingTemplatesUsageQueryKeys.all,
  workoutDurationHistoryQueryKeys.all,
  workoutFrequencyQueryKeys.all,
];

// --- Overview page prefetch (matches useDateRange default: last 31 days) ---

export function prefetchTrainingsOverviewPageQueries(
  queryClient: QueryClient,
  startDate: DateOnly,
  endDate: DateOnly,
) {
  return Promise.all([
    queryClient.ensureQueryData(
      trainingsOverviewQueryOptions.byDateRange(startDate, endDate),
    ),
    queryClient.ensureQueryData(
      workoutFrequencyQueryOptions.byFilters(startDate, endDate, "Weekly"),
    ),
    queryClient.ensureQueryData(
      muscleGroupDistributionQueryOptions.byDateRange(startDate, endDate),
    ),
    queryClient.ensureQueryData(
      difficultyDistributionQueryOptions.byDateRange(startDate, endDate),
    ),
    queryClient.ensureQueryData(
      workoutDurationHistoryQueryOptions.byFilters(
        startDate,
        endDate,
        "Weekly",
        "Sum",
      ),
    ),
    queryClient.ensureQueryData(
      caloriesBurnedHistoryQueryOptions.byFilters(
        startDate,
        endDate,
        "Weekly",
        "Sum",
      ),
    ),
    queryClient.ensureQueryData(
      exercisePerformanceQueryOptions.byFilters(
        startDate,
        endDate,
        null,
        "Sequential",
        1,
        10,
      ),
    ),
    queryClient.ensureQueryData(
      topExercisesQueryOptions.byPage(1, 10, startDate, endDate),
    ),
    queryClient.ensureQueryData(
      trainingTemplatesUsageQueryOptions.byDateRange(startDate, endDate),
    ),
  ]);
}
