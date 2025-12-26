import { useMemo, useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { StatusCodes } from "http-status-codes";

import { router } from "@/App";
import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Card } from "@/components/ui/card";
import {
  Carousel,
  CarouselContent,
  CarouselDots,
  CarouselItem,
} from "@/components/ui/carousel";
import { ToggleGroup, ToggleGroupItem } from "@/components/ui/toggle-group";
import { screensEnum } from "@/constants/tailwindSizes";
import { useAppGeneralStateContext } from "@/contexts/AppGeneralContextProvider";
import FoodSearch from "@/features/nutrition/common/components/foodSearch/FoodSearch";
import { FoodSearchContextProvider } from "@/features/nutrition/common/components/foodSearch/useFoodSearchContext";
import FoodDiaryTable from "@/features/nutrition/diary/components/diaryTable/FoodDiaryTable";
import AddFoodDiaryEntryButton from "@/features/nutrition/diary/components/foodSearch/AddFoodDiaryEntryButton";
import MacroProgress from "@/features/nutrition/diary/components/MacrosProgress";
import AddRecipeDiaryEntryButton from "@/features/nutrition/diary/components/recipesSearch/AddRecipeDiaryEntryButton";
import RecipeSearch from "@/features/nutrition/diary/components/recipesSearch/RecipeSearch";
import { nutritionDiariesQueryOptions } from "@/features/nutrition/diary/queries/useDiaryQuery";
import CaloriesGraph from "@/features/nutrition/goals/components/CaloriesGraph";
import FitnessCalculator from "@/features/nutrition/goals/components/nutritionGoalsCalculator/FitnessCalculator";
import { nutritionGoalsQueryOptions } from "@/features/nutrition/goals/queries/nutritionGoalsQuery";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { useDateOnlyState } from "@/hooks/useDateOnly";
import { DateOnly } from "@/lib/date";
import { cn } from "@/lib/utils";
import withDate from "@/lib/with";

function FoodDiaryPage() {
  const [date, setDate] = useDateOnlyState();
  const [searchCategory, setSearchCategory] = useState<"foods" | "recipes">(
    "foods",
  );

  const navigate = useNavigate();

  const memoizedAddFoodButton = useMemo(
    () => withDate(AddFoodDiaryEntryButton, date),
    [date],
  );

  const memoizedAddRecipeButton = useMemo(
    () => withDate(AddRecipeDiaryEntryButton, date),
    [date],
  );

  return (
    <PageCard>
      <PageTitle title="Food Diary" />
      <Card className="mx-auto w-full max-w-[2000px] p-4">
        <NutritionTabCardHeader date={date}></NutritionTabCardHeader>
      </Card>
      <Card className="mx-auto w-full max-w-[2000px]">
        <div className="mx-auto space-y-4 p-4">
          <div className="flex justify-between">
            <div className="flex h-10 w-full">
              <ToggleGroup
                type="single"
                value={searchCategory}
                onValueChange={(value: string) => {
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
            <FoodSearchContextProvider>
              <FoodSearch
                addFoodButtonComponent={memoizedAddFoodButton}
                onSelectedFoodToOptions={{
                  to: "/nutrition/diary/foodDiary/create",
                }}
              />
            </FoodSearchContextProvider>
          ) : (
            <RecipeSearch
              AddRecipeButton={memoizedAddRecipeButton}
              onRecipeSelected={(recipe) => {
                navigate({
                  to: "/nutrition/diary/recipeDiary/create",
                  search: {
                    recipeId: recipe.id,
                  },
                });
              }}
              onHoverRecipe={(recipe) => {
                router.preloadRoute({
                  to: "/nutrition/diary/recipeDiary/create",
                  search: {
                    recipeId: recipe.id,
                  },
                });
              }}
              onTouchRecipe={(recipe) => {
                router.preloadRoute({
                  to: "/nutrition/diary/recipeDiary/create",
                  search: {
                    recipeId: recipe.id,
                  },
                });
              }}
            />
          )}
        </div>
      </Card>
      {/* <Card className="p-4"> */}
      <FoodDiaryTable date={date} setDate={setDate} />
      {/* </Card> */}
    </PageCard>
  );
}

function NutritionTabCardHeader({ date }: Readonly<{ date: DateOnly }>) {
  const { screenSize } = useAppGeneralStateContext();

  const {
    query: { data: nutritionOverview, isPending: nutritionOverviewIsPending },
  } = useCustomQuery(nutritionDiariesQueryOptions.overview(date, date));

  const {
    query: { data: goals, error: goalsError },
  } = useCustomQuery(nutritionGoalsQueryOptions.byDate(date));

  const goalsAreNotDefined =
    goalsError?.status === StatusCodes.NOT_FOUND || goals === undefined;

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
          <Carousel className="w-max">
            <CarouselContent className="">
              <CarouselItem className="flex justify-center pl-[21px]">
                <CaloriesGraph
                  goals={goals}
                  nutritionOverview={nutritionOverview}
                />
              </CarouselItem>
              <CarouselItem className="flex justify-center pl-[21px]">
                <MacroProgress
                  goals={goals}
                  nutritionOverview={nutritionOverview}
                  isLoading={nutritionOverviewIsPending}
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
                nutritionOverview={nutritionOverview}
              />
            </div>
            <div className="flex w-full justify-center">
              <MacroProgress
                goals={goals}
                nutritionOverview={nutritionOverview}
                isLoading={nutritionOverviewIsPending}
              />
            </div>
          </>
        )}
      </div>
    </div>
  );
}

export default FoodDiaryPage;
