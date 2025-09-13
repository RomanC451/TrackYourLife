import { createFileRoute, useSearch } from "@tanstack/react-router";
import z from "zod";

import { CreateRecipeDiaryDialog } from "@/features/nutrition/diary/components/recipeDiaryDialogs/CreateRecipeDiaryDialog";
import { recipesQueryOptions } from "@/features/nutrition/recipes/queries/useRecipeQuery";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary/_dialogs/recipeDiary/create",
)({
  component: RouteComponent,

  validateSearch: z.object({
    recipeId: z.string(),
  }),
  loaderDeps: ({ search }) => ({ recipeId: search.recipeId }),

  loader: async ({ deps: { recipeId } }) => {
    await queryClient.ensureQueryData(recipesQueryOptions.byId(recipeId));
  },
});

function RouteComponent() {
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/nutrition/diary",
  });

  const { recipeId } = useSearch({
    from: "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary/_dialogs/recipeDiary/create",
  });

  return (
    <CreateRecipeDiaryDialog
      recipeId={recipeId}
      onSuccess={() => {
        navigateBackOrDefault();
      }}
      onClose={() => navigateBackOrDefault()}
    />
  );
}
