import { LoadingContextProvider } from "@/contexts/LoadingContext";
import NutritionTabCard from "@/features/nutrition/common/components/NutritionTabCard";
import RecipesTable from "@/features/nutrition/recipes/components/recipesTable/RecipesTable";
import { RecipesTableContextProvider } from "@/features/nutrition/recipes/contexts/RecipesTableContext";

function RecipesPage() {
  return (
    <NutritionTabCard>
      <LoadingContextProvider>
        <RecipesTableContextProvider>
          <RecipesTable />
        </RecipesTableContextProvider>
      </LoadingContextProvider>
    </NutritionTabCard>
  );
}

export default RecipesPage;
