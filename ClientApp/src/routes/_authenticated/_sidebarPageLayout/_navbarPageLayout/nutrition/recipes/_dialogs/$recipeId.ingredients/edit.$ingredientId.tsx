import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

import EditIngredientDialog from "@/features/nutrition/ingredients/components/ingredientDialogs/EditIngredientDialog";
import { recipesQueryOptions } from "@/features/nutrition/recipes/queries/useRecipeQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes/_dialogs/$recipeId/ingredients/edit/$ingredientId",
)({
  component: RouteComponent,

  loader: async ({ params }) => {
    await queryClient.ensureQueryData(
      recipesQueryOptions.byId(params.recipeId),
    );
  },
});

function RouteComponent() {
  const { recipeId, ingredientId } = Route.useParams();

  const { data: recipe } = useSuspenseQuery(recipesQueryOptions.byId(recipeId));

  const ingredient = recipe.ingredients.find((i) => i.id === ingredientId);

  if (!ingredient) {
    throw new Error("Ingredient not found");
  }

  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/nutrition/recipes/edit/$recipeId",
    params: { recipeId },
  });

  return (
    <EditIngredientDialog
      recipe={recipe}
      ingredient={ingredient}
      onClose={navigateBackOrDefault}
      onSuccess={navigateBackOrDefault}
    />
  );
}
