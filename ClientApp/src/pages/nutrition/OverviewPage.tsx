import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import NutrientsCharts from "@/features/nutrition/overview/components/NutrientsCharts";
import { NutritionSummary } from "@/features/nutrition/overview/components/NutritionSummaryChart/NutritionSummary";

const OverviewPage = () => {
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
};

export default OverviewPage;
