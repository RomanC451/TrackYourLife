import React, {
  createContext,
  ReactNode,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";

const LoadingStateContext = createContext<boolean | undefined>(undefined);
const LoadingUpdateContext = createContext<
  ((id: string, state: boolean) => void) | undefined
>(undefined);

export const LoadingContextProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const [loadingStates, setLoadingStates] = useState<Record<string, boolean>>(
    {},
  );

  const updateLoadingState = useCallback((id: string, state: boolean) => {
    setLoadingStates((prev) => ({ ...prev, [id]: state }));
  }, []);

  const isLoading = useMemo(
    () => Object.values(loadingStates).some((state) => state),
    [loadingStates],
  );

  return (
    <LoadingUpdateContext.Provider value={updateLoadingState}>
      <LoadingStateContext.Provider value={isLoading}>
        {children}
      </LoadingStateContext.Provider>
    </LoadingUpdateContext.Provider>
  );
};

// Separate hooks for state and updates
export const useLoadingState = () => {
  const context = useContext(LoadingStateContext);
  if (context === undefined)
    throw new Error(
      "useLoadingState must be used within LoadingContextProvider",
    );
  return context;
};

export const useLoadingUpdate = () => {
  const context = useContext(LoadingUpdateContext);
  if (context === undefined)
    throw new Error(
      "useLoadingUpdate must be used within LoadingContextProvider",
    );
  return context;
};

// Modified useStoreLoadingStateToContext.ts
export default function useStoreLoadingStateToContext(
  id: string,
  loadingState: boolean,
) {
  const updateLoadingState = useLoadingUpdate(); // Only subscribes to updater function

  useEffect(() => {
    updateLoadingState(id, loadingState);
  }, [id, loadingState, updateLoadingState]);
}
