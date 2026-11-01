import { useEffect, useRef, useState } from "react";

import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectSeparator,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { cn } from "@/lib/utils";

import CreateTuningDialog from "./CreateTuningDialog";
import {
  localhostTunerHref,
  useTunerMicrophone,
} from "../hooks/useTunerMicrophone";
import {
  listTunerPresets,
  removeCustomTuning,
  selectTunerPreset,
  selectedTunerPreset,
  useTunerLibrary,
} from "../lib/customTunings";
import {
  useStablePitch,
  useSteppedCents,
  type PitchReading,
} from "../lib/pitchStability";
import { playReferencePluck } from "../lib/referencePluck";
import {
  clampNeedle,
  formatCents,
  frequencyFromMidi,
  matchPitch,
} from "../lib/tuningPresets";

function GuitarTuner() {
  const { status, pitchHz, pitchFrame, request } = useTunerMicrophone();
  const library = useTunerLibrary();
  const presets = listTunerPresets(library);
  const preset = selectedTunerPreset(library);
  const [dialog, setDialog] = useState<"new" | "edit" | null>(null);
  const [lockedIndex, setLockedIndex] = useState<number | null>(null);
  const [plucking, setPlucking] = useState(false);
  const pluckContextRef = useRef<AudioContext | null>(null);
  const pluckStopRef = useRef<(() => void) | null>(null);
  const pluckToken = useRef(0);
  const mountedRef = useRef(true);

  useEffect(() => {
    mountedRef.current = true;
    return () => {
      mountedRef.current = false;
      pluckStopRef.current?.();
      void pluckContextRef.current?.close();
    };
  }, []);

  const playString = (index: number) => {
    pluckStopRef.current?.();
    const token = ++pluckToken.current;
    setLockedIndex(index);
    setPlucking(true);

    if (!pluckContextRef.current) {
      pluckContextRef.current = new AudioContext();
    }
    const audioContext = pluckContextRef.current;
    const frequency = frequencyFromMidi(preset.strings[index].midi);

    void audioContext
      .resume()
      .then(() => {
        if (pluckToken.current !== token) {
          return;
        }
        const playback = playReferencePluck(frequency, audioContext);
        pluckStopRef.current = playback.stop;
        return playback.finished;
      })
      .catch(() => undefined)
      .finally(() => {
        if (mountedRef.current && pluckToken.current === token) {
          setPlucking(false);
        }
      });
  };

  const judgedStrings =
    lockedIndex == null ? preset.strings : [preset.strings[lockedIndex]];
  const heardHz = plucking || status !== "listening" ? null : pitchHz;
  const match = heardHz == null ? null : matchPitch(heardHz, judgedStrings);
  const rawReading: PitchReading | null =
    match == null
      ? null
      : {
          stringIndex: lockedIndex ?? match.stringIndex,
          cents: match.cents,
        };
  const stable = useStablePitch(rawReading, pitchFrame);
  const highlightedIndex = lockedIndex ?? stable?.stringIndex ?? null;
  const cents = useSteppedCents(stable?.cents ?? null);
  const noteName =
    cents != null && highlightedIndex != null
      ? preset.strings[highlightedIndex].name
      : null;
  const inTune = cents === 0;
  const needle = cents == null ? 0 : clampNeedle(cents);

  return (
    <div className="mx-auto flex w-full max-w-lg flex-col items-center gap-8 py-4">
      <div className="grid w-full max-w-sm gap-2">
        <Label htmlFor="tuning-preset">Tuning</Label>
        <Select
          value={preset.id}
          onValueChange={(value) => {
            if (presets.some((option) => option.id === value)) {
              selectTunerPreset(value);
            }
          }}
        >
          <SelectTrigger id="tuning-preset" className="w-full">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            {presets
              .filter((option) => !option.custom)
              .map((option) => (
                <SelectItem key={option.id} value={option.id}>
                  {option.name}
                </SelectItem>
              ))}
            {presets.some((option) => option.custom) ? <SelectSeparator /> : null}
            {presets
              .filter((option) => option.custom)
              .map((option) => (
                <SelectItem key={option.id} value={option.id}>
                  {option.name}
                </SelectItem>
              ))}
          </SelectContent>
        </Select>
        <div className="flex gap-2">
          <Button
            type="button"
            variant="outline"
            className="flex-1"
            onClick={() => setDialog("new")}
          >
            New tuning
          </Button>
          {preset.custom ? (
            <Button
              type="button"
              variant="outline"
              className="flex-1"
              onClick={() => setDialog("edit")}
            >
              Edit
            </Button>
          ) : null}
          {preset.custom ? (
            <Button
              type="button"
              variant="outline"
              className="flex-1"
              onClick={() => removeCustomTuning(preset.id)}
            >
              Remove
            </Button>
          ) : null}
        </div>
        {dialog ? (
          <CreateTuningDialog
            open
            tuning={
              dialog === "edit"
                ? library.customTunings.find((item) => item.id === preset.id)
                : undefined
            }
            onOpenChange={(nextOpen) => {
              if (!nextOpen) {
                setDialog(null);
              }
            }}
          />
        ) : null}
      </div>

      <div
        className="flex w-full flex-col items-center gap-4"
        role="meter"
        aria-label="Tuning meter"
        aria-valuemin={-50}
        aria-valuemax={50}
        aria-valuenow={Math.round(needle)}
        aria-valuetext={
          noteName == null || cents == null
            ? "No pitch"
            : `${noteName} ${formatCents(cents)} cents`
        }
      >
        <p
          data-testid="tuner-note"
          className={cn(
            "text-6xl font-bold tabular-nums",
            noteName == null && "text-muted-foreground",
            inTune && "text-green-500",
          )}
        >
          {noteName ?? "—"}
        </p>
        <p
          data-testid="tuner-cents"
          className={cn(
            "min-h-8 text-2xl font-semibold tabular-nums",
            inTune && "text-green-500",
          )}
        >
          {cents == null ? "" : formatCents(cents)}
        </p>
        <div className="relative h-3 w-full rounded-full bg-muted">
          <div className="absolute left-1/2 top-0 h-full w-px bg-border" />
          <div
            data-testid="tuner-needle"
            className={cn(
              "absolute top-1/2 size-5 -translate-x-1/2 -translate-y-1/2 rounded-full",
              cents == null && "bg-muted-foreground",
              cents != null && !inTune && "bg-primary",
              inTune && "bg-green-500",
            )}
            style={{ left: `${needle + 50}%` }}
          />
        </div>
        {status === "insecure" && (
          <div className="flex flex-col items-center gap-3 pt-2">
            <p className="text-center text-sm text-muted-foreground">
              This address is not secure, so the browser will not open the
              microphone. On this computer, open the tuner through localhost.
              You can still play each string.
            </p>
            <Button asChild variant="outline">
              <a href={localhostTunerHref(window.location)}>Open on localhost</a>
            </Button>
          </div>
        )}
        {status === "denied" && (
          <div className="flex flex-col items-center gap-3 pt-2">
            <p className="text-center text-sm text-muted-foreground">
              The browser blocked the microphone. Allow it for this site in the
              browser settings, then try again. You can still play each string.
            </p>
            <Button type="button" variant="outline" onClick={request}>
              Allow microphone
            </Button>
          </div>
        )}
        {status === "requesting" && (
          <p className="pt-2 text-center text-sm text-muted-foreground">
            Waiting for the microphone. You can still play each string.
          </p>
        )}
      </div>

      <div className="flex w-full items-stretch gap-2">
        {preset.strings.map((tunerString, index) => (
          <Button
            key={`${preset.id}-${index}`}
            type="button"
            variant={highlightedIndex === index ? "default" : "outline"}
            aria-pressed={highlightedIndex === index}
            aria-label={`${tunerString.name}, string ${index + 1}`}
            className="h-14 min-w-0 flex-1 px-0 text-lg font-semibold"
            onClick={() => playString(index)}
          >
            {tunerString.name}
          </Button>
        ))}
        <Button
          type="button"
          variant={lockedIndex == null ? "default" : "outline"}
          aria-pressed={lockedIndex == null}
          className="h-14 min-w-0 flex-1 px-0 text-base font-semibold"
          onClick={() => setLockedIndex(null)}
        >
          Auto
        </Button>
      </div>
    </div>
  );
}

export default GuitarTuner;
