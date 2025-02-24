import { useEffect, useState } from "react";
import { endOfYear, startOfYear } from "date-fns";

import { Button } from "@/components/ui/button";
import { colors } from "@/constants/tailwindColors";
import { getDateOnly } from "@/lib/date";

import useDailyNutritionOverviewsQuery from "../queries/useDailyNutritionOverviewsQuery";
import { calculateNutritionSummary } from "../utils/nutritionCalculations";
import { NutrientCard } from "./NutrientCard";

export type OverviewType = "day" | "week" | "month";

function NutrientsCharts() {
  const dailyNutritionOverviewsQuery = useDailyNutritionOverviewsQuery(
    getDateOnly(startOfYear(new Date())),
    getDateOnly(endOfYear(new Date())),
  );

  const [overviewType, setOverviewType] = useState<OverviewType>("day");

  const [overviewData, setOverviewData] = useState({
    calories: { value: 0, target: 0 },
    carbs: { value: 0, target: 0 },
    proteins: { value: 0, target: 0 },
    fats: { value: 0, target: 0 },
  });

  useEffect(() => {
    setOverviewData(
      calculateNutritionSummary(
        dailyNutritionOverviewsQuery.data,
        overviewType,
        new Date(),
      ),
    );
  }, [dailyNutritionOverviewsQuery.data, overviewType]);

  const handleOverviewChange = (type: OverviewType) => {
    setOverviewType(type);
  };

  if (dailyNutritionOverviewsQuery.isLoading) {
    return <div>Loading...</div>;
  }

  if (dailyNutritionOverviewsQuery.isError) {
    return <div>Error</div>;
  }

  if (dailyNutritionOverviewsQuery.data?.length === 0) {
    return <NutrientsCharts.Empty />;
  }

  return (
    <>
      <div className="mb-6 flex flex-wrap items-center justify-between gap-4">
        <div className="flex gap-4">
          <Button
            onClick={() => handleOverviewChange("day")}
            variant={overviewType === "day" ? "default" : "outline"}
          >
            Today
          </Button>
          <Button
            onClick={() => handleOverviewChange("week")}
            variant={overviewType === "week" ? "default" : "outline"}
          >
            This week
          </Button>
          <Button
            onClick={() => handleOverviewChange("month")}
            variant={overviewType === "month" ? "default" : "outline"}
          >
            This month
          </Button>
        </div>
      </div>
      <div className="@5xl:grid-cols-4 @2xl:grid-cols-2 grid gap-4">
        <NutrientCard
          title="Calories"
          current={overviewData.calories.value}
          target={overviewData.calories.target}
          unit="kcal"
          color={colors.blue}
          overviewType={overviewType}
        />
        <NutrientCard
          title="Carbs"
          current={overviewData.carbs.value}
          target={overviewData.carbs.target}
          unit="g"
          color={colors.green}
          overviewType={overviewType}
        />
        <NutrientCard
          title="Proteins"
          current={overviewData.proteins.value}
          target={overviewData.proteins.target}
          unit="g"
          color={colors.violet}
          overviewType={overviewType}
        />
        <NutrientCard
          title="Fats"
          current={overviewData.fats.value}
          target={overviewData.fats.target}
          unit="g"
          color={colors.yellow}
          overviewType={overviewType}
        />
      </div>
    </>
  );
}
NutrientsCharts.Empty = function Empty() {
  return <div>No data</div>;
};

export default NutrientsCharts;
