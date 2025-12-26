import {
  createRootRouteWithContext,
  lazyRouteComponent,
  Navigate,
} from "@tanstack/react-router";

import LoadingPage from "@/pages/LoadingPage";

export type AuthContext = {
  userLoggedIn: () => Promise<boolean>;
};

export const Route = createRootRouteWithContext<AuthContext>()({
  component: lazyRouteComponent(
    () => import("@/layouts/pageLayouts/RootPageLayout"),
  ),
  notFoundComponent: NotFound,
  pendingMs: 100,
  pendingMinMs: 1000,
  pendingComponent: () => <LoadingPage />,
  loader: () => void 0,
});

function NotFound() {
  return Navigate({ to: "/not-found", replace: true }); // Redirect to your notFound route
}
