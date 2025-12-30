import { createFileRoute, Outlet } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/channels/_dialogs"
)({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div className="fixed inset-0 z-50 bg-black/80">
      <Outlet />
    </div>
  );
}

