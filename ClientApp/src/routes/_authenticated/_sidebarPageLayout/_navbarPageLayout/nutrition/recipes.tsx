import { createFileRoute, Outlet } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import RecipesTable from "@/features/nutrition/recipes/components/recipesTable/RecipesTable";
import { recipesQueryOptions } from "@/features/nutrition/recipes/queries/useRecipeQuery";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes",
)({
  component: RouteComponent,
  loader: () => {
    queryClient.ensureQueryData(recipesQueryOptions.all);
  },
});

function RouteComponent() {
  return (
    <PageCard>
      <RecipesTable />
      <Outlet />
    </PageCard>
  );
}
