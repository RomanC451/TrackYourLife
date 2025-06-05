import { createFileRoute } from "@tanstack/react-router";

import OverviewPage from "@/pages/trainings/OverviewPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/overview",
)({
  component: RouteComponent,
});

function RouteComponent() {
  return <OverviewPage />;
}
