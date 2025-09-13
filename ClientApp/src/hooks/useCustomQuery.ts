import { useEffect, useState } from "react";
import { QueryKey, useQuery, UseQueryOptions } from "@tanstack/react-query";

import { ApiError } from "@/services/openapi/apiSettings";

import { useDelayedLoadingV2 } from "./useDelayedLoading";

export type PendingState = {
  isPending: boolean;
  isDelayedPending: boolean;
};

export function useCustomQuery<
  TQueryFnData = unknown,
  TError = ApiError,
  TData = TQueryFnData,
  TQueryKey extends QueryKey = QueryKey,
>({
  onFirstFetch,
  ...options
}: UseQueryOptions<TQueryFnData, TError, TData, TQueryKey> & {
  onFirstFetch?: () => void;
}) {
  const [isFirstFetch, setIsFirstFetch] = useState(true);

  const query = useQuery(options);

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
