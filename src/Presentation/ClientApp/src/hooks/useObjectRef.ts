import { MutableRefObject, useRef } from "react";

type RefValue = number | string | boolean;

export type ObjectRefType<T extends Record<string, RefValue>> = {
  [K in keyof T]: MutableRefObject<T[K]>;
};

const useObjectRef = <T extends Record<string, RefValue>>(
  data: T
): [MutableRefObject<T>, (name: string, value: T[keyof T]) => void] => {
  const objectRef = useRef(data);

  function handleChange(name: string, value: T[keyof T]) {
    if (!(name in objectRef.current)) {
      return;
    }
    (objectRef.current[name as keyof T] as T[keyof T]) = value as T[keyof T];
  }

  return [objectRef, handleChange];
};

export default useObjectRef;
