import { useQuery } from "@tanstack/react-query";

import { QUERY_KEYS } from "@/features/nutrition/common/data/queryKeys";
import useDelayedLoading from "@/hooks/useDelayedLoading";
import { DateOnly, getDateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import { NutritionalContent, NutritionDiariesApi } from "@/services/openapi";

import { addNutritionalContent } from "../../common/utils/nutritionalContent";

const nutritionDiariesApi = new NutritionDiariesApi();

type UseNutritionOverviewQueryProps = {
  startDate?: DateOnly;
  endDate?: DateOnly;
};

const useNutritionOverviewQuery = (props?: UseNutritionOverviewQueryProps) => {
  const todayDate = getDateOnly(new Date());

  const startDate = props?.startDate ?? todayDate;

  const endDate = props?.endDate ?? todayDate;

  const nutritionOverviewQuery = useQuery({
    queryKey: [QUERY_KEYS.totalCalories],
    queryFn: () => {
      return nutritionDiariesApi
        .getNutritionOverviewByPeriod(startDate, endDate)
        .then((res) => res.data);
    },
  });

  const isPending = useDelayedLoading(nutritionOverviewQuery.isPending);

  return { nutritionOverviewQuery, isPending };
};

export const prefetchNutritionOverviewQuery = (
  props?: UseNutritionOverviewQueryProps,
) => {
  queryClient.prefetchQuery({
    queryKey: [QUERY_KEYS.totalCalories],
    queryFn: () => {
      return nutritionDiariesApi
        .getNutritionOverviewByPeriod(
          props?.startDate ?? getDateOnly(new Date()),
          props?.endDate ?? getDateOnly(new Date()),
        )
        .then((res) => res.data);
    },
  });
};

export const invalidateNutritionOverviewQuery = () => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.totalCalories],
  });
};

export const setNutritionOverviewQueryData = ({
  adjustment,
  invalidate = false,
}: {
  adjustment: NutritionalContent;
  invalidate?: boolean;
}) => {
  queryClient.setQueryData(
    [QUERY_KEYS.totalCalories],
    (oldData: NutritionalContent) => addNutritionalContent(oldData, adjustment),
  );
  if (invalidate) invalidateNutritionOverviewQuery();
};

export default useNutritionOverviewQuery;
