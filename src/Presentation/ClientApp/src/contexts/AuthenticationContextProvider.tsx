import { useQuery } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";
import { ReactNode, createContext, useContext } from "react";
import { getUserDataRequest } from "~/auth/getUsetDataRequest";
import { getErrorObject } from "~/data/apiSettings";
import { useApiRequests } from "~/hooks/useApiRequests";
import { UserData } from "~/types/authTypes";
import { Assert } from "~/utils";

type ContextInterface = {
  userLogedIn: () => Promise<boolean>;
  userData: UserData | undefined;
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
  const { fetchRequest } = useApiRequests();

  const userDataQuery = useQuery({
    queryKey: ["userData"],
    initialData: undefined,
    queryFn: () =>
      getUserDataRequest({
        fetchRequest,
      }),
    retry: (failureCount, error) => {
      const errorObject = getErrorObject(error);

      const stopRetrying =
        !errorObject ||
        [StatusCodes.UNAUTHORIZED, StatusCodes.NOT_FOUND].includes(
          errorObject.status,
        ) ||
        failureCount > 2;

      if (stopRetrying) return false;

      return true;
    },

    enabled: false,
  });

  const userLogedIn = async (): Promise<boolean> => {
    console.log(userDataQuery.data);
    if (userDataQuery.data?.id) return Promise.resolve(true);

    const resp = await userDataQuery.refetch();
    return Promise.resolve(resp.isSuccess);
  };

  return (
    <AuthenticationContext.Provider
      value={{
        userLogedIn,
        userData: userDataQuery.data,
      }}
    >
      {children}
    </AuthenticationContext.Provider>
  );
};

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
