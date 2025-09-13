import { useNavigate } from "@tanstack/react-router";
import { Pencil } from "lucide-react";

import { router } from "@/App";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import FoodListElementOverview from "@/features/nutrition/common/components/foodSearch/FoodListElementOverview";
import { multiplyNutritionalContent } from "@/features/nutrition/common/utils/nutritionalContent";
import { RecipeDto } from "@/services/openapi";

type RecipesListElementProps = {
  recipe: RecipeDto;
  AddRecipeButton: React.ComponentType<{
    recipe: RecipeDto;
    className?: string;
  }>;
  onRecipeSelected: (recipe: RecipeDto) => void;
  onHoverRecipe: (recipe: RecipeDto) => void;
  onTouchRecipe: (recipe: RecipeDto) => void;
};

function RecipesListElement({
  recipe,
  AddRecipeButton,
  onRecipeSelected,
  onHoverRecipe,
  onTouchRecipe,
}: RecipesListElementProps): JSX.Element {
  const navigate = useNavigate();
  return (
    <div className="relative">
      <button
        className="w-full"
        onClick={() => onRecipeSelected(recipe)}
        onMouseEnter={() => onHoverRecipe(recipe)}
        onTouchStart={() => onTouchRecipe(recipe)}
      >
        <FoodListElementOverview
          name={recipe.name}
          nutritionalContents={multiplyNutritionalContent(
            recipe.nutritionalContents,
            recipe.servingSizes[0].nutritionMultiplier,
          )}
        />
      </button>

      <Button
        variant="ghost"
        size="icon"
        className="absolute right-12 top-[50%] translate-y-[-50%]"
        onClick={() => {
          navigate({
            to: "/nutrition/recipes/edit/$recipeId",
            params: { recipeId: recipe.id },
          });
        }}
        onMouseEnter={() => {
          router.preloadRoute({
            to: "/nutrition/recipes/edit/$recipeId",
            params: { recipeId: recipe.id },
          });
        }}
        onTouchStart={() => {
          router.preloadRoute({
            to: "/nutrition/recipes/edit/$recipeId",
            params: { recipeId: recipe.id },
          });
        }}
      >
        <Pencil />
      </Button>

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
