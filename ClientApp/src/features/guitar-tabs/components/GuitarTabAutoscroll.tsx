import { useEffect, useRef, useState } from "react";
import { Minus, Pause, Play, Plus } from "lucide-react";

import { Button } from "@/components/ui/button";

const minSpeed = 0.1;
const maxSpeed = 1.5;
const speedStep = 0.1;
const pixelsPerSpeedUnit = 30;

export function changeAutoscrollSpeed(speed: number, direction: -1 | 1) {
  const next = Math.round((speed + direction * speedStep) * 10) / 10;
  return Math.min(maxSpeed, Math.max(minSpeed, next));
}

function isTypingTarget(target: EventTarget | null) {
  if (!(target instanceof HTMLElement)) {
    return false;
  }

  return (
    target instanceof HTMLInputElement ||
    target instanceof HTMLTextAreaElement ||
    target instanceof HTMLSelectElement ||
    target.isContentEditable
  );
}

function GuitarTabAutoscroll() {
  const [playing, setPlaying] = useState(false);
  const [speed, setSpeed] = useState(0.2);
  const speedRef = useRef(speed);
  speedRef.current = speed;

  useEffect(() => {
    const onKeyDown = (event: KeyboardEvent) => {
      if (event.altKey || event.ctrlKey || event.metaKey || isTypingTarget(event.target)) {
        return;
      }

      const target = event.target instanceof HTMLElement ? event.target : null;

      if (event.code === "Space") {
        if (event.repeat || target?.closest("[data-autoscroll-toggle]")) {
          return;
        }

        event.preventDefault();
        setPlaying((current) => !current);
        return;
      }

      if (event.code !== "ArrowUp" && event.code !== "ArrowDown") {
        return;
      }

      if (target?.closest("[data-autoscroll-speed], [data-transpose]")) {
        return;
      }

      event.preventDefault();
      setSpeed((current) =>
        changeAutoscrollSpeed(current, event.code === "ArrowUp" ? 1 : -1),
      );
    };

    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, []);

  useEffect(() => {
    if (!playing) {
      return;
    }

    let frame = 0;
    let last = performance.now();
    let pendingPixels = 0;

    const tick = (now: number) => {
      const scroller = document.scrollingElement;
      if (!(scroller instanceof HTMLElement)) {
        return;
      }

      const maxScroll = scroller.scrollHeight - scroller.clientHeight;
      if (maxScroll > 0 && scroller.scrollTop >= maxScroll - 1) {
        setPlaying(false);
        return;
      }

      pendingPixels += (speedRef.current * pixelsPerSpeedUnit * (now - last)) / 1000;
      last = now;
      const pixels = Math.floor(pendingPixels);
      if (pixels > 0) {
        pendingPixels -= pixels;
        scroller.scrollTop += pixels;
      }

      frame = requestAnimationFrame(tick);
    };

    frame = requestAnimationFrame(tick);
    return () => cancelAnimationFrame(frame);
  }, [playing]);

  const adjustSpeed = (direction: -1 | 1) => {
    setSpeed((current) => changeAutoscrollSpeed(current, direction));
  };

  return (
    <div className="flex flex-wrap items-center gap-2">
      <Button
        type="button"
        size="sm"
        variant={playing ? "default" : "outline"}
        className={playing ? "bg-yellow-400 text-black hover:bg-yellow-500" : undefined}
        aria-pressed={playing}
        data-autoscroll-toggle=""
        onClick={() => setPlaying((current) => !current)}
      >
        {playing ? <Pause /> : <Play />}
        {playing ? "Pause" : "Autoscroll"}
      </Button>
      <div
        role="spinbutton"
        aria-label="Speed"
        aria-valuemin={minSpeed}
        aria-valuemax={maxSpeed}
        aria-valuenow={speed}
        aria-valuetext={speed.toFixed(1)}
        tabIndex={0}
        data-autoscroll-speed=""
        className="flex items-center gap-1"
        onKeyDown={(event) => {
          if (event.key === "ArrowUp" || event.key === "ArrowRight") {
            event.preventDefault();
            adjustSpeed(1);
          }
          if (event.key === "ArrowDown" || event.key === "ArrowLeft") {
            event.preventDefault();
            adjustSpeed(-1);
          }
        }}
      >
        <span className="text-sm text-muted-foreground">Speed</span>
        <Button
          type="button"
          size="icon"
          variant="outline"
          className="size-8"
          aria-label="Decrease speed"
          disabled={speed <= minSpeed}
          onClick={() => adjustSpeed(-1)}
        >
          <Minus />
        </Button>
        <span className="w-6 text-center text-sm tabular-nums">{speed.toFixed(1)}</span>
        <Button
          type="button"
          size="icon"
          variant="outline"
          className="size-8"
          aria-label="Increase speed"
          disabled={speed >= maxSpeed}
          onClick={() => adjustSpeed(1)}
        >
          <Plus />
        </Button>
      </div>
    </div>
  );
}

export default GuitarTabAutoscroll;
