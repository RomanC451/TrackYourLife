import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";

import RootPageLayout from "@/layouts/pageLayouts/RootPageLayout";
import LoadingPage from "@/pages/LoadingPage";
import MissingPage from "@/pages/MissingPage";

export type AuthContext = {
  userLoggedIn: () => Promise<boolean>;
};

export const Route = createRootRouteWithContext<AuthContext>()({
  component: () => (
    <RootPageLayout>
      <Outlet />
    </RootPageLayout>
  ),
  notFoundComponent: MissingPage,
  pendingMs: 100,
  pendingComponent: () => <LoadingPage />,

  // ?? Is this necessary?
  // loader: () => wait(1),
});
