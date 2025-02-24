import { useEffect, useState } from "react";

export interface LoadingState {
  isStarting: boolean;
  isLoading: boolean;
  isLoaded: boolean;
}

function useDelayedLoading(
  isLoading: boolean | undefined,
  delay: number = 400,
): LoadingState {
  const [state, setState] = useState<LoadingState>(() => ({
    isStarting: isLoading === true,
    isLoading: false,
    isLoaded: isLoading === false || isLoading === undefined,
  }));

  useEffect(() => {
    let timeoutId: NodeJS.Timeout | undefined;

    if (isLoading === true) {
      setState({ isStarting: true, isLoading: false, isLoaded: false });
      timeoutId = setTimeout(() => {
        setState({ isStarting: false, isLoading: true, isLoaded: false });
      }, delay);
    } else if (isLoading === false) {
      setState({ isStarting: false, isLoading: false, isLoaded: true });
    }

    return () => {
      if (timeoutId) clearTimeout(timeoutId);
    };
  }, [isLoading, delay]);

  return state;
}

export default useDelayedLoading;
