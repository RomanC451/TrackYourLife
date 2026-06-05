import { useEffect, useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { createFileRoute, useNavigate } from "@tanstack/react-router";

import {
  defaultTrainingStatsDateWindow,
  ensureTrainingStatsData,
  trainingStatsQueryOptions,
  trainingStatsSearchSchema,
  type TrainingStatsSearch,
  resolveTrainingStatsSearchFromParsedUrl,
} from "@/features/trainings/trainings/queries/trainingStatsQuery";
import { type DateOnly } from "@/lib/date";
import type { AggregationType, ExerciseStatsRange } from "@/services/openapi";
import TrainingStatsPage from "@/pages/trainings/TrainingStatsPage";
import LoadingPage from "@/pages/LoadingPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts/$workoutId/stats",
)({
  validateSearch: trainingStatsSearchSchema,
  loader: async ({ params, location }) => {
    await ensureTrainingStatsData(params.workoutId, location.search);
  },
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();
  const { workoutId } = Route.useParams();
  const search = Route.useSearch();

  const effectiveSearch: TrainingStatsSearch = useMemo(
    () => resolveTrainingStatsSearchFromParsedUrl(search),
    [search],
  );

  useEffect(() => {
    if (search.startDate && search.endDate) return;
    if (search.range === "All") return;
    const w = defaultTrainingStatsDateWindow();
    navigate({
      replace: true,
      to: "/trainings/workouts/$workoutId/stats",
      params: { workoutId },
      search: {
        range: search.range,
        chartAggregation: search.chartAggregation,
        startDate: w.startDate,
        endDate: w.endDate,
      } as never,
    });
  }, [
    workoutId,
    navigate,
    search.chartAggregation,
    search.endDate,
    search.range,
    search.startDate,
  ]);

  const statsQuery = useQuery({
    ...trainingStatsQueryOptions.bySearch(workoutId, effectiveSearch),
  });

  if (!statsQuery.data) {
    if (statsQuery.isError) {
      return (
        <div className="p-6 text-center text-sm text-muted-foreground">
          Could not load training stats.
        </div>
      );
    }
    return <LoadingPage />;
  }

  const stats = statsQuery.data;

  const goToSearch = (next: TrainingStatsSearch) => {
    const searchPayload: {
      range: ExerciseStatsRange;
      chartAggregation: AggregationType;
      startDate?: DateOnly;
      endDate?: DateOnly;
    } = {
      range: next.range,
      chartAggregation: next.chartAggregation,
    };
    if (next.startDate && next.endDate) {
      searchPayload.startDate = next.startDate;
      searchPayload.endDate = next.endDate;
    }

    navigate({
      to: "/trainings/workouts/$workoutId/stats",
      params: { workoutId },
      search: searchPayload as never,
    });
  };

  return (
    <TrainingStatsPage
      stats={stats}
      trainingId={workoutId}
      search={effectiveSearch}
      isChartRefetching={statsQuery.isFetching && !statsQuery.isPending}
      onChartAggregationChange={(chartAggregation) => {
        goToSearch({
          ...effectiveSearch,
          chartAggregation,
        });
      }}
      onCustomDateRangeChange={(startDate: DateOnly, endDate: DateOnly) => {
        goToSearch({
          ...effectiveSearch,
          range: "TwelveWeeks",
          startDate,
          endDate,
        });
      }}
      onAllTimeRangeSelect={() => {
        goToSearch({ ...effectiveSearch, range: "All" });
      }}
      onClearCustomDates={() => {
        const w = defaultTrainingStatsDateWindow();
        goToSearch({
          ...effectiveSearch,
          range: "TwelveWeeks",
          startDate: w.startDate,
          endDate: w.endDate,
        });
      }}
    />
  );
}
