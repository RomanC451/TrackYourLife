import React, { useState } from "react";

import { ToggleGroup, ToggleGroupItem } from "@/components/ui/toggle-group";
import FoodSearch from "@/features/nutrition/common/components/FoodSearch";
import NutritionTabCard from "@/features/nutrition/common/components/NutritionTabCard";
import CaloriesGraph from "@/features/nutrition/diary/components/CaloriesGraph";
import FoodDiaryTable from "@/features/nutrition/diary/components/diaryTable/FoodDiaryTable";
import AddFoodDiaryEntryDialog from "@/features/nutrition/diary/components/foodDiaryEntryDialogs/AddFoodDiaryEntryDialog";
import AddFoodDiaryEntryButton from "@/features/nutrition/diary/components/foodSearch/AddFoodDiaryEntryButton";
import AddRecipeDiaryEntryDialog from "@/features/nutrition/diary/components/recipeDiaryEntryDialogs/AddRecipeDiaryEntryDialog";
import AddRecipeDiaryEntryButton from "@/features/nutrition/diary/components/recipesSearch/AddRecipeDiaryEntryButton";
import RecipeSearch from "@/features/nutrition/diary/components/recipesSearch/RecipeSearch";
import { useDateOnlyState } from "@/hooks/useDateOnly";
import withDate from "@/lib/with";

const FoodDiaryPage: React.FC = (): JSX.Element => {
  const [date, setDate] = useDateOnlyState();

  const [searchCategory, setSearchCategory] = useState<"foods" | "recipes">(
    "foods",
  );

  return (
    <NutritionTabCard>
      <CaloriesGraph />
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

      {searchCategory === "foods" ? (
        <FoodSearch
          AddFoodButton={withDate(AddFoodDiaryEntryButton, date)}
          AddFoodDialog={withDate(AddFoodDiaryEntryDialog, date)}
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
