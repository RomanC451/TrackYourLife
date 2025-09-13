import { infiniteQueryOptions, queryOptions } from "@tanstack/react-query";

import { FoodsApi } from "@/services/openapi";
import { retryQueryExcept404 } from "@/services/openapi/retry";

const foodsApi = new FoodsApi();

const PAGE_SIZE = 10;

export const foodQueryKeys = {
  all: ["foods"] as const,
  byId: (id: string) => [...foodQueryKeys.all, id] as const,
  search: (searchTerm: string) =>
    [...foodQueryKeys.all, "search", searchTerm] as const,
};

export const foodQueryOptions = {
  byId: (id: string) =>
    queryOptions({
      queryKey: foodQueryKeys.byId(id),
      queryFn: () => foodsApi.getFoodById(id).then((res) => res.data),
    }),
  search: (searchTerm: string, setError: (error: string) => void) =>
    infiniteQueryOptions({
      queryKey: foodQueryKeys.search(searchTerm),
      queryFn: ({ pageParam }) =>
        foodsApi
          .searchFoodsByName(searchTerm, pageParam, PAGE_SIZE)
          .then((res) => res.data),
      retry: (failureCount, error) =>
        retryQueryExcept404(failureCount, error, {
          notFoundCallback: () => {
            setError("No foods found");
          },
        }),

      getNextPageParam: (lastPage?) =>
        lastPage?.hasNextPage ? lastPage.page + 1 : undefined,
      initialPageParam: 1,
    }),
};
