import { describe, expect, it } from "vitest";

import { colors } from "../tailwindColors";
import { screensEnum } from "../tailwindSizes";

describe("tailwind constants", () => {
  it("exposes the expected color palette", () => {
    expect(colors.blue).toBe("#4A90E2");
    expect(colors.violet).toBe("#6F4BDA");
  });

  it("exposes responsive screen breakpoints", () => {
    expect(screensEnum.sm).toBe(640);
    expect(screensEnum.lg).toBe(1024);
    expect(screensEnum["2xl"]).toBe(1536);
  });
});
