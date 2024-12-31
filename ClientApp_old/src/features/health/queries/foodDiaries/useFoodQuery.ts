import { useQuery } from "@tanstack/react-query";
import { FoodsApi } from "~/services/openapi";
import { QUERY_KEYS } from "../data/queryKeys";

const foodsApi = new FoodsApi();

const useFoodQuery = (foodId: string) => {
  const foodQuery = useQuery({
    queryKey: [QUERY_KEYS.food, foodId],
    queryFn: () => foodsApi.getFoodById(foodId).then((res) => res.data),
  });
  return foodQuery;
};

export default useFoodQuery;
