import { useState } from "react";

import { queryClient } from "@/queryClient";

import "@/services/openapi/apiSettings";

import { useCustomInfiniteQuery } from "@/hooks/useCustomInfiniteQuery";

import { foodQueryKeys, foodQueryOptions } from "./useFoodQuery";

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
  queryClient.invalidateQueries({ queryKey: foodQueryKeys.searches() });
}

export function removeFoodSearchQuery(searchTerm?: string) {
  queryClient.removeQueries({
    queryKey: searchTerm
      ? foodQueryKeys.search(searchTerm)
      : foodQueryKeys.searches(),
  });
}

export default useFoodSearch;
