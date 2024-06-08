import { useState } from "react";
import { ApiError, toastDefaultServerError } from "~/data/apiSettings";

import { useInfiniteQuery } from "@tanstack/react-query";

import { StatusCodes } from "http-status-codes";
import { FoodApi } from "~/services/openapi";

const REQUEST_MAX_RETRY_COUNT = 3;

const foodApi = new FoodApi();

const useFoodSearch = () => {
  const [resultsTableOpened, setResultsTableOpened] = useState(false);

  const [searchValue, setSearchValue] = useState("");
  const [searchError, setSearchError] = useState("");

  const searchQuery = useInfiniteQuery({
    queryKey: ["foodSearch", searchValue],
    queryFn: ({ pageParam }) =>
      foodApi.getList(searchValue, pageParam, 10).then((res) => res.data),
    enabled: searchValue.length > 0,
    retry: (failureCount, error: ApiError) => {
      const errorObj = error.response?.data;

      if (!errorObj) {
        toastDefaultServerError();
        return false;
      }

      if (errorObj.status === StatusCodes.NOT_FOUND) {
        setSearchError("No results found");
        return false;
      }

      if (failureCount >= REQUEST_MAX_RETRY_COUNT) {
        if (errorObj.status === StatusCodes.INTERNAL_SERVER_ERROR)
          setSearchError("Server error");
        toastDefaultServerError();
        return false;
      }

      return true;
    },
    getNextPageParam: (lastPage?) =>
      lastPage?.items.hasNextPage ? lastPage.items.page + 1 : undefined,
    initialPageParam: 1,
  });

  return {
    searchQuery,
    setSearchValue,
    searchValue,
    searchError,
    setSearchError,
    resultsTableOpened,
    setResultsTableOpened,
  };
};
export default useFoodSearch;
