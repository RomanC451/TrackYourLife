import { createFileRoute, Outlet } from "@tanstack/react-router";

import { nutritionDiariesQueryOptions } from "@/features/nutrition/diary/queries/useDiaryQuery";
import { nutritionGoalsQueryOptions } from "@/features/nutrition/goals/queries/nutritionGoalsQuery";
import { recipesQueryOptions } from "@/features/nutrition/recipes/queries/useRecipeQuery";
import { getDateOnly } from "@/lib/date";
import FoodDiaryPage from "@/pages/nutrition/FoodDiaryPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary",
)({
  loader: async () => {
    const date = getDateOnly(new Date());

    await Promise.all([
      queryClient.ensureQueryData(nutritionDiariesQueryOptions.byDate(date)),
      queryClient.ensureQueryData(nutritionGoalsQueryOptions.byDate(date)),
      queryClient.ensureQueryData(
        nutritionDiariesQueryOptions.overview(date, date),
      ),
      queryClient.ensureQueryData(recipesQueryOptions.all),
    ]);
  },

  component: () => (
    <>
      <FoodDiaryPage />
      <Outlet />
    </>
  ),
});
