import type { GuitarTabChord, GuitarTabChordShape } from "../api/guitarTabsApi";

const sharpNotes = ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"];
const flatNotes = ["C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B"];

const noteIndex: Record<string, number> = {
  C: 0,
  "B#": 0,
  "C#": 1,
  Db: 1,
  D: 2,
  "D#": 3,
  Eb: 3,
  E: 4,
  Fb: 4,
  F: 5,
  "E#": 5,
  "F#": 6,
  Gb: 6,
  G: 7,
  "G#": 8,
  Ab: 8,
  A: 9,
  "A#": 10,
  Bb: 10,
  B: 11,
  Cb: 11,
};

export function changeTranspose(current: number, direction: -1 | 1) {
  const next = current + direction;
  if (next > 11 || next < -11) {
    return 0;
  }

  return next;
}

export function transposeNote(note: string, semitones: number, useFlats: boolean) {
  const parsed = note.match(/^([A-G])([#b]?)$/);
  if (!parsed) {
    return note;
  }

  const index = noteIndex[parsed[1] + parsed[2]];
  if (index == null) {
    return note;
  }

  const next = (index + semitones + 120) % 12;
  return (useFlats ? flatNotes : sharpNotes)[next];
}

export function transposeChordName(
  name: string,
  semitones: number,
  useFlats: boolean,
) {
  const parsed = name.match(/^([A-G][#b]?)([^/]*)(?:\/([A-G][#b]?))?$/);
  if (!parsed) {
    return name;
  }

  const root = transposeNote(parsed[1], semitones, useFlats);
  const bass = parsed[3] ? `/${transposeNote(parsed[3], semitones, useFlats)}` : "";
  return `${root}${parsed[2]}${bass}`;
}

export function displayChordName(
  name: string,
  semitones: number,
  useFlats: boolean,
) {
  if (semitones === 0 && !useFlats) {
    return name;
  }

  return transposeChordName(name, semitones, useFlats);
}

export function transposeGuitarChord(
  chord: GuitarTabChord,
  semitones: number,
  useFlats: boolean,
): GuitarTabChord {
  const name = displayChordName(chord.name, semitones, useFlats);
  if (semitones === 0) {
    return { ...chord, name };
  }

  const fromRoot = chordRoot(chord.name);
  const toRoot = chordRoot(name);
  const shapes =
    fromRoot == null || toRoot == null
      ? chord.shapes.map((shape) => slideChordShape(shape, semitones))
      : placeChordShapes(chord.shapes, fromRoot, toRoot);

  return {
    ...chord,
    name,
    shapes: shapes.length > 0 ? shapes : chord.shapes,
  };
}

function chordRoot(name: string) {
  const parsed = name.match(/^([A-G][#b]?)/);
  if (!parsed) {
    return null;
  }

  const index = noteIndex[parsed[1]];
  return index == null ? null : index;
}

function placeChordShapes(
  shapes: GuitarTabChordShape[],
  fromRoot: number,
  toRoot: number,
) {
  const delta = (toRoot - fromRoot + 12) % 12;
  if (delta === 0) {
    return shapes;
  }

  const lower = delta - 12;
  const low: GuitarTabChordShape[] = [];
  const high: GuitarTabChordShape[] = [];
  for (const shape of shapes) {
    const shift = fits(shape, lower) ? lower : fits(shape, delta) ? delta : null;
    if (shift == null) {
      continue;
    }

    (shift === lower ? low : high).push(applyShift(shape, shift));
  }

  return dedupeShapes([...low, ...high]);
}

function slideChordShape(shape: GuitarTabChordShape, semitones: number) {
  const shift = shape.frets.some((fret) => fret >= 0 && fret + semitones < 0)
    ? semitones + 12
    : semitones;
  return applyShift(shape, shift);
}

function fits(shape: GuitarTabChordShape, shift: number) {
  return shape.frets.every((fret) => fret < 0 || fret + shift >= 0);
}

function applyShift(shape: GuitarTabChordShape, shift: number): GuitarTabChordShape {
  const frets = shape.frets.map((fret) => (fret < 0 ? -1 : fret + shift));
  const fingers = frets.map((_, index) =>
    frets[index] > 0 ? (shape.fingers[index] ?? 0) : 0,
  );
  const played = frets.filter((fret) => fret > 0);
  const hasOpen = frets.some((fret) => fret === 0);
  const barreFret = shape.barreFret == null ? null : shape.barreFret + shift;
  const keepBarre = barreFret != null && barreFret > 0;

  return {
    ...shape,
    frets,
    fingers,
    position: hasOpen || played.length === 0 ? 0 : Math.min(...played),
    barreFret: keepBarre ? barreFret : null,
    barreFromString: keepBarre ? shape.barreFromString : null,
    barreToString: keepBarre ? shape.barreToString : null,
    barreFinger: keepBarre ? shape.barreFinger : null,
  };
}

function dedupeShapes(shapes: GuitarTabChordShape[]) {
  const seen = new Set<string>();
  return shapes.filter((shape) => {
    const key = shape.frets.join(",");
    if (seen.has(key)) {
      return false;
    }

    seen.add(key);
    return true;
  });
}
