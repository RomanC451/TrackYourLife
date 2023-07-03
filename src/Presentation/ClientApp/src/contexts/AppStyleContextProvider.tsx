import React, { createContext, useContext, useState, ReactNode } from "react";

import { useLocalStorage, useDarkMode } from "usehooks-ts";

interface ContextInterface {
  currentColor: string;
  setCurrentColor: (color: string) => void;
  isDarkMode: boolean;
  togleDarkMode: () => void;
}

const AppStyleStateContext = createContext<ContextInterface>(
  {} as ContextInterface
);

export const AppStyleContextProvider = ({
  children,
}: {
  children: ReactNode;
}) => {
  const [currentColor, setCurrentColor] = useLocalStorage(
    "currentColor",
    "#03C9D7"
  );
  const { isDarkMode, toggle } = useDarkMode(true);

  function togleDarkMode() {
    toggle();
  }

  return (
    // eslint-disable-next-line react/jsx-no-constructed-context-values
    <AppStyleStateContext.Provider
      value={{
        currentColor,
        setCurrentColor,
        isDarkMode,
        togleDarkMode,
      }}
    >
      {children}
    </AppStyleStateContext.Provider>
  );
};

export const useAppStyleStateContext = () => useContext(AppStyleStateContext);
