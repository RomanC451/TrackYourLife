import { useEffect, useRef, useState } from "react";
import { SkipForward } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";

import { useShowWorkoutTimer } from "../../hooks/useShowWorkoutTimer";

function WorkoutTimerPage({
  timerSeconds,
  setIsResting,
  timerTrigger,
  hidden,
}: {
  timerSeconds: number;
  timerTrigger: boolean;
  setIsResting: (isResting: boolean) => void;
  hidden: boolean;
}) {
  const [timerSecondsLeft, setTimerSecondsLeft] = useState(timerSeconds);
  const intervalRef = useRef<NodeJS.Timeout | null>(null);
  const audioRef = useRef<HTMLAudioElement | null>(null);

  const { resetShowWorkoutTimer } = useShowWorkoutTimer();

  // Interval to decrement timer
  useEffect(() => {
    if (timerSecondsLeft === 0 || hidden) return;
    intervalRef.current = setInterval(() => {
      setTimerSecondsLeft((prev) => {
        if (prev <= 1) {
          clearInterval(intervalRef.current!);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);
    return () => clearInterval(intervalRef.current!);
  }, [timerSecondsLeft, hidden]);

  useEffect(() => {
    if (timerSecondsLeft === timerSeconds && !hidden) {
      setIsResting(true);
    } else if (timerSecondsLeft === 0) {
      setIsResting(false);
    }
  }, [timerSecondsLeft, setIsResting, timerSeconds, hidden]);

  // Reset timer when timerTrigger is changed
  useEffect(() => {
    setTimerSecondsLeft(timerSeconds);
  }, [timerTrigger, setTimerSecondsLeft, timerSeconds]);

  // Play alarm when timer ends
  useEffect(() => {
    if (timerSecondsLeft === 0 && audioRef.current) {
      resetShowWorkoutTimer();
      audioRef.current.play();
    }
  }, [timerSecondsLeft, resetShowWorkoutTimer]);

  // Progress bar calculation
  const progress = ((timerSeconds - timerSecondsLeft) / timerSeconds) * 100;

  const handleSkip = () => {
    setTimerSecondsLeft(0);
    setIsResting(false);
  };

  return (
    <>
      {timerSecondsLeft === 0 || hidden ? null : (
        <>
          <div className="w-full space-y-4 rounded-xl border border-muted-foreground/40 bg-muted-foreground/10 p-4 shadow-lg">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <span className="text-lg">🕒</span>
                <span className="text-base font-semibold">Rest Timer</span>
              </div>
              <span className="text-3xl font-bold">{timerSecondsLeft} sec</span>
            </div>
            <div className="h-1 w-full rounded-full bg-muted-foreground/40">
              <div
                className="h-1 rounded-full bg-violet-600 transition-all duration-300"
                style={{ width: `${progress}%` }}
              ></div>
            </div>
            <div className="inline-flex w-full justify-end">
              <Button
                variant="default"
                className="h-8 bg-violet-600 px-4 py-1 text-sm hover:bg-violet-700"
                onClick={handleSkip}
              >
                <span className="flex items-center gap-1">
                  Skip
                  <SkipForward />
                </span>
              </Button>
            </div>
          </div>
          <Separator className="h-[1px] w-full" />
        </>
      )}
      <audio ref={audioRef} src="/alarm.mp3" preload="auto" />
    </>
  );
}

export default WorkoutTimerPage;
