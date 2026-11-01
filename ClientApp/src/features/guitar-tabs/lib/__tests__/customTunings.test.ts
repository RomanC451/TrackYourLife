import { beforeEach, describe, expect, it } from "vitest";

import {
  getTunerLibrary,
  resetTunerLibrary,
  saveCustomTuning,
  selectedTunerPreset,
  updateCustomTuning,
} from "../customTunings";

describe("custom tunings", () => {
  beforeEach(() => {
    resetTunerLibrary();
  });

  it("stores a named tuning and selects it", () => {
    const result = saveCustomTuning("C# Tuning", "C# F# B E G# C#");
    expect(result).toMatchObject({
      tuning: {
        name: "C# Tuning",
        notes: ["C#", "F#", "B", "E", "G#", "C#"],
      },
    });

    const stored = getTunerLibrary();
    expect(stored.customTunings).toHaveLength(1);
    expect(stored.selectedId).toBe(stored.customTunings[0]?.id);

    const selected = selectedTunerPreset();
    expect(selected.name).toBe("C# Tuning");
    expect(selected.strings.map((tunerString) => tunerString.midi)).toEqual([
      37, 42, 47, 52, 56, 61,
    ]);
    expect(selected.custom).toBe(true);
  });

  it("updates a saved tuning in place", () => {
    const created = saveCustomTuning("C# Tuning", "C# F# B E G# C#");
    if (!("tuning" in created)) {
      throw new Error("expected a saved tuning");
    }

    const updated = updateCustomTuning(created.tuning.id, "Open G", "D G D G B D");
    expect(updated).toEqual({
      tuning: {
        id: created.tuning.id,
        name: "Open G",
        notes: ["D", "G", "D", "G", "B", "D"],
      },
    });
    expect(getTunerLibrary().customTunings).toHaveLength(1);
    expect(selectedTunerPreset().strings.map((tunerString) => tunerString.midi)).toEqual([
      38, 43, 50, 55, 59, 62,
    ]);
  });

  it("rejects a blank name without saving", () => {
    const result = saveCustomTuning("   ", "E A D G B E");
    expect(result).toEqual({ error: "Name the tuning." });
    expect(getTunerLibrary().customTunings).toHaveLength(0);
  });
});
