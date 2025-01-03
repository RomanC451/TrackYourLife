import { createFileRoute } from "@tanstack/react-router";

import FoodDiaryPage from "@/pages/nutrition/FoodDiaryPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary",
)({
  component: FoodDiaryPage,
});
