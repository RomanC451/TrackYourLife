import React, { useState } from "react";
import CaloriesGraph from "~/features/health/components/calories/CaloriesGraph";

import { ToggleGroup, ToggleGroupItem } from "~/chadcn/ui/toggle-group";
import { useDateOnlyState } from "~/utils/date";
import withDate from "~/utils/with";
import FoodSearch from "../foodSearch/FoodSearch";
import NutritionTabCard from "../NutritionTabCard";
import RecipeSearch from "../recipeSearch/RecipeSearch";
import AddFoodDiaryEntryButton from "./AddFoodDiaryEntryButton";
import AddRecipeDiaryEntryButton from "./AddRecipeDiaryEntryButton";
import AddFoodDiaryEntryDialog from "./foodDIaryDialogs/AddFoodDiaryEntryDialog";
import AddRecipeDiaryEntryDialog from "./foodDIaryDialogs/AddRecipeDiaryEntryDialog";
import FoodDiaryTable from "./foodDiaryTable/FoodDiaryTable";

const FoodDiaryTab: React.FC = (): JSX.Element => {
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

export default FoodDiaryTab;
