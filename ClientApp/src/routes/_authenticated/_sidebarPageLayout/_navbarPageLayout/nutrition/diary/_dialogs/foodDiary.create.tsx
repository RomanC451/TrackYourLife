import { createFileRoute, useSearch } from "@tanstack/react-router";
import z from "zod";

import { foodQueryOptions } from "@/features/nutrition/common/queries/useFoodQuery";
import { CreateFoodDiaryDialog } from "@/features/nutrition/diary/components/foodDiaryDialogs/CreateFoodDiaryDialog";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary/_dialogs/foodDiary/create",
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
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: "/nutrition/diary",
  });

  const { foodId } = useSearch({
    from: "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary/_dialogs/foodDiary/create",
  });

  return (
    <CreateFoodDiaryDialog
      foodId={foodId}
      onSuccess={() => {
        navigateBackOrDefault();
      }}
      onClose={() => navigateBackOrDefault()}
    />
  );
}
