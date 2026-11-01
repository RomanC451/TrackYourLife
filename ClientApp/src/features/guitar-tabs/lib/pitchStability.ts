import { useRef } from "react";

import { IN_TUNE_CENTS } from "./tuningPresets";

export const CENTS_FOLLOW_WINDOW = 20;
export const STABLE_READING_COUNT = 3;
export const CENTS_STEP = 5;

export type PitchReading = {
  stringIndex: number;
  cents: number;
};

export type PitchStability = {
  shown: PitchReading | null;
  pending: { reading: PitchReading; count: number } | null;
};

function readingsAgree(left: PitchReading, right: PitchReading) {
  return (
    left.stringIndex === right.stringIndex &&
    Math.abs(left.cents - right.cents) <= CENTS_FOLLOW_WINDOW
  );
}

export function stabilizePitch(
  state: PitchStability,
  next: PitchReading | null,
): PitchStability {
  if (next == null) {
    return { shown: null, pending: null };
  }

  if (state.shown != null && readingsAgree(state.shown, next)) {
    return { shown: next, pending: null };
  }

  const pending = state.pending;
  if (pending != null && readingsAgree(pending.reading, next)) {
    const count = pending.count + 1;
    if (count >= STABLE_READING_COUNT) {
      return { shown: next, pending: null };
    }

    return {
      shown: state.shown,
      pending: { reading: next, count },
    };
  }

  return {
    shown: state.shown,
    pending: { reading: next, count: 1 },
  };
}

function snapToFive(cents: number): number {
  const sign = Math.sign(cents);
  return sign * Math.round(Math.abs(cents) / CENTS_STEP) * CENTS_STEP;
}

export function stepCents(shownStep: number | null, rawCents: number): number {
  if (Math.abs(rawCents) <= IN_TUNE_CENTS) {
    return 0;
  }
  const snapped = snapToFive(rawCents);
  if (shownStep == null || Math.abs(rawCents - shownStep) >= CENTS_STEP) {
    return snapped;
  }
  return shownStep;
}

export function useSteppedCents(cents: number | null): number | null {
  const shown = useRef<number | null>(null);
  if (cents == null) {
    shown.current = null;
    return null;
  }
  shown.current = stepCents(shown.current, cents);
  return shown.current;
}

export function useStablePitch(
  reading: PitchReading | null,
  frame: number,
): PitchReading | null {
  const state = useRef<PitchStability & { key: string }>({
    shown: null,
    pending: null,
    key: "",
  });
  const key = `${frame}:${reading == null ? "silent" : "pitch"}`;
  if (state.current.key !== key) {
    state.current = { ...stabilizePitch(state.current, reading), key };
  }
  return state.current.shown;
}
