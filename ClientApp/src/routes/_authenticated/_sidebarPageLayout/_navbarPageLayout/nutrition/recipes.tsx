import { createFileRoute } from "@tanstack/react-router";

import RecipesPage from "@/pages/nutrition/RecipesPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes",
)({
  component: RecipesPage,
});
