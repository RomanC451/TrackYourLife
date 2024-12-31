import { createFileRoute } from "@tanstack/react-router";
import RecipesTab from "~/features/health/components/recipes/RecipesTab";

export const Route = createFileRoute(
  "/_sideAndNavBarsPageLayout/nutrition/recipes",
)({
  component: () => <RecipesTab />,
  loader: () => {
    // prefetchRecipesQuery();
  },
});
