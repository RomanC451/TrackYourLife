import { describe, expect, it } from "vitest";

import { viewModeConfig } from "../consts";

describe("viewModeConfig", () => {
  it("defines labels for chart view modes", () => {
    expect(viewModeConfig.calories.label).toBe("Calories Overview");
    expect(viewModeConfig.nutrients.label).toBe("Nutrients Overview");
  });
});
