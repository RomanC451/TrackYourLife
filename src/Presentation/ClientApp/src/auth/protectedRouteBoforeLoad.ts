import { FileRoutesByPath, ParsedLocation } from "@tanstack/react-router";
import { router } from "~/App";
import { AuthContext } from "~/routes/__root";

type ProtectedRouteBoforeLoadProps = {
  context: AuthContext;
  location: ParsedLocation<object>;
};

const protectedRouteBoforeLoad = async ({
  context,
  location,
}: ProtectedRouteBoforeLoadProps) => {
  console.log("protected route hit");
  const isProtected = await context.userLogedIn();
  console.log(isProtected);
  if (isProtected === false) {
    router.navigate({
      to: "/auth",
      search: {
        redirect: location.href as keyof FileRoutesByPath,
      },
      replace: true,
    });
  }
};

export default protectedRouteBoforeLoad;
