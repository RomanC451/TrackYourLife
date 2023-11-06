import { createContext, ReactNode, useContext, useState } from "react";
import { Assert } from "~/utils";

interface ContextInterface {
  navbarState: NavbarStateInterface;
  handleClick: (navbarItem: string) => void;
  resetNavbarState: () => void;
}

interface NavbarStateInterface {
  chat: boolean;
  cart: boolean;
  userProfile: boolean;
  notification: boolean;
  musicPlayer: boolean;
}

const NavbarStateContext = createContext<ContextInterface>(
  {} as ContextInterface
);

const navbarInitialState = {
  chat: false,
  cart: false,
  userProfile: false,
  notification: false,
  musicPlayer: false
};

export const NavbarContextProvider = ({
  children
}: {
  children: ReactNode;
}) => {
  const [navbarState, setNavbarState] = useState(navbarInitialState);

  const handleClick = (navbarItem: string) => {
    setNavbarState({ ...navbarInitialState, [navbarItem]: true });
  };

  const resetNavbarState = () => setNavbarState(navbarInitialState);

  return (
    <NavbarStateContext.Provider
      value={{
        navbarState,
        handleClick,
        resetNavbarState
      }}
    >
      {children}
    </NavbarStateContext.Provider>
  );
};

export const useNavbarStateContext = () => {
  const context = useContext(NavbarStateContext);
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
