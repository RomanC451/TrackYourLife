import { createFileRoute } from "@tanstack/react-router";

import SubscriptionSuccessPage from "@/pages/payments/SubscriptionSuccessPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/subscription-success",
)({
  component: SubscriptionSuccessPage,
});
