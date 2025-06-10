import { useEffect, useRef, useState } from "react";

import { Button } from "@/components/ui/button";
import { invalidateActiveOngoingTrainingQuery } from "@/features/trainings/ongoing-workout/queries/useActiveOngoingTrainingQuery";

function WorkoutTimerPage({
  timerSeconds,
  timerTrigger,
}: {
  timerSeconds: number;
  timerTrigger: boolean;
}) {
  const [timerSecondsLeft, setTimerSecondsLeft] = useState(timerSeconds);
  const intervalRef = useRef<NodeJS.Timeout | null>(null);
  const audioRef = useRef<HTMLAudioElement | null>(null);

  useEffect(() => {
    invalidateActiveOngoingTrainingQuery();
  }, []);

  useEffect(() => {
    if (timerSecondsLeft === 0) return;
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
  }, [timerSecondsLeft]);

  useEffect(() => {
    if (timerTrigger) {
      setTimerSecondsLeft(timerSeconds);
    }
  }, [timerTrigger, setTimerSecondsLeft, timerSeconds]);

  // Play alarm when timer ends
  useEffect(() => {
    if (timerSecondsLeft === 0 && audioRef.current) {
      audioRef.current.play();
    }
  }, [timerSecondsLeft]);

  // Progress bar calculation
  const progress = ((timerSeconds - timerSecondsLeft) / timerSeconds) * 100;

  if (timerSecondsLeft === 0) {
    return null;
  }

  const handleSkip = () => {
    setTimerSecondsLeft(0);
  };

  return (
    <>
      {/* <div className="flex min-h-[60vh] flex-col items-center justify-center"> */}
      <div className="w-full max-w-7xl rounded-xl bg-[#070e1a] p-8 shadow-lg">
        <h2 className="mb-2 text-3xl font-bold text-white">Take a break</h2>
        <div className="mb-2 text-sm text-blue-300">
          Next: Bench Press - Set 2
        </div>
        <div className="flex flex-col items-center justify-center py-6">
          <span className="text-[80px] font-bold text-blue-400">
            {timerSecondsLeft}
          </span>
        </div>
        <div className="mb-4 h-4 w-full rounded-full bg-[#1a2233]">
          <div
            className="h-4 rounded-full bg-blue-500 transition-all duration-300"
            style={{ width: `${progress}%` }}
          ></div>
        </div>
        <div className="mb-6 text-center text-lg text-blue-200">
          {timerSecondsLeft > 0
            ? `Rest for ${timerSeconds} seconds before continuing`
            : "Time's up! Ready to continue?"}
        </div>
        <Button
          className="w-full rounded-lg bg-blue-500 py-3 text-lg font-medium text-white transition hover:bg-blue-600"
          onClick={handleSkip}
        >
          Skip Rest
        </Button>
      </div>
      <audio ref={audioRef} src="/alarm.mp3" preload="auto" />
    </>
  );
}

export default WorkoutTimerPage;
