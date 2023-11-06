import { createContext, ReactNode, useContext, useState } from "react";
import { useLocalStorage } from "usehooks-ts";
import wretch from "wretch";
import { Assert } from "~/utils";

import { apiUrl } from "../data/apiSettings";

interface ContextInterface {
  defaultApi: any;
  authorizedApi: any;
  setJwtToken: (newToken: string) => void;
  refreshTokenIsValid: () => boolean;
}

const ApiContext = createContext<ContextInterface>({} as ContextInterface);

export const ApiContextProvider = ({
  children
}: {
  children: ReactNode;
}): JSX.Element => {
  const [jwtToken, setJwtToken] = useLocalStorage("rereshToken", "");

  const defaultApi = wretch(apiUrl).resolve((_) =>
    _.forbidden(() => {
      //todo the implementation of forbidden access to do API
    })
  );

  const authorizedApi = wretch(apiUrl)
    // Authorization header
    .auth(`Bearer ${jwtToken}`)
    // Cors fetch options
    .options({ credentials: "include", mode: "cors" })
    // Handle 403 errors
    .resolve((_) =>
      _.forbidden(() => {
        //todo the implementation of forbidden access to do API
      })
    );

  const refreshTokenIsValid = () => {
    return localStorage.getItem("refreshToken") !== "";
  };

  return (
    <ApiContext.Provider
      value={{
        defaultApi,
        authorizedApi,
        setJwtToken,
        refreshTokenIsValid
      }}
    >
      {children}
    </ApiContext.Provider>
  );
};

export const useApiContext = () => {
  const context = useContext(ApiContext);

  Assert.isNotUndefined(
    context,
    "useCount must be used within a CountProvider!"
  );
  Assert.isNotEmptyObject(
    context,
    "useCount must be used within a CountProvider!"
  );
  return context;
};
