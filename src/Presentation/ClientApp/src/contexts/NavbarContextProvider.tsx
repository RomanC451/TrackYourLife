import React, { createContext, useContext, useState, ReactNode } from "react";

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
  musicPlayer: false,
};

export const NavbarContextProvider = ({
  children,
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
        resetNavbarState,
      }}
    >
      {children}
    </NavbarStateContext.Provider>
  );
};

export const useNavbarStateContext = () => useContext(NavbarStateContext);
