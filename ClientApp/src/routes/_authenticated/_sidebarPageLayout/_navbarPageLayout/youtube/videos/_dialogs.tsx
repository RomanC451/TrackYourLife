import { createFileRoute, Outlet } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/videos/_dialogs",
)({
  component: RouteComponent,
});

function RouteComponent() {
  return <Outlet />;
}
