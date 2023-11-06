import { createContext, ReactNode, useContext, useState } from "react";
import { useDarkMode, useLocalStorage } from "usehooks-ts";
import { Assert } from "~/utils";

interface ContextInterface {
  currentColor: string;
  setCurrentColor: (color: string) => void;
  isDarkMode: boolean;
  togleDarkMode: () => void;
}

const AppStyleContext = createContext<ContextInterface>({} as ContextInterface);

export const AppStyleContextProvider = ({
  children
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
    <AppStyleContext.Provider
      value={{
        currentColor,
        setCurrentColor,
        isDarkMode,
        togleDarkMode
      }}
    >
      {children}
    </AppStyleContext.Provider>
  );
};

export const useAppStyleStateContext = () => {
  const context = useContext(AppStyleContext);
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
