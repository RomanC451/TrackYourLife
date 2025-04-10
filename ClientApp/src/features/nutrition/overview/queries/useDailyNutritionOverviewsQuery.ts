import { useQuery } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { DateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import { DailyNutritionOverviewsApi } from "@/services/openapi";

import { QUERY_KEYS } from "../../common/data/queryKeys";

const dailyNutritionOverviewsApi = new DailyNutritionOverviewsApi();

function useDailyNutritionOverviewsQuery(
  startDate: DateOnly,
  endDate: DateOnly,
) {
  const dailyNutritionOverviewsQuery = useQuery({
    queryKey: [QUERY_KEYS.dailyNutritionOverviews, startDate, endDate],
    queryFn: () =>
      dailyNutritionOverviewsApi
        .getDailyNutritionOverviewsByDateRange(startDate, endDate)
        .then((res) => res.data),
  });

  const isPending = useDelayedLoading(dailyNutritionOverviewsQuery.isPending);

  return { dailyNutritionOverviewsQuery, isPending };
}

export const invalidateDailyNutritionOverviewsQuery = () => {
  setTimeout(() =>
    queryClient.invalidateQueries({
      queryKey: [QUERY_KEYS.dailyNutritionOverviews],
    }),
  );
};

export default useDailyNutritionOverviewsQuery;
