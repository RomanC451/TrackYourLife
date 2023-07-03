import { createContext, ReactNode, useContext, useState } from "react";
import wretch from "wretch";

import { apiUrl } from "../data/apiSettings";

interface ContextInterface {
  defaultApi: any;
  authorizedApi: any;
  setJwtToken: (newToken: string) => void;
}

const initialState = {
  defaultApi: wretch("").resolve(() => {}),
  authorizedApi: wretch("").resolve(() => {}),
  setJwtToken: () => {}
};

const ApiContext = createContext<ContextInterface>(initialState);

export const ApiContextProvider = ({
  children
}: {
  children: ReactNode;
}): JSX.Element => {
  const [jwtToken, setJwtToken] = useState("");

  const defaultApi = wretch(apiUrl).resolve((_) =>
    _.forbidden(() => {
      //todo the implementation of forbidden access to do API
    })
  );

  const authorizedApi = wretch(apiUrl)
    // Authorization header
    .auth(`Bearer ${jwtToken}`)
    // Cors fetch options
    // .options({ credentials: "include", mode: "cors" })
    // Handle 403 errors
    .resolve((_) =>
      _.forbidden(() => {
        //todo the implementation of forbidden access to do API
      })
    );

  return (
    <ApiContext.Provider
      value={{
        defaultApi,
        authorizedApi,
        setJwtToken
      }}
    >
      {children}
    </ApiContext.Provider>
  );
};

export const useApiContext = () => useContext(ApiContext);
