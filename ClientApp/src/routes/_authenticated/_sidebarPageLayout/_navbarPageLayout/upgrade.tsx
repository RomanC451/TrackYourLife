import { createFileRoute } from "@tanstack/react-router";

import UpgradePage from "@/pages/payments/UpgradePage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/upgrade",
)({
  component: UpgradePage,
});
