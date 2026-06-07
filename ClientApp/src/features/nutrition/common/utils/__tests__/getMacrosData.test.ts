import { describe, expect, it } from "vitest";

import { colors } from "@/constants/tailwindColors";

import { createEmptyNutritionalContent } from "../nutritionalContent";
import getMacrosData from "../getMacrosData";

describe("getMacrosData", () => {
  it("formats macro masses and percentages", () => {
    const nutrition = createEmptyNutritionalContent();
    nutrition.carbohydrates = 40;
    nutrition.fat = 10;
    nutrition.protein = 10;

    const result = getMacrosData(nutrition, 2);

    expect(result.carbohydrates).toEqual({
      name: "Carbs",
      mass: "80.0",
      percentage: 67,
      color: colors.green,
    });
    expect(result.fat).toEqual({
      name: "Fat",
      mass: "20.0",
      percentage: 17,
      color: colors.yellow,
    });
    expect(result.protein).toEqual({
      name: "Protein",
      mass: "20.0",
      percentage: 17,
      color: colors.violet,
    });
  });

  it("returns zero percentages when all macros are zero", () => {
    const result = getMacrosData(createEmptyNutritionalContent(), 1);

    expect(result.carbohydrates.percentage).toBe(0);
    expect(result.fat.percentage).toBe(0);
    expect(result.protein.percentage).toBe(0);
  });
});
