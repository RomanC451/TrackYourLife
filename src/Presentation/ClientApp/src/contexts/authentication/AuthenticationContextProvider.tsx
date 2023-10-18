import { createContext, ReactNode, useContext, useState } from "react";
import { useLocalStorage } from "usehooks-ts";
import { transitionProps } from "~/components/cards/AuthCard";
import {
  authAlertType,
  severityEnum
} from "~/features/authentication/data/alerts";
import { authModes, TAuthModes } from "~/features/authentication/data/enums";

interface ContextInterface {
  authMode: TAuthModes;
  switchAuthMode: () => void;
  isAnimating: boolean;
  alert: authAlertType;
  setAlert: (formState: authAlertType) => void;
}

const initialState = {
  authMode: authModes.singUp,
  switchAuthMode: () => {},
  isAnimating: false,
  alert: {
    message: "",
    severity: severityEnum.info
  },
  setAlert: () => {}
};

const AuthenticationContext = createContext<ContextInterface>(initialState);

/**
 * React context provider for the authentication page.
 * @param children The children of the context provider.
 * @returns A JSX Element.
 */
export const AuthenticationContextProvider = ({
  children
}: {
  children: ReactNode;
}): JSX.Element => {
  const [authMode, setAuthMode] = useLocalStorage<TAuthModes>(
    "authenticationMethode",
    authModes.logIn
  );

  const [alert, setAlertMEssage] = useState<authAlertType>({
    message: "",
    severity: severityEnum.info
  });

  const setAlert = (formState: authAlertType) => {
    setAlertMEssage(formState);
  };

  const [isAnimating, setIsAnimating] = useState(false);

  function switchAuthMode() {
    if (isAnimating) return;

    setIsAnimating(true);
    setAuthMode(
      authMode === authModes.logIn ? authModes.singUp : authModes.logIn
    );
    setTimeout(() => {
      setIsAnimating(false);
    }, transitionProps.duration * 1000);
  }

  return (
    <AuthenticationContext.Provider
      value={{
        authMode,
        switchAuthMode,
        isAnimating,
        alert,
        setAlert
      }}
    >
      {children}
    </AuthenticationContext.Provider>
  );
};

export const useAuthenticationContext = () => useContext(AuthenticationContext);
