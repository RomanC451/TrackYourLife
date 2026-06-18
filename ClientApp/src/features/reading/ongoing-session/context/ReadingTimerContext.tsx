import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { useLocalStorage } from "usehooks-ts";

type ReadingTimerContextValue = {
  elapsedSeconds: number;
  isRunning: boolean;
  start: () => void;
  pause: () => void;
  reset: () => void;
};

const ReadingTimerContext = createContext<ReadingTimerContextValue | null>(
  null,
);

export function ReadingTimerProvider({
  sessionId,
  children,
}: {
  sessionId: string;
  children: ReactNode;
}) {
  const storageKey = `reading-timer-${sessionId}`;
  const [startedAt, setStartedAt] = useLocalStorage<number | null>(
    `${storageKey}-started`,
    null,
  );
  const [accumulated, setAccumulated] = useLocalStorage(
    `${storageKey}-accumulated`,
    0,
  );
  const [tick, setTick] = useState(0);

  const isRunning = startedAt !== null;

  useEffect(() => {
    if (sessionId === "inactive" || startedAt !== null || accumulated !== 0) {
      return;
    }
    setStartedAt(Date.now());
  }, [accumulated, sessionId, setStartedAt, startedAt]);

  useEffect(() => {
    if (!isRunning) return;
    const id = window.setInterval(() => setTick((t) => t + 1), 1000);
    return () => window.clearInterval(id);
  }, [isRunning]);

  const elapsedSeconds = useMemo(() => {
    if (!startedAt) return accumulated;
    return accumulated + Math.floor((Date.now() - startedAt) / 1000);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [accumulated, startedAt, tick]);

  const start = useCallback(() => {
    if (startedAt === null) setStartedAt(Date.now());
  }, [setStartedAt, startedAt]);

  const pause = useCallback(() => {
    if (startedAt !== null) {
      setAccumulated(
        accumulated + Math.floor((Date.now() - startedAt) / 1000),
      );
      setStartedAt(null);
    }
  }, [accumulated, setAccumulated, setStartedAt, startedAt]);

  const reset = useCallback(() => {
    setStartedAt(null);
    setAccumulated(0);
  }, [setAccumulated, setStartedAt]);

  const value = useMemo(
    () => ({ elapsedSeconds, isRunning, start, pause, reset }),
    [elapsedSeconds, isRunning, pause, reset, start],
  );

  return (
    <ReadingTimerContext.Provider value={value}>
      {children}
    </ReadingTimerContext.Provider>
  );
}

export function useReadingTimer() {
  const ctx = useContext(ReadingTimerContext);
  if (!ctx) {
    throw new Error("useReadingTimer must be used within ReadingTimerProvider");
  }
  return ctx;
}
