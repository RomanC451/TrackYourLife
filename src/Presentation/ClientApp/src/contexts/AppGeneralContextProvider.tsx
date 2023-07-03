import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode
} from "react";

interface ContextInterface {
  sidebarActive: boolean;
  setSidebarActive: React.Dispatch<React.SetStateAction<boolean>>;
  screenSize: number;
  setScreenSize: React.Dispatch<React.SetStateAction<number>>;
  themeSettingsActive: boolean;
  setThemeSettingsActive: React.Dispatch<React.SetStateAction<boolean>>;
}

const initialState = {
  sidebarActive: false,
  setSidebarActive: () => {},
  screenSize: window.innerWidth,
  setScreenSize: () => {},
  themeSettingsActive: false,
  setThemeSettingsActive: () => {}
};

const AppGeneralStateContext = createContext<ContextInterface>(initialState);

// interface AppGeneralContextProviderProps {
//   children: React.ReactNode;
// }

export const AppGeneralContextProvider = ({
  children
}: {
  children: ReactNode;
}): JSX.Element => {
  const [sidebarActive, setSidebarActive] = useState(
    initialState.sidebarActive
  );
  const [screenSize, setScreenSize] = useState(initialState.screenSize);
  const [themeSettingsActive, setThemeSettingsActive] = useState(
    initialState.themeSettingsActive
  );
  const handleResize = () => setScreenSize(window.innerWidth);

  useEffect(() => {
    window.addEventListener("resize", handleResize);

    return () => window.removeEventListener("resize", handleResize);
  }, []);

  return (
    <AppGeneralStateContext.Provider
      value={{
        sidebarActive,
        setSidebarActive,
        screenSize,
        setScreenSize,
        themeSettingsActive,
        setThemeSettingsActive
      }}
    >
      {children}
    </AppGeneralStateContext.Provider>
  );
};

export const useAppGeneralStateContext = () =>
  useContext(AppGeneralStateContext);
