export const CONCERT_PITCH_HZ = 440;
export const NEEDLE_LIMIT_CENTS = 50;
export const IN_TUNE_CENTS = 5;

export type TunerString = {
  name: string;
  midi: number;
};

export type TuningPreset = {
  id: string;
  name: string;
  strings: readonly TunerString[];
  custom: boolean;
};

const note = (name: string, midi: number): TunerString => ({ name, midi });

const PITCH_CLASS: Record<string, number> = {
  C: 0,
  "C#": 1,
  Db: 1,
  D: 2,
  "D#": 3,
  Eb: 3,
  E: 4,
  F: 5,
  "F#": 6,
  Gb: 6,
  G: 7,
  "G#": 8,
  Ab: 8,
  A: 9,
  "A#": 10,
  Bb: 10,
  B: 11,
};

const LOW_STRING_FLOOR_MIDI = 36;

export function canonicalNoteName(token: string): string | null {
  const compact = token.trim().replace("♯", "#").replace("♭", "b");
  const match = /^([A-Ga-g])([#b])?$/.exec(compact);
  if (!match) {
    return null;
  }
  const name = `${match[1].toUpperCase()}${match[2] ?? ""}`;
  return name in PITCH_CLASS ? name : null;
}

export function parseTuningNotes(
  text: string,
): { notes: string[] } | { error: string } {
  const tokens = text
    .trim()
    .split(/[\s,]+/)
    .filter((token) => token.length > 0);
  if (tokens.length !== 6) {
    return { error: "Enter six notes, from the low string to the high string." };
  }

  const notes: string[] = [];
  for (const token of tokens) {
    const name = canonicalNoteName(token);
    if (!name) {
      return { error: `Couldn't read "${token}". Use names like C# or Bb.` };
    }
    notes.push(name);
  }
  return { notes };
}

export function assignOpenStringMidis(notes: readonly string[]): number[] {
  const midis: number[] = [];
  for (const noteName of notes) {
    const pitchClass = PITCH_CLASS[noteName];
    let midi = LOW_STRING_FLOOR_MIDI + pitchClass;
    const previous = midis[midis.length - 1];
    while (previous != null && midi <= previous) {
      midi += 12;
    }
    midis.push(midi);
  }
  return midis;
}

export function tuningFromNotes(
  id: string,
  name: string,
  notes: readonly string[],
  custom: boolean,
): TuningPreset {
  const midis = assignOpenStringMidis(notes);
  return {
    id,
    name,
    custom,
    strings: notes.map((noteName, index) => note(noteName, midis[index])),
  };
}

export const tuningPresets: readonly TuningPreset[] = [
  {
    id: "standard",
    name: "Standard",
    custom: false,
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
    custom: false,
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
    custom: false,
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
    custom: false,
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
    custom: false,
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

export function tuningPresetById(
  id: string,
  presets: readonly TuningPreset[] = tuningPresets,
): TuningPreset {
  return presets.find((preset) => preset.id === id) ?? defaultTuningPreset;
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
