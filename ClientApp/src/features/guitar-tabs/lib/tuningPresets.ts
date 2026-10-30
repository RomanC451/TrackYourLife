export const CONCERT_PITCH_HZ = 440;
export const NEEDLE_LIMIT_CENTS = 50;
export const IN_TUNE_CENTS = 5;

export type TunerString = {
  name: string;
  midi: number;
};

export type TuningPresetId =
  | "standard"
  | "half-step-down"
  | "whole-step-down"
  | "drop-d"
  | "drop-c";

export type TuningPreset = {
  id: TuningPresetId;
  name: string;
  strings: readonly TunerString[];
};

const note = (name: string, midi: number): TunerString => ({ name, midi });

export const tuningPresets: readonly TuningPreset[] = [
  {
    id: "standard",
    name: "Standard",
    strings: [
      note("E", 40),
      note("A", 45),
      note("D", 50),
      note("G", 55),
      note("B", 59),
      note("E", 64),
    ],
  },
  {
    id: "half-step-down",
    name: "Half-step down",
    strings: [
      note("Eb", 39),
      note("Ab", 44),
      note("Db", 49),
      note("Gb", 54),
      note("Bb", 58),
      note("Eb", 63),
    ],
  },
  {
    id: "whole-step-down",
    name: "Whole-step down",
    strings: [
      note("D", 38),
      note("G", 43),
      note("C", 48),
      note("F", 53),
      note("A", 57),
      note("D", 62),
    ],
  },
  {
    id: "drop-d",
    name: "Drop D",
    strings: [
      note("D", 38),
      note("A", 45),
      note("D", 50),
      note("G", 55),
      note("B", 59),
      note("E", 64),
    ],
  },
  {
    id: "drop-c",
    name: "Drop C",
    strings: [
      note("C", 36),
      note("G", 43),
      note("C", 48),
      note("F", 53),
      note("A", 57),
      note("D", 62),
    ],
  },
];

export const defaultTuningPreset = tuningPresets[0];

export function tuningPresetById(id: TuningPresetId): TuningPreset {
  return (
    tuningPresets.find((preset) => preset.id === id) ?? defaultTuningPreset
  );
}

export function isTuningPresetId(value: string): value is TuningPresetId {
  return tuningPresets.some((preset) => preset.id === value);
}

export function frequencyFromMidi(midi: number): number {
  return CONCERT_PITCH_HZ * 2 ** ((midi - 69) / 12);
}

export function centsBetween(heardHz: number, targetHz: number): number {
  return 1200 * Math.log2(heardHz / targetHz);
}

export type PitchMatch = {
  stringIndex: number;
  cents: number;
};

const FOLD_CENTS_TOLERANCE = 0.05;

function foldedCents(heardHz: number, targetHz: number): number {
  let closest = centsBetween(heardHz, targetHz);
  for (const factor of [2, 0.5]) {
    const cents = centsBetween(heardHz, targetHz * factor);
    if (Math.abs(cents) < Math.abs(closest)) {
      closest = cents;
    }
  }
  return closest;
}

export function matchPitch(
  heardHz: number,
  strings: readonly TunerString[],
): PitchMatch | null {
  if (!Number.isFinite(heardHz) || heardHz <= 0 || strings.length === 0) {
    return null;
  }

  let bestIndex = 0;
  let bestFolded = Number.POSITIVE_INFINITY;
  let bestTrue = Number.POSITIVE_INFINITY;
  let bestCents = 0;

  strings.forEach((tunerString, index) => {
    const targetHz = frequencyFromMidi(tunerString.midi);
    const cents = foldedCents(heardHz, targetHz);
    const folded = Math.abs(cents);
    const trueDistance = Math.abs(centsBetween(heardHz, targetHz));
    const closerFold = folded < bestFolded - FOLD_CENTS_TOLERANCE;
    const sameFold = Math.abs(folded - bestFolded) <= FOLD_CENTS_TOLERANCE;
    const closerTrue = trueDistance < bestTrue - FOLD_CENTS_TOLERANCE;
    const sameTrue = Math.abs(trueDistance - bestTrue) <= FOLD_CENTS_TOLERANCE;

    if (
      closerFold ||
      (sameFold && closerTrue) ||
      (sameFold && sameTrue && index < bestIndex)
    ) {
      bestIndex = index;
      bestFolded = folded;
      bestTrue = trueDistance;
      bestCents = cents;
    }
  });

  return { stringIndex: bestIndex, cents: bestCents };
}

export function isInTune(cents: number): boolean {
  return Math.abs(cents) <= IN_TUNE_CENTS;
}

export function clampNeedle(cents: number): number {
  return Math.max(-NEEDLE_LIMIT_CENTS, Math.min(NEEDLE_LIMIT_CENTS, cents));
}

export function formatCents(cents: number): string {
  const rounded = Math.round(cents);
  if (rounded > 0) {
    return `+${rounded}`;
  }
  return `${rounded}`;
}
