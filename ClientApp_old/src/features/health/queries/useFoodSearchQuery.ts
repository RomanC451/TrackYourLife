import { useState } from "react";
import { ApiError, toastDefaultServerError } from "~/data/apiSettings";

import { useInfiniteQuery } from "@tanstack/react-query";

import { StatusCodes } from "http-status-codes";
import useDelayedLoading from "~/hooks/useDelayedLoading copy2";
import { queryClient } from "~/queryClient";
import { FoodsApi } from "~/services/openapi";
import { QUERY_KEYS } from "../data/queryKeys";

const REQUEST_MAX_RETRY_COUNT = 3;

const PAGE_SIZE = 10;

const foodsApi = new FoodsApi();

const useFoodSearchQuery = () => {
  const [searchValue, setSearchValue] = useState("");
  const [error, setError] = useState("");

  const searchQuery = useInfiniteQuery({
    queryKey: [QUERY_KEYS.foodSearch, searchValue],

    queryFn: ({ pageParam }) =>
      foodsApi
        .searchFoodsByName(searchValue, pageParam, PAGE_SIZE)
        .then((res) => res.data),
    retry: (failureCount, error: ApiError) => {
      const errorObj = error.response?.data;

      if (!errorObj) {
        toastDefaultServerError();
        return false;
      }

      if (errorObj.status === StatusCodes.NOT_FOUND) {
        setError("No results found");
        return false;
      }

      if (failureCount >= REQUEST_MAX_RETRY_COUNT) {
        if (errorObj.status === StatusCodes.INTERNAL_SERVER_ERROR)
          setError("Server error");
        toastDefaultServerError();
        return false;
      }

      return true;
    },
    getNextPageParam: (lastPage?) =>
      lastPage?.hasNextPage ? lastPage.page + 1 : undefined,
    initialPageParam: 1,
  });

  const isPending = useDelayedLoading(searchQuery.isPending);

  function resetError() {
    setError("");
  }

  return {
    searchQuery,
    setSearchValue,
    searchValue,
    error,
    resetError,
    isPending,
  };
};

export function removeFoodSearchQuery(searchTerm?: string) {
  queryClient.removeQueries({
    queryKey: searchTerm
      ? [QUERY_KEYS.foodSearch, searchTerm]
      : [QUERY_KEYS.foodSearch],
  });
}

export default useFoodSearchQuery;
