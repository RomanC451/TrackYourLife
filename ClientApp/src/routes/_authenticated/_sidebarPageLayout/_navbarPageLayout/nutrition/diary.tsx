import { createFileRoute } from '@tanstack/react-router'

import { prefetchNutritionGoalQueryQuery } from '@/features/nutrition/diary/queries/useNutritionGoalQueries'
import { prefetchNutritionOverviewQuery } from '@/features/nutrition/diary/queries/useNutritionOverviewQuery'
import FoodDiaryPage from '@/pages/nutrition/FoodDiaryPage'

export const Route = createFileRoute(
  '/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary',
)({
  loader: async () => {
    await prefetchNutritionGoalQueryQuery()
    await prefetchNutritionOverviewQuery()
  },

  component: FoodDiaryPage,
})
