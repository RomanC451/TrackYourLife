import { useCallback, useState } from "react";

const STORAGE_KEY = "showWorkoutTimer";

export function useShowWorkoutTimer() {
  const [shouldShowTimer, setShouldShowTimer] = useState(false);

  const refreshShowWorkoutTimer = useCallback(() => {
    const show = localStorage.getItem(STORAGE_KEY) === "true";
    setShouldShowTimer(show);
  }, []);


  const resetShowWorkoutTimer = useCallback(() => {
    localStorage.removeItem(STORAGE_KEY);
  }, []);

  const triggerShowTimer = useCallback(() => {
    localStorage.setItem(STORAGE_KEY, "true");
    setShouldShowTimer(true);
  }, []);

  return { shouldShowTimer, triggerShowTimer, resetShowWorkoutTimer, refreshShowWorkoutTimer};
}