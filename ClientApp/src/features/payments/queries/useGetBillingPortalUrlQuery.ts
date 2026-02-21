import { queryOptions } from "@tanstack/react-query";

import { FileRoutesByTo } from "@/routeTree.gen";
import { PaymentsApi } from "@/services/openapi";

const paymentsApi = new PaymentsApi();

export const billingPortalUrlQueryKeys = {
  all: ["billingPortalUrl"] as const,
  byReturnPath: (returnPath: keyof FileRoutesByTo) =>
    [...billingPortalUrlQueryKeys.all, returnPath] as const,
};

export const billingPortalUrlQueryOptions = {
  byReturnPath: (returnPath: keyof FileRoutesByTo) =>
    queryOptions({
      queryKey: billingPortalUrlQueryKeys.byReturnPath(returnPath),
      queryFn: () =>
        paymentsApi
          .getBillingPortalUrl(`${globalThis.location.origin}${returnPath}`)
          .then((res) => res.data),
    }),
};
