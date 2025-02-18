import { createFileRoute } from "@tanstack/react-router";

import OverviewPage from "@/pages/nutrition/OverviewPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/overview",
)({
  component: OverviewPage,
});
