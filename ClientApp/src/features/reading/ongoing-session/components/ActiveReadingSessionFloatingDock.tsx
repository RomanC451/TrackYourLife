import { Link, useLocation } from "@tanstack/react-router";
import { BookOpen } from "lucide-react";

import { cn } from "@/lib/utils";
import type { ReadingSessionDto } from "@/services/openapi";

import { useReadingTimer } from "../context/ReadingTimerContext";

function formatElapsedSeconds(totalSeconds: number) {
  const minutes = Math.floor(totalSeconds / 60);
  const seconds = totalSeconds % 60;
  return `${minutes}:${String(seconds).padStart(2, "0")}`;
}

type ActiveReadingSessionFloatingDockProps = {
  session: ReadingSessionDto;
};

function ActiveReadingSessionFloatingDock({
  session,
}: ActiveReadingSessionFloatingDockProps) {
  const pathname = useLocation({ select: (location) => location.pathname });
  const { elapsedSeconds, isRunning } = useReadingTimer();

  if (pathname === "/reading/ongoing-session") {
    return null;
  }

  return (
    <Link
      to="/reading/ongoing-session"
      className={cn(
        "fixed right-0 top-1/2 z-60 flex w-14 -translate-y-1/2 flex-col items-center justify-center gap-1.5",
        "rounded-l-full border border-r-0 bg-primary py-4 pl-3 pr-2 text-primary-foreground shadow-lg",
        "transition-transform hover:-translate-x-0.5 hover:shadow-xl",
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
      )}
      aria-label={`Resume reading ${session.bookTitle}`}
      title={session.bookTitle}
    >
      <BookOpen className="h-5 w-5 shrink-0" aria-hidden />
      <span className="font-mono text-[11px] leading-none tabular-nums">
        {formatElapsedSeconds(elapsedSeconds)}
      </span>
      {isRunning && (
        <span
          className="absolute right-2 top-3 h-2 w-2 rounded-full bg-primary-foreground/90"
          aria-hidden
        />
      )}
    </Link>
  );
}

export default ActiveReadingSessionFloatingDock;
