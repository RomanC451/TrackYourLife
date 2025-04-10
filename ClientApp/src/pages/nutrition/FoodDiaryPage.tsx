import { useMemo, useState } from "react";

import {
  Carousel,
  CarouselContent,
  CarouselDots,
  CarouselItem,
} from "@/components/ui/carousel";
import { ToggleGroup, ToggleGroupItem } from "@/components/ui/toggle-group";
import { screensEnum } from "@/constants/tailwindSizes";
import { useAppGeneralStateContext } from "@/contexts/AppGeneralContextProvider";
import FoodSearch from "@/features/nutrition/common/components/FoodSearch";
import NutritionTabCard from "@/features/nutrition/common/components/NutritionTabCard";
import CaloriesGraph from "@/features/nutrition/diary/components/CaloriesGraph";
import FoodDiaryTable from "@/features/nutrition/diary/components/diaryTable/FoodDiaryTable";
import FitnessCalculator from "@/features/nutrition/diary/components/fitnessCalculator/FitnessCalculator";
import AddFoodDiaryEntryDialog from "@/features/nutrition/diary/components/foodDiaryEntryDialogs/AddFoodDiaryEntryDialog";
import AddFoodDiaryEntryButton from "@/features/nutrition/diary/components/foodSearch/AddFoodDiaryEntryButton";
import MacroProgress from "@/features/nutrition/diary/components/MacrosProgress";
import AddRecipeDiaryEntryDialog from "@/features/nutrition/diary/components/recipeDiaryEntryDialogs/AddRecipeDiaryEntryDialog";
import AddRecipeDiaryEntryButton from "@/features/nutrition/diary/components/recipesSearch/AddRecipeDiaryEntryButton";
import RecipeSearch from "@/features/nutrition/diary/components/recipesSearch/RecipeSearch";
import useNutritionGoalsQuery from "@/features/nutrition/diary/queries/useNutritionGoalQueries";
import useNutritionOverviewQuery from "@/features/nutrition/diary/queries/useNutritionOverviewQuery";
import { useDateOnlyState } from "@/hooks/useDateOnly";
import { DateOnly } from "@/lib/date";
import { cn } from "@/lib/utils";
import withDate from "@/lib/with";

function FoodDiaryPage() {
  const [date, setDate] = useDateOnlyState();
  const [searchCategory, setSearchCategory] = useState<"foods" | "recipes">(
    "foods",
  );

  const memoizedAddFoodButton = useMemo(
    () => withDate(AddFoodDiaryEntryButton, date),
    [date],
  );
  const memoizedAddFoodDialog = useMemo(
    () => withDate(AddFoodDiaryEntryDialog, date),
    [date],
  );

  return (
    <NutritionTabCard>
      <NutritionTabCardHeader date={date}></NutritionTabCardHeader>

      <div className="flex justify-between">
        <div className="flex h-10 w-full">
          <ToggleGroup
            type="single"
            value={searchCategory}
            onValueChange={(value) => {
              if (value) setSearchCategory(value as "foods" | "recipes");
            }}
          >
            <ToggleGroupItem value="foods" aria-label="Toggle bold">
              Foods
            </ToggleGroupItem>
            <ToggleGroupItem value="Recipes" aria-label="Toggle italic">
              Recipes
            </ToggleGroupItem>
          </ToggleGroup>
        </div>
        <FitnessCalculator />
      </div>

      {searchCategory === "foods" ? (
        <FoodSearch
          AddFoodButton={memoizedAddFoodButton}
          AddFoodDialog={memoizedAddFoodDialog}
        />
      ) : (
        <RecipeSearch
          AddRecipeButton={withDate(AddRecipeDiaryEntryButton, date)}
          AddRecipeDialog={withDate(AddRecipeDiaryEntryDialog, date)}
        />
      )}

      <FoodDiaryTable date={date} setDate={setDate} />
    </NutritionTabCard>
  );
}

function NutritionTabCardHeader({ date }: Readonly<{ date: DateOnly }>) {
  const { screenSize } = useAppGeneralStateContext();

  const { nutritionOverviewQuery } = useNutritionOverviewQuery({
    endDate: date,
    startDate: date,
  });

  const { goals, goalsAreNotDefined } = useNutritionGoalsQuery(date);

  console.log(goalsAreNotDefined);

  return (
    <div className="relative">
      <div
        className={cn(
          "absolute left-0 top-0 z-10 flex h-full w-full flex-col items-center justify-center gap-4",
          goalsAreNotDefined ? "z-10" : "-z-10 hidden",
        )}
      >
        <p>Goals are not defined. Calculate them first.</p>
        <FitnessCalculator buttonText="Calculate goals" />
      </div>

      <div
        className={cn(
          "grid place-content-center gap-4 lg:grid-cols-2",
          goalsAreNotDefined ? "blur-xl" : null,
        )}
      >
        {screenSize.width < screensEnum.lg ? (
          <Carousel className="w-[320px]">
            <CarouselContent>
              <CarouselItem className="pl-[21px]">
                <CaloriesGraph
                  goals={goals}
                  nutritionOverview={nutritionOverviewQuery.data}
                />
              </CarouselItem>
              <CarouselItem className="pl-[21px]">
                <MacroProgress
                  goals={goals}
                  nutritionOverview={nutritionOverviewQuery.data}
                  isLoading={nutritionOverviewQuery.isPending}
                />
              </CarouselItem>
            </CarouselContent>
            <CarouselDots className="mt-4" />
          </Carousel>
        ) : (
          <>
            <div className="flex w-full justify-center">
              <CaloriesGraph
                goals={goals}
                nutritionOverview={nutritionOverviewQuery.data}
              />
            </div>
            <div className="flex w-full justify-center">
              <MacroProgress
                goals={goals}
                nutritionOverview={nutritionOverviewQuery.data}
                isLoading={nutritionOverviewQuery.isPending}
              />
            </div>
          </>
        )}
      </div>
    </div>
  );
}

export default FoodDiaryPage;
