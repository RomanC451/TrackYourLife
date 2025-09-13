import { createFileRoute, Outlet } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary/_dialogs",
)({
  component: RouteComponent,

  errorComponent: () => <>error</>,
});

function RouteComponent() {
  return (
    <div className="fixed inset-0 z-50 bg-black/80">
      <Outlet />
    </div>
  );
}
