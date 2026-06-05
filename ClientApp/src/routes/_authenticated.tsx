import { createFileRoute, Outlet, redirect } from "@tanstack/react-router";

import { authModes } from "@/features/authentication/data/enums";

export const Route = createFileRoute("/_authenticated")({
  beforeLoad: async ({ context, location }) => {
    const isProtected = await context.userLoggedIn();

    if (isProtected === false) {
      throw redirect({
        to: "/auth",
        search: {
          authMode: authModes.logIn,
          redirect: location.pathname,
        },
        replace: true,
      });
    }
  },

  component: () => <Outlet />,
});
