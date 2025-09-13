import { createFileRoute } from "@tanstack/react-router";
import z from "zod";

import EditRecipeDialog from "@/features/nutrition/recipes/components/recipeDialogs/EditRecipeDialog";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes/_dialogs/edit/$recipeId",
)({
  component: RouteComponent,
  validateSearch: z.object({
    tab: z.enum(["details", "ingredients"]).default("details"),
  }),
});

function RouteComponent() {
  const { recipeId } = Route.useParams();

  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/nutrition/recipes",
  });

  return (
    <EditRecipeDialog
      recipeId={recipeId}
      onClose={() => navigateBackOrDefault()}
    />
  );
}
