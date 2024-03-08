import { useRef, useState } from "react";
import { getErrorObject } from "~/data/apiSettings";
import { useApiRequests } from "~/hooks/useApiRequests";

import { useInfiniteQuery } from "@tanstack/react-query";

import { getFoodListRequest } from "../requests/";

const useFoodSearch = () => {
  const [resultsTableOpened, setResultsTableOpened] = useState(false);

  const { fetchRequest } = useApiRequests();

  const requestControllerRef = useRef<AbortController | undefined>();

  const [searchValue, setSearchValue] = useState("");
  const [searchError, setSearchError] = useState("");

  const searchQuery = useInfiniteQuery({
    queryKey: ["foodSearch", searchValue],
    queryFn: ({ pageParam }) => {
      const obj = getFoodListRequest({
        fetchRequest,
        controllerRef: requestControllerRef,
        setSearchError,
        searchParam: searchValue,
        page: pageParam
      });

      return obj;
    },
    enabled: searchValue.length > 0,
    retry: (_, error) => {
      const { status: errorStatus }: { status: number } = getErrorObject(error);
      if (errorStatus === 404) {
        return false;
      }
      return true;
    },
    getNextPageParam: (lastPage?) =>
      lastPage?.foodList.hasNextPage ? lastPage.foodList.page + 1 : undefined,
    initialPageParam: 1
  });

  return {
    searchQuery,
    setSearchValue,
    searchValue,
    searchError,
    setSearchError,
    resultsTableOpened,
    setResultsTableOpened
  };
};
export default useFoodSearch;
