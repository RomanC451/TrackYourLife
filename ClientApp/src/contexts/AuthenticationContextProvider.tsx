import { createContext, ReactNode, useCallback, useContext } from "react";
import { useQuery } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";

import Assert from "@/lib/assert";
import { UserDto, UsersApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { retryQueryExcept } from "@/services/openapi/retry";

type ContextInterface = {
  userLoggedIn: () => Promise<boolean>;
  userData: UserDto | undefined;
};

const AuthenticationContext = createContext<ContextInterface>(
  {} as ContextInterface,
);

/**
 * React context provider for the authentication page.
 * @param children The children of the context provider.
 * @returns A JSX Element.
 */
export const AuthenticationContextProvider = ({
  children,
}: {
  children: ReactNode;
}): JSX.Element => {
  const usersApi = new UsersApi();

  const userDataQuery = useQuery({
    queryKey: ["userData"],
    initialData: undefined,
    queryFn: () => usersApi.getCurrentUser().then((res) => res.data),

    retry: (failureCount, error: ApiError) =>
      retryQueryExcept(failureCount, error, {
        checkedCodes: {
          [StatusCodes.UNAUTHORIZED]: null,
          [StatusCodes.NOT_FOUND]: null,
        },
      }),

    enabled: false,
  });

  const userLoggedIn = useCallback(async (): Promise<boolean> => {
    if (userDataQuery.data?.id) return Promise.resolve(true);

    const resp = await userDataQuery.refetch();

    return Promise.resolve(resp.isSuccess);
  }, [userDataQuery]);

  return (
    <AuthenticationContext.Provider
      value={{
        userLoggedIn,
        userData: userDataQuery.data,
      }}
    >
      {children}
    </AuthenticationContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAuthenticationContext = () => {
  const context = useContext(AuthenticationContext);
  Assert.isNotUndefined(
    context,
    "useCount must be used within a CountProvider!",
  );
  Assert.isNotEmptyObject(
    context,
    "useCount must be used within a CountProvider!",
  );
  return context;
};
