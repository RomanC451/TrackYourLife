import React, {
  createContext,
  ReactNode,
  useCallback,
  useContext,
  useMemo,
  useState,
} from "react";

type LoadingContextType = {
  loadingStates: Record<string, boolean>;
  updateLoadingState: (id: string, state: boolean) => void;
};

const LoadingContext = createContext<LoadingContextType | undefined>(undefined);

export const LoadingContextProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const [loadingStates, setLoadingStates] = useState<Record<string, boolean>>(
    {},
  );

  const updateLoadingState = useCallback((id: string, state: boolean) => {
    setLoadingStates((prev) => ({ ...prev, [id]: state }));
  }, []);

  const value = useMemo(
    () => ({ loadingStates, updateLoadingState }),
    [loadingStates, updateLoadingState],
  );

  return (
    <LoadingContext.Provider value={value}>{children}</LoadingContext.Provider>
  );
};

// Separate hooks for state and updates
// eslint-disable-next-line react-refresh/only-export-components
export const useLoadingContext = (id: string) => {
  const context = useContext(LoadingContext);
  if (context === undefined)
    throw new Error(
      "useLoadingState must be used within LoadingContextProvider",
    );

  // useEffect(() => {
  //   if (context.loadingStates[id] === undefined) {
  //     context.updateLoadingState(id, false);
  //   }
  // }, [context, id]);

  return {
    loadingState: context.loadingStates[id] || false,
    updateLoadingState: (state: boolean) =>
      context.updateLoadingState(id, state),
  };
};

// // Modified useStoreLoadingStateToContext.ts
// export default function useStoreLoadingStateToContext(
//   id: string,
//   loadingState: boolean,
// ) {
//   const updateLoadingState = useLoadingUpdate(); // Only subscribes to updater function

//   useEffect(() => {
//     updateLoadingState(id, loadingState);
//   }, [id, loadingState, updateLoadingState]);
// }
