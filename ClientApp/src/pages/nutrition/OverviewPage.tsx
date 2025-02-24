import { Button } from "@/components/ui/button";
import NutritionTabCard from "@/features/nutrition/common/components/NutritionTabCard";
import NutrientsCharts from "@/features/nutrition/overview/components/NutrientsCharts";
import { invalidateDailyNutritionOverviewsQuery } from "@/features/nutrition/overview/queries/useDailyNutritionOverviewsQuery";

// const dailyNutritionOverviewsApi = new DailyNutritionOverviewsApi();

const OverviewPage = () => {
  return (
    <NutritionTabCard>
      <div className="@container container mx-auto p-4">
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-3xl font-bold">Nutrition Overview</h1>

          <Button onClick={() => invalidateDailyNutritionOverviewsQuery()}>
            Refresh data
          </Button>
        </div>
        <NutrientsCharts />
        <div className="mt-8">{/* <NutritionSummary /> */}</div>
      </div>
    </NutritionTabCard>
  );
};

export default OverviewPage;
