import { useMemo, useState } from "react";

import {
  Carousel,
  CarouselContent,
  CarouselDots, // New import
  CarouselItem,
} from "@/components/ui/carousel";
// New import
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
import { useDateOnlyState } from "@/hooks/useDateOnly";
import withDate from "@/lib/with";

const FoodDiaryPage = () => {
  const [date, setDate] = useDateOnlyState();
  const [searchCategory, setSearchCategory] = useState<"foods" | "recipes">(
    "foods",
  );
  const { screenSize } = useAppGeneralStateContext();

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
      <div className="grid place-content-center gap-4 lg:grid-cols-2">
        {screenSize.width < screensEnum.lg ? (
          <Carousel className="w-[320px]">
            <CarouselContent>
              <CarouselItem className="pl-[21px]">
                <CaloriesGraph />
              </CarouselItem>
              <CarouselItem className="pl-[21px]">
                <MacroProgress />
              </CarouselItem>
            </CarouselContent>
            <CarouselDots className="mt-4" />
          </Carousel>
        ) : (
          <>
            <div className="flex w-full justify-center">
              <CaloriesGraph />
            </div>
            <div className="flex w-full justify-center">
              <MacroProgress />
            </div>
          </>
        )}
      </div>

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
};

export default FoodDiaryPage;
