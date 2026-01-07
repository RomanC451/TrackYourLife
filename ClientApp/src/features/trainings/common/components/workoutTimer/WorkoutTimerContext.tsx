import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { differenceInSeconds } from "date-fns";
import { useLocalStorage } from "usehooks-ts";

import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import Assert from "@/lib/assert";

type WorkoutTimerContextType = {
  secondsLeft: number;
  isTimerPlaying: boolean;
  startTimer: () => void;
  stopTimer: () => void;
  progress: number;
};

const WorkoutTimerContext = createContext<WorkoutTimerContextType>({
  secondsLeft: 0,
  isTimerPlaying: false,
  startTimer: () => {},
  stopTimer: () => {},
  progress: 0,
});

export function WorkoutTimerContextProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const ongoingTrainingQuery = useSuspenseQuery(
    ongoingTrainingsQueryOptions.active,
  );

  const restSeconds = ongoingTrainingQuery.data?.training.restSeconds || 0;

  const [timerStartedAt, setTimerStartedAt] = useLocalStorage<Date | undefined>(
    "timerStartedAt",
    undefined,
  );

  const [secondsLeft, setSecondsLeft] = useState(
    timerStartedAt
      ? Math.max(
          0,
          restSeconds - differenceInSeconds(new Date(), timerStartedAt),
        )
      : 0,
  );

  const intervalRef = useRef<NodeJS.Timeout | null>(null);

  useEffect(() => {
    if (secondsLeft <= 0) return;

    intervalRef.current = setInterval(() => {
      setSecondsLeft((prev) => {
        if (prev <= 1) {
          clearInterval(intervalRef.current!);
          setTimerStartedAt(undefined);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);
    return () => clearInterval(intervalRef.current!);
  }, [secondsLeft, setTimerStartedAt]);

  const startTimer = useCallback(() => {
    setSecondsLeft(ongoingTrainingQuery.data.training.restSeconds);
    setTimerStartedAt(new Date());
  }, [ongoingTrainingQuery.data, setTimerStartedAt]);

  const stopTimer = useCallback(() => {
    clearInterval(intervalRef.current!);
    setSecondsLeft(0);
    setTimerStartedAt(undefined);
  }, [setTimerStartedAt]);

  const contextValue = useMemo(
    () => ({
      secondsLeft,
      isTimerPlaying: secondsLeft > 0,
      startTimer,
      stopTimer,
      progress: ((restSeconds - secondsLeft) / restSeconds) * 100,
    }),
    [secondsLeft, startTimer, stopTimer, restSeconds],
  );

  return (
    <WorkoutTimerContext.Provider value={contextValue}>
      {children}
    </WorkoutTimerContext.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useWorkoutTimerContext() {
  const context = useContext(WorkoutTimerContext);

  Assert.contextIsDefined(context, "WorkoutTimerContext");

  return context;
}

export default WorkoutTimerContext;
