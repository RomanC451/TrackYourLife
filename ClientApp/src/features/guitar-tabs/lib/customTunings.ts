import { useSyncExternalStore } from "react";

import {
  canonicalNoteName,
  defaultTuningPreset,
  parseTuningNotes,
  tuningFromNotes,
  tuningPresetById,
  tuningPresets,
  type TuningPreset,
} from "./tuningPresets";

export type CustomTuning = {
  id: string;
  name: string;
  notes: string[];
};

type TunerLibrary = {
  customTunings: CustomTuning[];
  selectedId: string;
};

const storageKey = "guitar-tabs.tuner-library";

const emptyLibrary: TunerLibrary = {
  customTunings: [],
  selectedId: defaultTuningPreset.id,
};

let current = readLibrary();
const listeners = new Set<() => void>();

function isCustomTuning(value: unknown): value is CustomTuning {
  if (typeof value !== "object" || value == null) {
    return false;
  }
  const record = value as Partial<CustomTuning>;
  return (
    typeof record.id === "string" &&
    typeof record.name === "string" &&
    Array.isArray(record.notes) &&
    record.notes.length === 6 &&
    record.notes.every(
      (note) => typeof note === "string" && canonicalNoteName(note) === note,
    )
  );
}

function readLibrary(): TunerLibrary {
  try {
    const raw = localStorage.getItem(storageKey);
    if (!raw) {
      return { ...emptyLibrary, customTunings: [] };
    }
    const parsed = JSON.parse(raw) as Partial<TunerLibrary>;
    const customTunings = Array.isArray(parsed.customTunings)
      ? parsed.customTunings.filter(isCustomTuning)
      : [];
    const selectedId =
      typeof parsed.selectedId === "string"
        ? parsed.selectedId
        : defaultTuningPreset.id;
    return { customTunings, selectedId };
  } catch {
    return { ...emptyLibrary, customTunings: [] };
  }
}

function emit() {
  listeners.forEach((listener) => listener());
}

function write(next: TunerLibrary) {
  current = next;
  localStorage.setItem(storageKey, JSON.stringify(current));
  emit();
}

export function customTuningToPreset(tuning: CustomTuning): TuningPreset {
  return tuningFromNotes(tuning.id, tuning.name, tuning.notes, true);
}

export function listTunerPresets(
  library: TunerLibrary = current,
): TuningPreset[] {
  return [
    ...tuningPresets,
    ...library.customTunings.map(customTuningToPreset),
  ];
}

export function getTunerLibrary() {
  return current;
}

export function selectedTunerPreset(library: TunerLibrary = current) {
  return tuningPresetById(library.selectedId, listTunerPresets(library));
}

export function selectTunerPreset(id: string) {
  const known = listTunerPresets().some((preset) => preset.id === id);
  write({
    ...current,
    selectedId: known ? id : defaultTuningPreset.id,
  });
}

function createTuningId(): string {
  if (typeof crypto.randomUUID === "function") {
    return `custom-${crypto.randomUUID()}`;
  }
  const bytes = new Uint8Array(16);
  crypto.getRandomValues(bytes);
  const hex = Array.from(bytes, (byte) => byte.toString(16).padStart(2, "0")).join(
    "",
  );
  return `custom-${hex}`;
}

function readTuningFields(
  name: string,
  notesText: string,
): { name: string; notes: string[] } | { error: string } {
  const trimmedName = name.trim();
  if (!trimmedName) {
    return { error: "Name the tuning." };
  }
  const parsed = parseTuningNotes(notesText);
  if ("error" in parsed) {
    return parsed;
  }
  return { name: trimmedName, notes: parsed.notes };
}

export function saveCustomTuning(
  name: string,
  notesText: string,
): { tuning: CustomTuning } | { error: string } {
  const fields = readTuningFields(name, notesText);
  if ("error" in fields) {
    return fields;
  }

  const tuning: CustomTuning = {
    id: createTuningId(),
    name: fields.name,
    notes: fields.notes,
  };
  write({
    customTunings: [...current.customTunings, tuning],
    selectedId: tuning.id,
  });
  return { tuning };
}

export function updateCustomTuning(
  id: string,
  name: string,
  notesText: string,
): { tuning: CustomTuning } | { error: string } {
  const existing = current.customTunings.find((tuning) => tuning.id === id);
  if (!existing) {
    return { error: "That tuning is no longer saved." };
  }
  const fields = readTuningFields(name, notesText);
  if ("error" in fields) {
    return fields;
  }

  const tuning: CustomTuning = {
    id,
    name: fields.name,
    notes: fields.notes,
  };
  write({
    ...current,
    customTunings: current.customTunings.map((item) =>
      item.id === id ? tuning : item,
    ),
  });
  return { tuning };
}

export function removeCustomTuning(id: string) {
  const customTunings = current.customTunings.filter(
    (tuning) => tuning.id !== id,
  );
  write({
    customTunings,
    selectedId:
      current.selectedId === id ? defaultTuningPreset.id : current.selectedId,
  });
}

export function subscribeTunerLibrary(listener: () => void) {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export function useTunerLibrary() {
  return useSyncExternalStore(
    subscribeTunerLibrary,
    getTunerLibrary,
    getTunerLibrary,
  );
}

export function resetTunerLibrary() {
  localStorage.removeItem(storageKey);
  current = { ...emptyLibrary, customTunings: [] };
  emit();
}
