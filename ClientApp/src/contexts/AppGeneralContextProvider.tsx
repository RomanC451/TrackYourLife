import {
  createContext,
  ReactNode,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";

import Assert from "@/lib/assert";

interface ContextInterface {
  screenSize: { width: number};
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

  useEffect(() => {
    const handleResize = () => {
      setScreenSize({ width: window.innerWidth});
    };
    window.addEventListener("resize", handleResize);

    return () => window.removeEventListener("resize", handleResize);
  }, []);

  const contextValue = useMemo(
    () => ({
      screenSize,
    }),
    [screenSize],
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
    "useCount must be used within a CountProvider!",
  );
  Assert.isNotEmptyObject(
    context,
    "useCount must be used within a CountProvider!",
  );
  return context;
};
