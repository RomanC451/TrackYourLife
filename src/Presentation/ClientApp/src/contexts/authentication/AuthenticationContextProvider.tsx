import { AnimationControls } from "framer-motion";
import React, {
  createContext,
  ReactNode,
  useContext,
  useEffect,
  useRef,
  useState
} from "react";
import { useLocalStorage } from "usehooks-ts";
import { authModesEnum } from "~/features/authentication/data/authEnums";
import useCardAnimation from "~/pages/authentication/animations/useCardAnimation";

interface ContextInterface {
  authMode: authModesEnum;
  setAuthMode: React.Dispatch<React.SetStateAction<authModesEnum>>;
  cardAnimationRef: AnimationControls;
  startCardAnimation: (callback: Function) => void;
  switchAuthMode: () => void;
}

const initialState = {
  authMode: authModesEnum.singUp,
  setAuthMode: () => {},
  cardAnimationRef: {} as AnimationControls,
  startCardAnimation: () => {},
  switchAuthMode: () => {}
};

const AuthenticationContext = createContext<ContextInterface>(initialState);

export const AuthenticationContextProvider = ({
  children
}: {
  children: ReactNode;
}): JSX.Element => {
  const [authMode, setAuthMode] = useLocalStorage(
    "authenticationMethode",
    authModesEnum.singUp
  );

  const [cardAnimationRef, startCardAnimation] = useCardAnimation(
    authMode === authModesEnum.logIn ? 0 : 1
  );

  function switchAuthMode() {
    startCardAnimation(() => {
      setAuthMode(
        authMode === authModesEnum.logIn
          ? authModesEnum.singUp
          : authModesEnum.logIn
      );
    });
  }

  return (
    <AuthenticationContext.Provider
      value={{
        authMode,
        setAuthMode,
        cardAnimationRef,
        startCardAnimation,
        switchAuthMode
      }}
    >
      {children}
    </AuthenticationContext.Provider>
  );
};

export const useAuthenticationContext = () => useContext(AuthenticationContext);
