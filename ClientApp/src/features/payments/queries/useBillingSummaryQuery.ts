import { queryOptions } from "@tanstack/react-query";

import { PaymentsApi } from "@/services/openapi";

const paymentsApi = new PaymentsApi();

export const billingSummaryQueryKeys = {
  all: ["billingSummary"] as const,
};

export const billingSummaryQueryOptions = {
  get: () =>
    queryOptions({
      queryKey: billingSummaryQueryKeys.all,
      queryFn: () => paymentsApi.getBillingSummary().then((res) => res.data),
    }),
};
