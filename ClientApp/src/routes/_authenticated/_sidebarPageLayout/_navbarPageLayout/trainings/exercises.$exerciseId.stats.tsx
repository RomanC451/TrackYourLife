import { useEffect, useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { createFileRoute, useNavigate } from "@tanstack/react-router";

import {
  defaultExerciseStatsDateWindow,
  ensureExerciseStatsData,
  exerciseStatsQueryOptions,
  exerciseStatsSearchSchema,
  type ExerciseStatsChartMetric,
  type ExerciseStatsRange,
  type ExerciseStatsSearch,
  resolveExerciseStatsSearchFromParsedUrl,
} from "@/features/trainings/exercises/queries/exerciseStatsQuery";
import { type DateOnly } from "@/lib/date";
import ExerciseStatsPage from "@/pages/trainings/ExerciseStatsPage";
import LoadingPage from "@/pages/LoadingPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/exercises/$exerciseId/stats",
)({
  validateSearch: exerciseStatsSearchSchema,
  loader: async ({ params, location }) => {
    await ensureExerciseStatsData(params.exerciseId, location.search);
  },
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();
  const { exerciseId } = Route.useParams();
  const search = Route.useSearch();

  const effectiveSearch: ExerciseStatsSearch = useMemo(
    () => resolveExerciseStatsSearchFromParsedUrl(search),
    [search],
  );

  useEffect(() => {
    if (search.startDate && search.endDate) return;
    if (search.range === "All") return;
    const w = defaultExerciseStatsDateWindow();
    navigate({
      replace: true,
      to: "/trainings/exercises/$exerciseId/stats",
      params: { exerciseId },
      search: {
        range: search.range,
        chartMetric: search.chartMetric,
        startDate: w.startDate,
        endDate: w.endDate,
      } as never,
    });
  }, [
    exerciseId,
    navigate,
    search.chartMetric,
    search.endDate,
    search.range,
    search.startDate,
  ]);

  const statsQuery = useQuery({
    ...exerciseStatsQueryOptions.bySearch(exerciseId, effectiveSearch),
  });

  if (!statsQuery.data) {
    if (statsQuery.isError) {
      return (
        <div className="p-6 text-center text-sm text-muted-foreground">
          Could not load exercise stats.
        </div>
      );
    }
    return <LoadingPage />;
  }

  const stats = statsQuery.data;

  const goToSearch = (next: ExerciseStatsSearch) => {
    const searchPayload: {
      range: ExerciseStatsRange;
      chartMetric: ExerciseStatsChartMetric;
      startDate?: DateOnly;
      endDate?: DateOnly;
    } = {
      range: next.range,
      chartMetric: next.chartMetric,
    };
    if (next.startDate && next.endDate) {
      searchPayload.startDate = next.startDate;
      searchPayload.endDate = next.endDate;
    }

    navigate({
      to: "/trainings/exercises/$exerciseId/stats",
      params: { exerciseId },
      search: searchPayload as never,
    });
  };

  return (
    <ExerciseStatsPage
      stats={stats}
      search={effectiveSearch}
      onChartMetricChange={(chartMetric: ExerciseStatsChartMetric) => {
        goToSearch({
          ...effectiveSearch,
          chartMetric,
        });
      }}
      onCustomDateRangeChange={(startDate, endDate) => {
        goToSearch({
          range: "TwelveWeeks",
          chartMetric: effectiveSearch.chartMetric,
          startDate,
          endDate,
        });
      }}
      onAllTimeRangeSelect={() => {
        goToSearch({
          range: "All",
          chartMetric: effectiveSearch.chartMetric,
        });
      }}
      onClearCustomDates={() => {
        const w = defaultExerciseStatsDateWindow();
        goToSearch({
          range: "TwelveWeeks",
          chartMetric: effectiveSearch.chartMetric,
          startDate: w.startDate,
          endDate: w.endDate,
        });
      }}
    />
  );
}
