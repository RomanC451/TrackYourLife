import { Suspense } from "react";
import { createFileRoute, Outlet } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/channels/_dialogs",
)({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div className="fixed inset-0 z-50 bg-black/80">
      <Suspense
        fallback={
          <div className="flex h-full items-center justify-center">
            <Loader2 className="h-10 w-10 animate-spin text-muted-foreground" />
          </div>
        }
      >
        <Outlet />
      </Suspense>
    </div>
  );
}
