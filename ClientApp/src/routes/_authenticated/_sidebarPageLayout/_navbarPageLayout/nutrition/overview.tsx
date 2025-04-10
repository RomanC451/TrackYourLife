import { createFileRoute } from '@tanstack/react-router'
import { endOfYear, startOfYear } from 'date-fns'

import { QUERY_KEYS } from '@/features/nutrition/common/data/queryKeys'
import { getDateOnly } from '@/lib/date'
import OverviewPage from '@/pages/nutrition/OverviewPage'
import { queryClient } from '@/queryClient'
import { DailyNutritionOverviewsApi } from '@/services/openapi'

const dailyNutritionOverviewsApi = new DailyNutritionOverviewsApi()

export const Route = createFileRoute(
  '/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/overview',
)({
  loader: () => {
    const startDate = getDateOnly(startOfYear(new Date()))
    const endDate = getDateOnly(endOfYear(new Date()))

    queryClient.prefetchQuery({
      queryKey: [QUERY_KEYS.dailyNutritionOverviews, startDate, endDate],
      queryFn: () =>
        dailyNutritionOverviewsApi
          .getDailyNutritionOverviewsByDateRange(startDate, endDate)
          .then((res) => res.data),
    })
  },
  component: OverviewPage,
})
