import { useMemo, useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { endOfMonth, endOfWeek, startOfMonth, startOfWeek } from "date-fns";

import { Button } from "@/components/ui/button";
import { colors } from "@/constants/tailwindColors";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { getDateOnly } from "@/lib/date";
import { DailyNutritionOverviewDto, OverviewType } from "@/services/openapi";

import { createEmptyNutritionalContent } from "../../common/utils/nutritionalContent";
import { dailyNutritionOverviewsQueryOptions } from "../queries/useDailyNutritionOverviewsQuery";
import { NutrientCard } from "./NutrientCard";

function NutrientsCharts() {
  const [overviewType, setOverviewType] = useState<OverviewType>("Daily");
  const navigate = useNavigate();

  const startDate = useMemo(() => {
    if (overviewType === "Daily") return getDateOnly(new Date());
    else if (overviewType === "Weekly")
      return getDateOnly(startOfWeek(new Date(), { weekStartsOn: 1 }));
    else return getDateOnly(startOfMonth(new Date()));
  }, [overviewType]);
  const endDate = useMemo(() => {
    if (overviewType === "Daily") return getDateOnly(new Date());
    else if (overviewType === "Weekly")
      return getDateOnly(endOfWeek(new Date(), { weekStartsOn: 1 }));
    else return getDateOnly(endOfMonth(new Date()));
  }, [overviewType]);

  const { query: dailyNutritionOverviewsQuery } = useCustomQuery(
    dailyNutritionOverviewsQueryOptions.byDateRange(
      startDate,
      endDate,
      overviewType,
      "Sum",
    ),
  );

  const handleOverviewChange = (type: OverviewType) => {
    setOverviewType(type);
  };

  if (
    dailyNutritionOverviewsQuery.data &&
    dailyNutritionOverviewsQuery.data.length > 1
  ) {
    return <div>Error fetching nutrition overviews</div>;
  }

  if (dailyNutritionOverviewsQuery.data === undefined) {
    return <div>Loading...</div>;
  }
  let overviewData: DailyNutritionOverviewDto;
  if (dailyNutritionOverviewsQuery.data?.length === 0) {
    overviewData = {
      isLoading: false,
      isDeleting: false,
      id: "",
      startDate: startDate,
      endDate: endDate,
      nutritionalContent: createEmptyNutritionalContent(),
      caloriesGoal: 1,
      carbohydratesGoal: 1,
      proteinGoal: 1,
      fatGoal: 1,
    };
  } else {
    overviewData = dailyNutritionOverviewsQuery.data[0];
  }

  return (
    <>
      <div className="mb-6 flex flex-wrap items-center justify-between gap-4">
        <div className="flex gap-4">
          <Button
            onClick={() => handleOverviewChange("Daily")}
            variant={overviewType === "Daily" ? "default" : "outline"}
          >
            Today
          </Button>
          <Button
            onClick={() => handleOverviewChange("Weekly")}
            variant={overviewType === "Weekly" ? "default" : "outline"}
          >
            This week
          </Button>
          <Button
            onClick={() => handleOverviewChange("Monthly")}
            variant={overviewType === "Monthly" ? "default" : "outline"}
          >
            This month
          </Button>
        </div>
      </div>
      <div className="relative grid gap-4 @2xl:grid-cols-2 @5xl:grid-cols-4">
        <NutrientCard
          title="Calories"
          currentValue={overviewData.nutritionalContent.energy.value}
          targetValue={overviewData.caloriesGoal}
          unit="kcal"
          color={colors.blue}
          overviewType={overviewType}
          isLoading={dailyNutritionOverviewsQuery.isFetching}
        />

        <NutrientCard
          title="Carbs"
          currentValue={overviewData.nutritionalContent.carbohydrates}
          targetValue={overviewData.carbohydratesGoal}
          unit="g"
          color={colors.green}
          overviewType={overviewType}
          isLoading={dailyNutritionOverviewsQuery.isFetching}
        />
        <NutrientCard
          title="Proteins"
          currentValue={overviewData.nutritionalContent.protein}
          targetValue={overviewData.proteinGoal}
          unit="g"
          color={colors.violet}
          overviewType={overviewType}
          isLoading={dailyNutritionOverviewsQuery.isFetching}
        />
        <NutrientCard
          title="Fats"
          currentValue={overviewData.nutritionalContent.fat}
          targetValue={overviewData.fatGoal}
          unit="g"
          color={colors.yellow}
          overviewType={overviewType}
          isLoading={dailyNutritionOverviewsQuery.isFetching}
        />
        {dailyNutritionOverviewsQuery.data?.length === 0 && (
          <div className="absolute inset-0 flex flex-col items-center justify-center gap-4 bg-background/80 backdrop-blur-sm">
            <p className="text-center text-xl font-semibold text-muted-foreground">
              No nutrition diary entries.
              <br />
              Log your food to see your nutrition overview.
            </p>
            <Button onClick={() => navigate({ to: "/nutrition/diary" })}>
              Log your food
            </Button>
          </div>
        )}
      </div>
    </>
  );
}
NutrientsCharts.Empty = function Empty() {
  // !!! TODO: Implement the Empty component

  return <div>No nutrition diary entry this month.</div>;
};

NutrientsCharts.Loading = function Loading() {};

export default NutrientsCharts;
