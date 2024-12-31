import { useEffect, useState } from "react";
import { ObjectValues } from "~/types/defaultTypes";

const loadingStateEnum = {
  STARTING: 0,
  LOADING: 1,
  LOADED: 2,
} as const;

type LoadingStateEnum = ObjectValues<typeof loadingStateEnum>;

export type LoadingState = {
  isStarting: boolean;
  isLoading: boolean;
  isLoaded: boolean;
};

function useDelayedLoading(
  data: object | boolean | undefined,
  delay: number = 400,
) {
  const [loadingState, setLoadingState] = useState<LoadingStateEnum>(
    loadingStateEnum.STARTING,
  );

  useEffect(() => {
    let timeoutId: ReturnType<typeof setTimeout> | null = null;

    if (data === undefined || data === true) {
      setLoadingState(loadingStateEnum.STARTING);
      timeoutId = setTimeout(
        () => setLoadingState(loadingStateEnum.LOADING),
        delay,
      );
    } else {
      setLoadingState(loadingStateEnum.LOADED);
      if (timeoutId) {
        clearTimeout(timeoutId);
        timeoutId = null;
      }
    }

    return () => {
      if (timeoutId) {
        clearTimeout(timeoutId);
      }
    };
  }, [data, delay]);

  return {
    isStarting: loadingState === loadingStateEnum.STARTING,
    isLoading: loadingState === loadingStateEnum.LOADING,
    isLoaded: loadingState === loadingStateEnum.LOADED,
  };
  // return loadingState;
}

export default useDelayedLoading;
