import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import { z } from "zod";

import { foodQueryOptions } from "@/features/nutrition/common/queries/useFoodQuery";
import CreateIngredientDialog from "@/features/nutrition/ingredients/components/ingredientDialogs/CreateIngredientDialog";
import { recipesQueryOptions } from "@/features/nutrition/recipes/queries/useRecipeQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes/_dialogs/$recipeId/ingredients/create",
)({
  component: RouteComponent,
  validateSearch: z.object({
    foodId: z.string(),
  }),
  loaderDeps: ({ search }) => ({ foodId: search.foodId }),

  loader: async ({ deps: { foodId } }) => {
    await queryClient.ensureQueryData(foodQueryOptions.byId(foodId));
  },
});

function RouteComponent() {
  const { foodId } = Route.useSearch();
  const { recipeId } = Route.useParams();

  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/nutrition/recipes/edit/$recipeId",
    params: { recipeId },
  });

  const { data: recipe } = useSuspenseQuery(recipesQueryOptions.byId(recipeId));

  const { data: food } = useSuspenseQuery(foodQueryOptions.byId(foodId));

  return (
    <CreateIngredientDialog
      food={food}
      recipe={recipe}
      onClose={navigateBackOrDefault}
      onSuccess={navigateBackOrDefault}
    />
  );
}
