import { useEffect, useRef } from "react";
import { SkipForward } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";

import { useWorkoutTimerContext } from "./WorkoutTimerContext";

function WorkoutTimer() {
  const audioRef = useRef<HTMLAudioElement | null>(null);

  const { secondsLeft, stopTimer, progress } = useWorkoutTimerContext();

  // Play alarm when timer ends
  useEffect(() => {
    if (secondsLeft === 1 && audioRef.current) {
      setTimeout(() => {
        audioRef.current?.play();
      }, 1000);
    }
  }, [secondsLeft]);

  const handleSkip = () => {
    stopTimer();
  };

  return (
    <>
      {secondsLeft === 0 ? null : (
        <>
          <div className="w-full space-y-4 rounded-xl border border-muted-foreground/40 bg-muted-foreground/10 p-4 shadow-lg">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <span className="text-lg">🕒</span>
                <span className="text-base font-semibold">Rest Timer</span>
              </div>
              <span className="text-3xl font-bold">{secondsLeft} sec</span>
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

export default WorkoutTimer;
