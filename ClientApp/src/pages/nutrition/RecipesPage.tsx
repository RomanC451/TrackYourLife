import PageCard from "@/components/common/PageCard";
import { LoadingContextProvider } from "@/contexts/LoadingContext";
import RecipesTable from "@/features/nutrition/recipes/components/recipesTable/RecipesTable";
import { RecipesTableContextProvider } from "@/features/nutrition/recipes/contexts/RecipesTableContext";

function RecipesPage() {
  return (
    <PageCard>
      <LoadingContextProvider>
        <RecipesTableContextProvider>
          <RecipesTable />
        </RecipesTableContextProvider>
      </LoadingContextProvider>
    </PageCard>
  );
}

export default RecipesPage;
