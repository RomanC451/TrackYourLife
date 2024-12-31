import { createFileRoute } from "@tanstack/react-router";
import { FoodDiaryTab } from "~/features/health";
import { prefetchNutritionDiariesQuery } from "~/features/health/queries/foodDiaries/useNutritionDiariesQuery";
import { prefetchCaloriesGoalQuery } from "~/features/health/queries/useCaloriesGoalQuery";
import { prefetchTotalCaloriesQuery } from "~/features/health/queries/useTotalCaloriesQuery";
import { getDateOnly } from "~/utils/date";

export const Route = createFileRoute(
  "/_sideAndNavBarsPageLayout/nutrition/foodDiary",
)({
  pendingComponent: () => <div>Loading...</div>,
  loader: () => {
    prefetchNutritionDiariesQuery(getDateOnly(new Date()));
    prefetchCaloriesGoalQuery();
    prefetchTotalCaloriesQuery();
  },
  component: FoodDiaryTab,
});
