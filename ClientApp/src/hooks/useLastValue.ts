import { useEffect, useRef } from "react";

function useLastValue<T>(value: T) {
  const lastValue = useRef<T | undefined>(undefined);

  useEffect(() => {
    lastValue.current = value;
  }, [value]);

  return lastValue.current;
}

export default useLastValue;
