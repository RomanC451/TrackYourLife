import NutritionTabCard from "@/features/nutrition/common/components/NutritionTabCard";
import NutrientsCharts from "@/features/nutrition/overview/components/NutrientsCharts";
import { NutritionSummary } from "@/features/nutrition/overview/components/NutritionSummaryChart/NutritionSummary";

// const dailyNutritionOverviewsApi = new DailyNutritionOverviewsApi();

const OverviewPage = () => {
  return (
    <NutritionTabCard>
      <div className="container mx-auto p-4 @container">
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-3xl font-bold">Nutrition Overview</h1>
        </div>
        <NutrientsCharts />
        <div className="mt-8">
          <NutritionSummary />
        </div>
      </div>
    </NutritionTabCard>
  );
};

export default OverviewPage;
