import { Skeleton } from "~/chadcn/ui/skeleton";
import { RecipeDto } from "~/services/openapi";

type RecipesListElementProps = {
  recipe: RecipeDto;
  AddRecipeButton: React.ComponentType<{
    recipe: RecipeDto;
    className?: string;
  }>;
  AddRecipeDialog: React.ComponentType<{ recipe: RecipeDto }>;
};

function RecipesListElement({
  recipe,
  AddRecipeButton,
  AddRecipeDialog,
}: RecipesListElementProps): JSX.Element {
  return (
    <div className="relative">
      <AddRecipeDialog recipe={recipe} />
      <AddRecipeButton
        recipe={recipe}
        className="absolute right-2 top-[50%] translate-y-[-50%]"
      />
    </div>
  );
}

RecipesListElement.Loading = function RecipesListElementLoading() {
  return (
    <div className="relative h-16 p-2">
      <div className="w-full">
        <Skeleton className="h-5 w-[100px]" />
        <Skeleton className="mt-2 h-4 w-[200px]" />
      </div>
      <Skeleton className="absolute right-2 top-[50%] h-[40px] w-[40px] translate-y-[-50%]" />
    </div>
  );
};

export default RecipesListElement;
