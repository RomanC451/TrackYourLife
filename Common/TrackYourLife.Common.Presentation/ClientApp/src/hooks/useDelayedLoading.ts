import { Timeout } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { ObjectValues } from "~/types/defaultTypes";

const loadingStateEnum = {
  STARTING: 0,
  LOADING: 1,
  LOADED: 2,
} as const;

type TLoadingStateEnum = ObjectValues<typeof loadingStateEnum>;

export type TLoadingState = {
  isStarting: boolean;
  isLoading: boolean;
  isLoaded: boolean;
};

function useDelayedLoading(delay: number, data?: object | boolean | undefined) {
  const [loadingState, setLoadingState] = useState<TLoadingStateEnum>(
    loadingStateEnum.STARTING,
  );

  useEffect(() => {
    let timeoutId: Timeout | null = null;

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
