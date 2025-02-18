import { useQuery } from "@tanstack/react-query";

import { queryClient } from "@/queryClient";
import { FoodDiariesApi } from "@/services/openapi";

import { QUERY_KEYS } from "../../common/data/queryKeys";

const foodDiariesApi = new FoodDiariesApi();

const useFoodDiaryQuery = (foodDiaryId: string) => {
  const foodDiaryQuery = useQuery({
    queryKey: [QUERY_KEYS.foodDiaries, foodDiaryId],
    queryFn: () =>
      foodDiariesApi.getFoodDiaryById(foodDiaryId).then((res) => res.data),
  });
  return foodDiaryQuery;
};

export const invalidateFoodDiaryQuery = (foodDiaryId: string) => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.foodDiaries, foodDiaryId],
  });
};

export default useFoodDiaryQuery;
