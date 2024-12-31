import { useEffect, useMemo, useRef, useState } from "react";

import { ObjectValues } from "@/types/defaultTypes";

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
  const [loadingState, setLoadingState] = useState<LoadingStateEnum>(() => {
    if (data === false || data != undefined) return loadingStateEnum.LOADED;
    return loadingStateEnum.STARTING;
  });

  const timeoutIdRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    if (timeoutIdRef.current === null && data === true) {
      setLoadingState(loadingStateEnum.STARTING);
      const tId = setTimeout(
        () => setLoadingState(loadingStateEnum.LOADING),
        delay,
      );

      timeoutIdRef.current = tId;
    } else {
      setLoadingState(loadingStateEnum.LOADED);
      if (timeoutIdRef.current) {
        clearTimeout(timeoutIdRef.current);
        timeoutIdRef.current = null;
      }
    }

    return () => {
      if (timeoutIdRef.current) clearTimeout(timeoutIdRef.current);
    };
  }, [data, delay]);

  const memoizedLoadingState = useMemo(() => {
    return {
      isStarting: loadingState === loadingStateEnum.STARTING,
      isLoading: loadingState === loadingStateEnum.LOADING,
      isLoaded: loadingState === loadingStateEnum.LOADED,
    };
  }, [loadingState]);

  return memoizedLoadingState;
}

export default useDelayedLoading;
