import { useQuery } from "@tanstack/react-query";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { queryClient } from "~/queryClient";
import { NutritionDiariesApi } from "~/services/openapi";
import { DateOnly, getDateOnly } from "~/utils/date";
import { QUERY_KEYS } from "../data/queryKeys";

const nutritionDiariesApi = new NutritionDiariesApi();

type UseTotalCaloriesQueryProps = {
  startDate?: DateOnly;
  endDate?: DateOnly;
};

const useTotalCaloriesQuery = (props?: UseTotalCaloriesQueryProps) => {
  const todayDate = getDateOnly(new Date());

  const startDate = props?.startDate ?? todayDate;

  const endDate = props?.endDate ?? todayDate;

  const totalCaloriesQuery = useQuery({
    queryKey: [QUERY_KEYS.totalCalories],
    queryFn: () => {
      return nutritionDiariesApi
        .getTotalCaloriesByPeriod(startDate, endDate)
        .then((res) => res.data);
    },
  });

  const isPending = useDelayedLoading(totalCaloriesQuery.isPending);

  return { totalCaloriesQuery, isPending };
};

export const prefetchTotalCaloriesQuery = (
  props?: UseTotalCaloriesQueryProps,
) => {
  queryClient.prefetchQuery({
    queryKey: [QUERY_KEYS.totalCalories],
    queryFn: () => {
      return nutritionDiariesApi
        .getTotalCaloriesByPeriod(
          props?.startDate ?? getDateOnly(new Date()),
          props?.endDate ?? getDateOnly(new Date()),
        )
        .then((res) => res.data);
    },
  });
};

export const invalidateTotalCaloriesQuery = () => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.totalCalories],
  });
};

export const setTotalCaloriesQueryData = ({
  adjustment,
  invalidate = false,
}: {
  adjustment: number;
  invalidate?: boolean;
}) => {
  queryClient.setQueryData([QUERY_KEYS.totalCalories], (oldData: number) =>
    Math.trunc(oldData + adjustment),
  );

  if (invalidate) invalidateTotalCaloriesQuery();
};

export default useTotalCaloriesQuery;
