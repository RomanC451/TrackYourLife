import React, {
  createContext,
  ReactNode,
  useContext,
  useEffect,
  useState
} from "react";
import { Assert } from "~/utils";

interface ContextInterface {
  sidebarActive: boolean;
  setSidebarActive: React.Dispatch<React.SetStateAction<boolean>>;
  screenSize: { width: number; height: number };
}

const AppGeneralStateContext = createContext<ContextInterface>(
  {} as ContextInterface
);

// interface AppGeneralContextProviderProps {
//   children: React.ReactNode;
// }

export const AppGeneralContextProvider = ({
  children
}: {
  children: ReactNode;
}): JSX.Element => {
  const [sidebarActive, setSidebarActive] = useState(false);
  const [screenSize, setScreenSize] = useState({
    width: window.innerWidth,
    height: window.innerHeight
  });

  useEffect(() => {
    const handleResize = () =>
      setScreenSize({ width: window.innerWidth, height: window.innerHeight });
    window.addEventListener("resize", handleResize);

    return () => window.removeEventListener("resize", handleResize);
  }, []);

  return (
    <AppGeneralStateContext.Provider
      value={{
        sidebarActive,
        setSidebarActive,
        screenSize
      }}
    >
      {children}
    </AppGeneralStateContext.Provider>
  );
};

export const useAppGeneralStateContext = () => {
  const context = useContext(AppGeneralStateContext);
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
