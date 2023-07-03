import { useRef } from "react";

const useWaitCallback = (): [() => boolean, () => void] => {
  const inProgress = useRef(false);

  function canCall() {
    console.log("animation in progress");
    if (!inProgress.current) {
      inProgress.current = true;
      return true;
    } else {
      return false;
    }
  }

  function reset() {
    console.log("animation finished");
    inProgress.current = false;
  }

  return [canCall, reset];
};

export default useWaitCallback;
