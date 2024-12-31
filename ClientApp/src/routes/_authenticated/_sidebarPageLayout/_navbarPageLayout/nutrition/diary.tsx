import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary",
)({
  component: RouteComponent,
});

function RouteComponent() {
  return "";
}
