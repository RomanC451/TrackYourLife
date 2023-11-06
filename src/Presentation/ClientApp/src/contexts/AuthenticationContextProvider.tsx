import { createContext, ReactNode, useContext, useState } from "react";
import { useLocalStorage } from "usehooks-ts";
import { transitionProps } from "~/features/authentication/components/AuthCard";
import {
  authAlertType,
  authModesEnum,
  severityEnum,
  TAuthModes
} from "~/features/authentication/data/enums";
import { Assert } from "~/utils";

interface ContextInterface {
  authMode: TAuthModes;
  switchAuthMode: () => void;
  isAnimating: boolean;
  alert: authAlertType;
  setAlert: (value: authAlertType) => void;
  emailToVerificate: string;
  setEmailToVerificate: (value: string) => void;
  userData: UserData;
  setUserData: React.Dispatch<React.SetStateAction<UserData>>;
}

const AuthenticationContext = createContext<ContextInterface>(
  {} as ContextInterface
);

export const userDataInitValue = {
  id: "00000000-0000-0000-0000-000000000000",
  email: "",
  firstName: "",
  lastName: ""
};

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
  const [userData, setUserData] = useState<UserData>(userDataInitValue);

  const [authMode, setAuthMode] = useLocalStorage<TAuthModes>(
    "authenticationMethode",
    authModesEnum.logIn
  );
  const [alert, setAlertMessage] = useState<authAlertType>({
    message: "",
    severity: severityEnum.info
  });

  const [isAnimating, setIsAnimating] = useState(false);

  const [emailToVerificate, setEmailToVerificate] = useState<string>("");

  function switchAuthMode() {
    if (isAnimating) return;

    setIsAnimating(true);
    setAuthMode(
      authMode === authModesEnum.logIn
        ? authModesEnum.singUp
        : authModesEnum.logIn
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
        setAlert: setAlertMessage,
        emailToVerificate,
        setEmailToVerificate,
        userData,
        setUserData
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
    "useCount must be used within a CountProvider!"
  );
  Assert.isNotEmptyObject(
    context,
    "useCount must be used within a CountProvider!"
  );
  return context;
};
