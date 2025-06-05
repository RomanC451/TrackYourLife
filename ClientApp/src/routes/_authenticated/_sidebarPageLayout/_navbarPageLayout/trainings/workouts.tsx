import { createFileRoute } from "@tanstack/react-router";

import WorkoutsPage from "@/pages/trainings/WorkoutsPage";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/workouts",
)({
  component: RouteComponent,
});

function RouteComponent() {
  return <WorkoutsPage />;
}
