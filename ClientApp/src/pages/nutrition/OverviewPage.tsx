import { endOfWeek, startOfWeek } from "date-fns";

import { Button } from "@/components/ui/button";
import NutritionTabCard from "@/features/nutrition/common/components/NutritionTabCard";
import NutrientsCharts from "@/features/nutrition/overview/components/NutrientsCharts";
import { NutritionSummary } from "@/features/nutrition/overview/components/NutritionSummaryChart/NutritionSummary";
import { getDateOnly } from "@/lib/date";
import { DailyNutritionOverviewsApi } from "@/services/openapi";

const dailyNutritionOverviewsApi = new DailyNutritionOverviewsApi();

const OverviewPage = () => {
  return (
    <NutritionTabCard>
      <div className="container mx-auto p-4">
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-3xl font-bold">Nutrition Overview</h1>

          <Button
            onClick={() =>
              dailyNutritionOverviewsApi.getDailyNutritionOverviewsByDateRange(
                getDateOnly(startOfWeek(new Date(2024, 7, 1))),
                getDateOnly(endOfWeek(new Date(2024, 8, 1))),
              )
            }
          >
            Seed Database
          </Button>
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
