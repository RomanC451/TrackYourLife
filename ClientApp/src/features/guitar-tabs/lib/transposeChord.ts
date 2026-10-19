import type { GuitarTabChord } from "../api/guitarTabsApi";

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

  const shift = chord.frets.some((fret) => fret >= 0 && fret + semitones < 0)
    ? semitones + 12
    : semitones;
  const frets = chord.frets.map((fret) => (fret < 0 ? -1 : fret + shift));
  const played = frets.filter((fret) => fret > 0);
  const hasOpen = frets.some((fret) => fret === 0);
  const position = hasOpen || played.length === 0 ? 0 : Math.min(...played);

  return {
    ...chord,
    name,
    frets,
    position,
    barreFret: chord.barreFret == null ? null : chord.barreFret + shift,
  };
}
