import {
  useMutation,
  UseMutationOptions,
  UseMutationResult,
} from "@tanstack/react-query";

import { ApiError } from "@/services/openapi/apiSettings";

import { useDelayedLoadingV2 } from "./useDelayedLoading";

export type MutationPendingState = {
  isPending: boolean;
  isDelayedPending: boolean;
};

export type UseCustomMutationResult<TData, TError, TVariables, TContext> =
  UseMutationResult<TData, TError, TVariables, TContext> & {
    isDelayedPending: boolean;
    pendingState: MutationPendingState;
  };

export function useCustomMutation<
  TData,
  TVariables,
  TContext,
  TError = ApiError,
>(options: UseMutationOptions<TData, TError, TVariables, TContext>) {
  const mutation = useMutation(options);

  const isDelayedPending = useDelayedLoadingV2(mutation.isPending);

  return {
    ...mutation,
    isDelayedPending,
    pendingState: {
      isPending: mutation.isPending,
      isDelayedPending,
    },
  };
}
