import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/_authenticated")({
  beforeLoad: async ({ context, location }) => {
    const isProtected = await context.userLoggedIn();
    if (isProtected === false) {
      throw redirect({
        to: "/auth",
        search: {
          redirect: location.href,
        },
        replace: true,
      });
    }
  },
});
