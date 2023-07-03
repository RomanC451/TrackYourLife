import { useState } from "react";

type RefValue = number | string | boolean;

export type ObjectStateType<T extends Record<string, RefValue>> = {
  [K in keyof T]: T[K];
};

type returnType<T extends Record<string, RefValue>> = [
  ObjectStateType<T>,
  (name: string, value: T[keyof T], resetFirst: boolean) => void
];

//TODO: create a useInputErrors hook
const useObjectState = <T extends Record<string, RefValue>>(
  data: T
): returnType<T> => {
  const [objectState, setObjectState] = useState<ObjectStateType<T>>(data);

  function handleChange(name: string, value: T[keyof T], resetFirst: boolean) {
    if (!(name in objectState)) {
      setObjectState(data);
      return;
    }

    setObjectState((prevRefsObject) => ({
      ...(resetFirst ? data : prevRefsObject), // TODO: use prevRefsObject instead
      [name]: value
    }));
  }

  return [objectState, handleChange];
};

export default useObjectState;
