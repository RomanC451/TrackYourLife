import { useState } from "react";

import { queryClient } from "@/queryClient";

import "@/services/openapi/apiSettings";

import { useCustomInfiniteQuery } from "@/hooks/useCustomInfiniteQuery";

import { QUERY_KEYS } from "../data/queryKeys";
import { foodQueryOptions } from "./useFoodQuery";

const useFoodSearch = () => {
  const [searchValue, setSearchValue] = useState("");
  const [error, setError] = useState("");

  const { query: searchQuery, pendingState } = useCustomInfiniteQuery(
    foodQueryOptions.search(searchValue, setError),
  );

  return {
    searchQuery,
    setSearchValue,
    searchValue,
    error,
    resetError: () => setError(""),
    pendingState,
  };
};

export function invalidateFoodSearchQuery() {
  queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.foodsSearch] });
}

export function removeFoodSearchQuery(searchTerm?: string) {
  queryClient.removeQueries({
    queryKey: searchTerm
      ? [QUERY_KEYS.foodsSearch, searchTerm]
      : [QUERY_KEYS.foodsSearch],
  });
}

export default useFoodSearch;
