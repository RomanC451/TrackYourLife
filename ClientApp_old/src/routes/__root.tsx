import { Outlet, createRootRouteWithContext } from "@tanstack/react-router";

import MissingPage from "~/pages/MissingPage";
import { wait } from "~/utils/wait";

export type AuthContext = {
  userLoggedIn: () => Promise<boolean>;
};

export const Route = createRootRouteWithContext<AuthContext>()({
  component: () => (
    <>
      <Outlet />
      {/* <TanStackRouterDevtools /> */}
    </>
  ),
  notFoundComponent: () => (
    <>
      <MissingPage />
      {/* <TanStackRouterDevtools /> */}
    </>
  ),
  loader: () => wait(1),
});
