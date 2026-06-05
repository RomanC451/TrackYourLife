import { createFileRoute } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import NutrientsCharts from "@/features/nutrition/overview/components/NutrientsCharts";
import { NutritionSummary } from "@/features/nutrition/overview/components/NutritionSummaryChart/NutritionSummary";
import { prefetchNutritionOverviewPageQueries } from "@/features/nutrition/overview/queries/useDailyNutritionOverviewsQuery";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/overview",
)({
  loader: () => prefetchNutritionOverviewPageQueries(queryClient),
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <PageCard>
      <PageTitle title="Nutrition Overview" />
      <div className="container mx-auto @container">
        <NutrientsCharts />
        <div className="mt-8">
          <NutritionSummary />
        </div>
      </div>
    </PageCard>
  );
}
