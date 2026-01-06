import {
  createContext,
  ReactNode,
  useCallback,
  useContext,
  useMemo,
  useState,
} from "react";
import { StatusCodes } from "http-status-codes";

import { router } from "@/App";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import Assert from "@/lib/assert";
import { FileRoutesByTo } from "@/routeTree.gen";
import { UserDto, UsersApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { retryQueryExcept } from "@/services/openapi/retry";

type ContextInterface = {
  userLoggedIn: () => Promise<boolean>;
  userData: UserDto | undefined;
  setQueryEnabled: (enabled: boolean) => void;
};

const unprotectedRoutes = new Set<keyof FileRoutesByTo>([
  "/auth",
  "/",
  "/error",
  "/not-found",
  "/email-verification",
  "/tmp",
]);

const AuthenticationContext = createContext<ContextInterface>(
  {} as ContextInterface,
);
const usersApi = new UsersApi();

/**
 * React context provider for the authentication page.
 * @param children The children of the context provider.
 * @returns A JSX Element.
 */
export const AuthenticationContextProvider = ({
  children,
}: {
  children: ReactNode;
}): React.JSX.Element => {
  const [queryEnabled, setQueryEnabled] = useState(
    !unprotectedRoutes.has(
      globalThis.location.pathname as keyof FileRoutesByTo,
    ),
  );

  const {
    query: { isLoading, data, refetch },
  } = useCustomQuery({
    queryKey: ["userData"],
    queryFn: () =>
      usersApi.getCurrentUser().then((res) => {
        router.invalidate();
        return res.data;
      }),
    staleTime: 1000 * 60 * 5,
    retry: (failureCount, error: ApiError) =>
      retryQueryExcept(failureCount, error, {
        checkedCodes: {
          [StatusCodes.UNAUTHORIZED]: null,
          [StatusCodes.NOT_FOUND]: null,
        },
      }),
    enabled: queryEnabled,
  });

  const userLoggedIn = useCallback(async (): Promise<boolean> => {
    try {
      if (isLoading) {
        const resp = await refetch();
        return Boolean(resp.data?.id);
      }

      return Boolean(data?.id);
    } catch {
      return false;
    }
  }, [isLoading, refetch, data]);

  const contextValue = useMemo(
    () => ({
      userLoggedIn: userLoggedIn,
      userData: data,
      setQueryEnabled,
    }),
    [userLoggedIn, data, setQueryEnabled],
  );

  return (
    <AuthenticationContext.Provider value={contextValue}>
      {children}
    </AuthenticationContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAuthenticationContext = () => {
  const context = useContext(AuthenticationContext);
  Assert.isNotUndefined(
    context,
    "useAuthenticationContext must be used within a AuthenticationContextProvider!",
  );
  Assert.isNotEmptyObject(
    context,
    "useAuthenticationContext must be used within a AuthenticationContextProvider!",
  );
  return context;
};
