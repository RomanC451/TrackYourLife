import { useEffect, useState } from "react";
import {
  QueryKey,
  useInfiniteQuery,
  UseInfiniteQueryOptions,
} from "@tanstack/react-query";

import { ApiError } from "@/services/openapi/apiSettings";

import { useDelayedLoadingV2 } from "./useDelayedLoading";

export function useCustomInfiniteQuery<
  TQueryFnData = unknown,
  TError = ApiError,
  TData = TQueryFnData,
  TQueryKey extends QueryKey = QueryKey,
>({
  onFirstFetch,
  ...options
}: UseInfiniteQueryOptions<TQueryFnData, TError, TData, TQueryKey, number> & {
  onFirstFetch?: () => void;
}) {
  const [isFirstFetch, setIsFirstFetch] = useState(true);

  const query = useInfiniteQuery(options);

  const { data } = query;

  useEffect(() => {
    if (isFirstFetch && data) {
      onFirstFetch?.();
      setIsFirstFetch(false);
    }
  }, [data, isFirstFetch, setIsFirstFetch, onFirstFetch]);

  const isDelayedPending = useDelayedLoadingV2(query.isPending);

  return {
    query,
    isDelayedPending,
    pendingState: {
      isPending: query.isPending,
      isDelayedPending,
    },
  };
}
