import React, {
  createContext,
  ReactNode,
  useContext,
  useEffect,
  useState
} from "react";

interface ContextInterface {
  sidebarActive: boolean;
  setSidebarActive: React.Dispatch<React.SetStateAction<boolean>>;
  screenSize: number;
  setScreenSize: React.Dispatch<React.SetStateAction<number>>;
}

const initialState = {
  sidebarActive: false,
  setSidebarActive: () => {},
  screenSize: window.innerWidth,
  setScreenSize: () => {}
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

  useEffect(() => {
    const handleResize = () => setScreenSize(window.innerWidth);
    window.addEventListener("resize", handleResize);

    return () => window.removeEventListener("resize", handleResize);
  }, []);

  return (
    <AppGeneralStateContext.Provider
      value={{
        sidebarActive,
        setSidebarActive,
        screenSize,
        setScreenSize
      }}
    >
      {children}
    </AppGeneralStateContext.Provider>
  );
};

export const useAppGeneralStateContext = () =>
  useContext(AppGeneralStateContext);
