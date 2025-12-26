import { createFileRoute, Outlet, redirect } from "@tanstack/react-router";

import { AuthenticationContextProvider } from "@/contexts/AuthenticationContextProvider";

export const Route = createFileRoute("/_authenticated")({
  beforeLoad: async ({ context, location }) => {
    const isProtected = await context.userLoggedIn();

    if (isProtected === false) {
      throw redirect({
        to: "/auth",
        search: {
          redirect: location.pathname,
        },
        replace: true,
      });
    }
  },

  component: () => {
    return (
      <AuthenticationContextProvider>
        <Outlet />
      </AuthenticationContextProvider>
    );
  },
});
