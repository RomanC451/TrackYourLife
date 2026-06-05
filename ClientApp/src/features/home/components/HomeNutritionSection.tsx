import { Link } from "@tanstack/react-router";

import { Button } from "@/components/ui/button";
import { colors } from "@/constants/tailwindColors";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { getDateOnly } from "@/lib/date";
import { createEmptyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { NutrientCard } from "@/features/nutrition/overview/components/NutrientCard";
import { dailyNutritionOverviewTodaySumQueryOptions } from "@/features/nutrition/overview/queries/useDailyNutritionOverviewsQuery";
import type { DailyNutritionOverviewDto } from "@/services/openapi";

import HomeSection from "./HomeSection";

function HomeNutritionSection() {
  const today = getDateOnly(new Date());

  const { query: dailyNutritionOverviewsQuery } = useCustomQuery(
    dailyNutritionOverviewTodaySumQueryOptions(),
  );

  if (
    dailyNutritionOverviewsQuery.data &&
    dailyNutritionOverviewsQuery.data.length > 1
  ) {
    return (
      <HomeSection title="Nutrition" description="Today's intake">
        <p className="text-sm text-destructive">Could not load nutrition overview.</p>
      </HomeSection>
    );
  }

  if (dailyNutritionOverviewsQuery.data === undefined) {
    return (
      <HomeSection title="Nutrition" description="Today's intake">
        <p className="text-sm text-muted-foreground">Loading...</p>
      </HomeSection>
    );
  }

  let overviewData: DailyNutritionOverviewDto;
  if (dailyNutritionOverviewsQuery.data.length === 0) {
    overviewData = {
      isLoading: false,
      isDeleting: false,
      id: "",
      startDate: today,
      endDate: today,
      nutritionalContent: createEmptyNutritionalContent(),
      caloriesGoal: 1,
      carbohydratesGoal: 1,
      proteinGoal: 1,
      fatGoal: 1,
    };
  } else {
    overviewData = dailyNutritionOverviewsQuery.data[0];
  }

  const hasNoDiaryEntries = dailyNutritionOverviewsQuery.data.length === 0;

  return (
    <HomeSection
      title="Nutrition"
      description="Today's intake"
      action={
        <Button variant="outline" size="sm" asChild>
          <Link to="/nutrition/diary">Food diary</Link>
        </Button>
      }
    >
      <div className="@container/nutrition relative min-h-[min(320px,50vh)] overflow-hidden grid grid-cols-2 gap-4 @3xl/nutrition:grid-cols-4">
        <NutrientCard
          title="Calories"
          currentValue={overviewData.nutritionalContent.energy.value}
          targetValue={overviewData.caloriesGoal}
          unit="kcal"
          color={colors.blue}
          overviewType="Daily"
          isLoading={dailyNutritionOverviewsQuery.isFetching}
        />
        <NutrientCard
          title="Carbs"
          currentValue={overviewData.nutritionalContent.carbohydrates}
          targetValue={overviewData.carbohydratesGoal}
          unit="g"
          color={colors.green}
          overviewType="Daily"
          isLoading={dailyNutritionOverviewsQuery.isFetching}
        />
        <NutrientCard
          title="Proteins"
          currentValue={overviewData.nutritionalContent.protein}
          targetValue={overviewData.proteinGoal}
          unit="g"
          color={colors.violet}
          overviewType="Daily"
          isLoading={dailyNutritionOverviewsQuery.isFetching}
        />
        <NutrientCard
          title="Fats"
          currentValue={overviewData.nutritionalContent.fat}
          targetValue={overviewData.fatGoal}
          unit="g"
          color={colors.yellow}
          overviewType="Daily"
          isLoading={dailyNutritionOverviewsQuery.isFetching}
        />
        {hasNoDiaryEntries ? (
          <div className="absolute inset-0 flex flex-col items-center gap-4 bg-background/80 pt-[10%] backdrop-blur-sm">
            <p className="text-center text-xl font-semibold text-muted-foreground">
              No nutrition diary entries.
              <br />
              Log your food to see your nutrition overview.
            </p>
            <Button asChild>
              <Link to="/nutrition/diary">Log your food</Link>
            </Button>
          </div>
        ) : null}
      </div>
    </HomeSection>
  );
}

export default HomeNutritionSection;
