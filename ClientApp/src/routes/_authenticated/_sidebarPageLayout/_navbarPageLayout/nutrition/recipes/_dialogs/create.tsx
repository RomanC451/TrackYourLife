import { createFileRoute } from "@tanstack/react-router";
import { z } from "zod";

import RecipeDialog from "@/features/nutrition/recipes/components/recipeDialogs/RecipeDialog";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes/_dialogs/create",
)({
  component: RouteComponent,

  validateSearch: z.object({
    tab: z.enum(["details", "ingredients"]).default("details"),
  }),
});

function RouteComponent() {
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/nutrition/recipes",
  });

  return <RecipeDialog dialogType="create" onClose={navigateBackOrDefault} />;
}
