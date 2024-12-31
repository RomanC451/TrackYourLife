import { FileRoutesByPath, ParsedLocation } from "@tanstack/react-router";

import { router } from "@/App";
import { AuthContext } from "@/routes/__root";

type ProtectedRouteBeforeLoadProps = {
  context: AuthContext;
  location: ParsedLocation<object>;
};

const protectedRouteBeforeLoad = async ({
  context,
  location,
}: ProtectedRouteBeforeLoadProps) => {
  const isProtected = await context.userLoggedIn();
  if (isProtected === false) {
    router.navigate({
      to: "/auth",
      search: {
        redirect: location.pathname as keyof FileRoutesByPath,
      },
      replace: true,
    });
  }
};

export default protectedRouteBeforeLoad;
