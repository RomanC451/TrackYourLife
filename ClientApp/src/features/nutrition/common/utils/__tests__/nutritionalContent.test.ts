import { describe, expect, it } from "vitest";

import type { NutritionalContent } from "@/services/openapi";

import {
  addNutritionalContent,
  createEmptyNutritionalContent,
  multiplyNutritionalContent,
  subtractNutritionalContent,
} from "../nutritionalContent";

function content(
  overrides: Partial<NutritionalContent> = {},
): NutritionalContent {
  const base = createEmptyNutritionalContent();
  return {
    ...base,
    ...overrides,
    energy: {
      unit: "calories",
      value: overrides.energy?.value ?? base.energy.value,
    },
  };
}

describe("createEmptyNutritionalContent", () => {
  it("returns zero values for all nutrients", () => {
    const empty = createEmptyNutritionalContent();

    expect(empty.protein).toBe(0);
    expect(empty.carbohydrates).toBe(0);
    expect(empty.fat).toBe(0);
    expect(empty.energy).toEqual({ unit: "calories", value: 0 });
  });
});

describe("multiplyNutritionalContent", () => {
  it("scales all nutrient fields including energy", () => {
    const source = content({
      protein: 10,
      carbohydrates: 20,
      fat: 5,
      energy: { unit: "calories", value: 100 },
    });

    const result = multiplyNutritionalContent(source, 2);

    expect(result.protein).toBe(20);
    expect(result.carbohydrates).toBe(40);
    expect(result.fat).toBe(10);
    expect(result.energy.value).toBe(200);
  });
});

describe("addNutritionalContent", () => {
  it("sums matching nutrient fields", () => {
    const a = content({ protein: 10, energy: { unit: "calories", value: 50 } });
    const b = content({ protein: 5, energy: { unit: "calories", value: 25 } });

    const result = addNutritionalContent(a, b);

    expect(result.protein).toBe(15);
    expect(result.energy.value).toBe(75);
  });
});

describe("subtractNutritionalContent", () => {
  it("subtracts nutrients from the first operand", () => {
    const a = content({ protein: 10, fat: 8 });
    const b = content({ protein: 3, fat: 2 });

    const result = subtractNutritionalContent(a, b);

    expect(result.protein).toBe(7);
    expect(result.fat).toBe(6);
  });

  it("clamps each field at zero", () => {
    const a = content({ protein: 5, energy: { unit: "calories", value: 10 } });
    const b = content({ protein: 10, energy: { unit: "calories", value: 20 } });

    const result = subtractNutritionalContent(a, b);

    expect(result.protein).toBe(0);
    expect(result.energy.value).toBe(0);
  });
});
