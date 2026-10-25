import { createRouter } from "@tanstack/react-router";

import LoadingPage from "./pages/LoadingPage";
import RouteErrorPage from "./pages/RouteErrorPage";
import { routeTree } from "./routeTree.gen";

export const router = createRouter({
  routeTree,
  defaultPendingComponent: () => {
    return <LoadingPage />;
  },

  defaultErrorComponent: RouteErrorPage,
  notFoundMode: "root",

  context: {
    userLoggedIn: undefined!,
    userData: undefined!,
  },
  defaultPreload: "intent",
  defaultPreloadStaleTime: 1000 * 60 * 5, // 5 minutes
  defaultPreloadDelay: 100, // 100ms delay before preloading
});

declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}
