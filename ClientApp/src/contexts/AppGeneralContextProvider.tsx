import {
  createContext,
  ReactNode,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";

import Assert from "@/lib/assert";

interface ContextInterface {
  screenSize: { width: number };
  queryToolsRef: React.RefObject<HTMLDivElement>;
  routerToolsRef: React.RefObject<HTMLDivElement>;
}

const AppGeneralStateContext = createContext<ContextInterface>(
  {} as ContextInterface,
);

export const AppGeneralContextProvider = ({
  children,
}: {
  children: ReactNode;
}): JSX.Element => {
  const [screenSize, setScreenSize] = useState({
    width: window.innerWidth,
  });
  const queryToolsRef = useRef<HTMLDivElement>(null);
  const routerToolsRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleResize = () => {
      setScreenSize({ width: window.innerWidth });
    };
    window.addEventListener("resize", handleResize);

    return () => window.removeEventListener("resize", handleResize);
  }, []);

  const contextValue = useMemo(
    () => ({
      screenSize,
      queryToolsRef,
      routerToolsRef,
    }),
    [screenSize, queryToolsRef],
  );

  return (
    <AppGeneralStateContext.Provider value={contextValue}>
      {children}
    </AppGeneralStateContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAppGeneralStateContext = () => {
  const context = useContext(AppGeneralStateContext);
  Assert.isNotUndefined(
    context,
    "useAppGeneralStateContext must be used within a AppGeneralContextProvider!",
  );
  Assert.isNotEmptyObject(
    context,
    "useAppGeneralStateContext must be used within a AppGeneralContextProvider!",
  );
  return context;
};
