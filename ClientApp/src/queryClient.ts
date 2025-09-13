import { matchQuery, MutationCache, QueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

import { setManuallySet } from "./lib/dtoUtils";
import { toastDefaultServerError } from "./services/openapi/apiSettings";

function setManuallySet2(data: unknown): unknown {
  const result = setManuallySet(data);
  return result;
}

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5,
      retry: 3,
      select: setManuallySet2,
    },
  },
  mutationCache: new MutationCache({
    onError: (error, _variables, _context, mutation) => {
      if (import.meta.env.MODE === "development")
        toastDefaultServerError(error);
      else if (mutation.meta?.noDefaultErrorToast === false)
        toast.error("Something went wrong");

      const onErrorToast = mutation.meta?.onErrorToast;

      if (onErrorToast) {
        if (onErrorToast.type === "default") {
          toast(onErrorToast.message, onErrorToast.data);
        } else {
          toast[onErrorToast.type](onErrorToast.message, onErrorToast.data);
        }
      }
    },

    onSuccess: (_data, _variables, _context, mutation) => {
      const onSuccessToast = mutation.meta?.onSuccessToast;

      if (onSuccessToast) {
        if (onSuccessToast.type === "default") {
          toast(onSuccessToast.message, onSuccessToast.data);
        } else {
          toast[onSuccessToast.type](
            onSuccessToast.message,
            onSuccessToast.data,
          );
        }
      }
    },

    onSettled: (
      _data,
      error,
      _variables,
      _context,
      mutation,
    ): Promise<void> | undefined => {
      const queries = mutation.meta?.invalidateQueries;

      if (error != undefined && !mutation.meta?.invalidateQueriesOnError) {
        return;
      }

      if (queries) {
        queryClient.invalidateQueries({
          predicate: (query) =>
            queries.some((queryKey) => matchQuery({ queryKey }, query)) ?? true,
        });
      }

      if (mutation.meta?.awaitInvalidationQuery) {
        return queryClient.invalidateQueries(
          {
            queryKey: mutation.meta.awaitInvalidationQuery,
          },
          { cancelRefetch: false },
        );
      }
    },
  }),
});
