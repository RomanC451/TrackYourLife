import { createFileRoute } from "@tanstack/react-router";

import { billingSummaryQueryOptions } from "@/features/payments/queries/useBillingSummaryQuery";
import { billingPortalUrlQueryOptions } from "@/features/payments/queries/useGetBillingPortalUrlQuery";
import BillingPage from "@/pages/payments/BillingPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/billing",
)({
  component: BillingPage,
  loader: async () => {
    await Promise.all([
      queryClient.ensureQueryData(
        billingPortalUrlQueryOptions.byReturnPath("/billing"),
      ),
      queryClient.ensureQueryData(billingSummaryQueryOptions.get()),
    ]);
  },
});
