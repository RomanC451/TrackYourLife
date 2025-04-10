import { useQuery } from "@tanstack/react-query";

import { QUERY_KEYS } from "@/features/nutrition/common/data/queryKeys";
import useDelayedLoading from "@/hooks/useDelayedLoading";
import { DateOnly, getDateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import { NutritionalContent, NutritionDiariesApi } from "@/services/openapi";

import { addNutritionalContent } from "../../common/utils/nutritionalContent";

const nutritionDiariesApi = new NutritionDiariesApi();

type UseNutritionOverviewQueryProps = {
  startDate: DateOnly;
  endDate: DateOnly;
};

const useNutritionOverviewQuery = ({
  startDate,
  endDate,
}: UseNutritionOverviewQueryProps) => {
  const nutritionOverviewQuery = useQuery({
    queryKey: [QUERY_KEYS.nutritionOverview, startDate, endDate],
    queryFn: () =>  nutritionDiariesApi
        .getNutritionOverviewByPeriod(startDate, endDate)
        .then((res) => res.data),
    
    placeholderData: (prev) => prev,
  });

  const isPending = useDelayedLoading(nutritionOverviewQuery.isLoading);

  return { nutritionOverviewQuery, isPending };
};

export const prefetchNutritionOverviewQuery = (
  startDate?: DateOnly,
  endDate?: DateOnly,
) => {
  const start = startDate ?? getDateOnly(new Date());
  const end = endDate ?? getDateOnly(new Date());

  return queryClient.prefetchQuery({
    queryKey: [QUERY_KEYS.nutritionOverview, start, end],
    queryFn: () => {
      return nutritionDiariesApi
        .getNutritionOverviewByPeriod(start, end)
        .then((res) => res.data);
    },
  });
};

export const invalidateNutritionOverviewQuery = (
  startDate?: DateOnly,
  endDate?: DateOnly,
) => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.nutritionOverview, startDate, endDate],
  });
};

export const setNutritionOverviewQueryData = ({
  adjustment,
  startDate,
  endDate,
  invalidate = false,
}: {
  adjustment: NutritionalContent;
  startDate: DateOnly;
  endDate: DateOnly;
  invalidate?: boolean;
}) => {
  queryClient.setQueryData(
    [QUERY_KEYS.nutritionOverview, startDate, endDate],
    (oldData: NutritionalContent) => addNutritionalContent(oldData, adjustment),
  );
  if (invalidate) invalidateNutritionOverviewQuery(startDate, endDate);
};

export default useNutritionOverviewQuery;
